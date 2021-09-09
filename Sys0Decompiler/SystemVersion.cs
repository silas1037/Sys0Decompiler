using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;


namespace Sys0Decompiler
{
	abstract class SystemVersion
	{
		public static int STRING_MAX = 65535;

		public enum DecompileModeType
		{
			FindLabels,
			ProcessCode,
			AG00,
			SuddenEOF,
			NotDecompiling
		}

		public enum SpecialCase
		{
			None,
			GKing,
			Patton,
			TT2
		}

		protected DecompilerForm parent;

		protected SpecialCase specialCase;

		protected FileStream outputStream;
		protected string curFile;
		private int curLineNum;
		protected string curLine;
		protected int curColumn;
		protected bool deprecated_newStyleTag = false;
		protected bool foundFirstMenu;
		//protected UInt16 curAddress;

		protected DecompileModeType decompileMode;
		protected int firstEOF;
		protected bool junkCodeMode;
		protected FileStream decompileInput;
		protected string decompileDirectory;
		protected byte[] scenarioData;
		protected int scenarioSize;
		protected int scenarioAddress;
		private int linkSector;
		private int dataSector;
		protected int curPage;
		protected int curDisk;
		protected char activeTextOutput = ' ';
		protected int nestingLevel = 0;
		protected List<int> labelAddresses;
		protected List<int> branchEndAddresses;
		protected bool activeSetMenu;
		protected bool fatalError;
		protected List<string> dummiedCommandWarnings;

		private char lastMSXKana;

		public enum IsStandalone
		{
			Yes,
			No,
			Maybe
		};
		protected Dictionary<int, IsStandalone> setMenuRecords;
		//protected int fallbackAddress;
		//protected long fallbackOutput;

		protected class LabelInfo
		{
			public UInt16 TempIndex { get; }
			public bool HasAssignedAddress { get; set; }
			public UInt16 DestinationAddress { get; set; }
			public List<long> CallPositions { get; set; }

			public LabelInfo(int i)
			{
				TempIndex = Convert.ToUInt16(i);
				HasAssignedAddress = false;
				DestinationAddress = 0;
				CallPositions = new List<long>();
			}
		}
		protected Dictionary<string, LabelInfo> labelMap;

		protected Stack<long> branchStarts;

		protected char[] operands;
		protected char[] varStarts;
		protected string[] validVars;
		protected char[] validCmds;
		protected int[] validYCmds;
		protected int numConst8BitMax;

		public SystemVersion(DecompilerForm p)
		{
			parent = p;

			dummiedCommandWarnings = new List<string>();

			labelMap = new Dictionary<string, LabelInfo>();
			labelAddresses = new List<int>();
			branchEndAddresses = new List<int>();
			branchStarts = new Stack<long>();
			setMenuRecords = new Dictionary<int, IsStandalone>();
		}

		public void Initialize()
		{
			dummiedCommandWarnings.Clear();

			labelMap.Clear();
			labelAddresses.Clear();
			branchEndAddresses.Clear();
			branchStarts.Clear();
			setMenuRecords.Clear();
		}

		protected abstract void compile_cmd_calc();
		protected abstract void compile_cmd_branch();
		protected abstract void compile_cmd_end_branch();
		protected abstract void compile_cmd_label_definition();
		protected abstract void compile_cmd_label_jump();
		protected abstract void compile_cmd_label_call();
		protected abstract void compile_cmd_page_jump();
		protected abstract void compile_cmd_page_call();
		protected abstract void compile_cmd_set_menu();
		protected abstract void compile_cmd_set_verbobj();
		protected abstract void compile_cmd_set_verbobj2();

		protected abstract void compile_cmd_a();
		protected abstract void compile_cmd_b();
		protected abstract void compile_cmd_d();
		protected abstract void compile_cmd_e();
		protected abstract void compile_cmd_f();
		protected abstract void compile_cmd_g();
		protected abstract void compile_cmd_h();
		protected abstract void compile_cmd_i();
		protected abstract void compile_cmd_j();
		protected abstract void compile_cmd_k();
		protected abstract void compile_cmd_l();
		protected abstract void compile_cmd_m();
		protected abstract void compile_cmd_n();
		protected abstract void compile_cmd_o();
		protected abstract void compile_cmd_p();
		protected abstract void compile_cmd_q();
		protected abstract void compile_cmd_r();
		protected abstract void compile_cmd_s();
		protected abstract void compile_cmd_t();
		protected abstract void compile_cmd_u();
		protected abstract void compile_cmd_v();
		protected abstract void compile_cmd_w();
		protected abstract void compile_cmd_x();
		protected abstract void compile_cmd_y();
		protected abstract void compile_cmd_z();

		protected abstract void CompileWriteOperand(char c);


		protected abstract void decompile_cmd_calc();
		protected abstract void decompile_cmd_branch();
		protected abstract void decompile_cmd_label_jump();
		protected abstract void decompile_cmd_label_call();
		protected abstract void decompile_cmd_page_jump();
		protected abstract void decompile_cmd_page_call();
		protected abstract void decompile_cmd_set_menu();
		protected abstract void decompile_cmd_set_verbobj();
		protected abstract void decompile_cmd_set_verbobj2();
		protected abstract void decompile_cmd_open_menu();
		protected abstract void decompile_cmd_branch_end();
		protected abstract void automaticBranchEnd();

		protected abstract void decompile_cmd_a();
		protected abstract void decompile_cmd_b();
		protected abstract void decompile_cmd_d();
		protected abstract void decompile_cmd_e();
		protected abstract void decompile_cmd_f();
		protected abstract void decompile_cmd_g();
		protected abstract void decompile_cmd_h();
		protected abstract void decompile_cmd_i();
		protected abstract void decompile_cmd_j();
		protected abstract void decompile_cmd_k();
		protected abstract void decompile_cmd_l();
		protected abstract void decompile_cmd_m();
		protected abstract void decompile_cmd_n();
		protected abstract void decompile_cmd_o();
		protected abstract void decompile_cmd_p();
		protected abstract void decompile_cmd_q();
		protected abstract void decompile_cmd_r();
		protected abstract void decompile_cmd_s();
		protected abstract void decompile_cmd_t();
		protected abstract void decompile_cmd_u();
		protected abstract void decompile_cmd_v();
		protected abstract void decompile_cmd_w();
		protected abstract void decompile_cmd_x();
		protected abstract void decompile_cmd_y();
		protected abstract void decompile_cmd_z();

		protected abstract string decompile_cali();
		protected abstract string decompile_cali2();

		protected char CGetAndWriteChar()
		{
			try
			{
				char c = curLine[curColumn];
				curColumn++;

				WriteByte(c);

				return c;
			}
			catch(IndexOutOfRangeException ex)
			{
				RaiseError("CGetAndWriteChar called out of index, column value too high.", ex);
				return '\0';
			}
		}
		protected char CPeekChar()
		{
			if(curColumn == curLine.Length)
			{
				return '\0';
			}
			return curLine[curColumn];
		}

		// Get and write an 8-bit integer (0-255).
		protected int CGetAndWriteNumericByte(int mod = 0, int min = 0)
		{
			string intStr = "";
			int res = -1;
			while(Char.IsNumber(curLine[curColumn]))
			{
				intStr += curLine[curColumn];
				curColumn++;
			}
			if(intStr.Length == 0)
			{
				RaiseError("Int value expected.");
				return 0;
			}

			res = Int32.Parse(intStr) + mod;

			if(res < min)
			{
				RaiseError("Value cannot be lower than " + min + ".");
				res = min;
			}

			if(res > 255)
			{
				RaiseError("Value cannot be higher than 255.");
				res = 255;
			}

			WriteByte(Convert.ToByte(res));

			return res;
		}

		// Get and write a text parameter. Be aware that TTSys processes text parameters exactly like text
		// output, using the same function and everything, all it does is change the output coords.
		protected void CGetAndWriteTextParam()
		{
			char delimeter = CGetAndWriteChar();

			// Make sure the character we just collected is a quote.
			if(delimeter != '"' && delimeter != '\'')
			{
				RaiseError("Text parameter not enclosed by quote mark.");
				return;
			}

			WriteMessage(delimeter);
		}

		// Even though most words are stored little-endian, variable indicies are big-endian, just to confuse us.
		// Furthermore, the high end is increased by 128 to indicate their role in a calculation - as the high end 
		// cannot ever get this high due to conditionals, this allows the high end to be decrypted using & 0x3f.
		//
		// Calculation elements are essentially arranged as follows in System 1:
		//	- High end 0x00-0x3f (0-63): Indicates a 16 bit digit, which in practice means 55-16383.
		//  - High end 0x40-0x77 (64-119): Indicates an 8-bit digit, from 0-54.
		//  - High end 0x78-0x7e (120-126): Operands (*, +, -, =, <, >, \). 
		//  - High end 0x7f		 (127): DEL, end equation.
		//  - High end 0x80-0xbf (128-191): Indicates an 8-bit variable index, from 0-63.
		//  - High end 0xc0-0xff (192-255): Indicates a 16-bit variable index, in practice anywhere from 64-9999.
		//
		// In System 2 and 3, we have to make room for division, which is done by shortening the range for 8-bit digits
		// to 0x40 through 0x76 (53), move multiplication to 0x77, and put division at 0x78. This does not impact
		// WriteVariableIndex, but does impact WriteNumericConst via numConst8BitMax. The operands are handled in 
		// CompileCaliConvertAndWrite.
		protected void WriteVariableIndex(int varIndex)
		{
			// Variables are stored by index number, and are encoded big-endian. If <= 63, they are digits,
			// but if larger, they are words. For whatever reason, the little end of the variable is then increased
			// by 128 (0x3f).
			if(varIndex <= 63)
			{
				WriteByte(varIndex + 0x80);
				return;
			}

			// Indicies higher than 63 instead become two - char values. The upper maximum is provided by
			// format, since var names do not go beyond VAR9999, so we don't need to worry about that.
			// First, isolate the two halves of the number.
			byte chr1 = Convert.ToByte(varIndex >> 8); // varIndex & 0x3f00;
			byte chr2 = Convert.ToByte(varIndex & 0xff); // varIndex & 0xff;

			// chr2 is left alone from this point, but chr1 is obfuscated by obfuscating it as above.
			chr1 += 0xc0;

			WriteByte(chr1);
			WriteByte(chr2);
		}

		// Constant numbers go thorugh a similar process to variable indicies, but to distinguish them from variables,
		// their first digit is modified differently: +64 instead of +128. To avoid overlap with operands, they can
		// only be a single byte if their value is <= 54 instead of 63.
		void WriteNumericConst(int i)
		{
			if(i <= numConst8BitMax)
			{
				WriteByte(i + 64);
				return;
			}

			// Indicies higher than 63 instead become two - char values. The upper maximum is provided by
			// format, since var names do not go beyond VAR9999, so we don't need to worry about that.
			// First, isolate the two halves of the number.
			byte chr1 = Convert.ToByte(i >> 8); // varIndex & 0x3f00;
			byte chr2 = Convert.ToByte(i & 0xff); // varIndex & 0xff;

			// chr2 is left alone from this point, but chr1 is obfuscated as above.
			//chr1 += 64;

			WriteByte(chr1);
			WriteByte(chr2);
		}

		protected void CaliConvertAndWrite(string s)
		{
			// Constant, postive or zero integer
			int n;
			if(Int32.TryParse(s, out n))
			{
				WriteNumericConst(n);
			}
			// Operand
			else if(CaliIsOp(s[0]))
			{
				CompileWriteOperand(s[0]);
			}
			// Variable.
			else
			{
				WriteVariableIndex(varNameToIndex(s));
			}
		}

		// Write a temporary label. SCO files indicate labels by address within the file, but we have no way of
		// knowing the accurate address until 1) the string labels are converted to 16 bits, and 2) the label 
		// definitions, which do not appear in SCO, are completely erased. For this reason, we will convert the
		// labels to temporary 16 bit indicies, and come back to them in a second pass after the fact.

		protected ushort GetLabelCall(string strLabel)
		{
			// If the labelMap does not contain the key, create a temporary entry for it, and write the index of that entry
			// to file. Then, add it to the list of positions to correct later.
			if(!labelMap.ContainsKey(strLabel))
			{
				if(labelMap.Count < UInt16.MaxValue)
				{
					labelMap.Add(strLabel, new LabelInfo(labelMap.Count));
					labelMap[strLabel].CallPositions.Add(outputStream.Position);
					return labelMap[strLabel].TempIndex;
				}
				else
				{
					// We can't have more labels than UInt16 has values, though that's extremely unlikely.
					RaiseError("Too many labels in file.");
					return 0;
				}
			}
			else
			{
				// If the labelMap does contain the key, and does contain an address, write the address to file.
				if(labelMap[strLabel].HasAssignedAddress)
				{
					return labelMap[strLabel].DestinationAddress;
				}
				// If it does not have an address, write its index to file and add it to the list of positions to correct
				// later.
				else
				{
					labelMap[strLabel].CallPositions.Add(outputStream.Position);
					return labelMap[strLabel].TempIndex;
				}

			}
		}

		protected void WriteLabelCall(string strLabel)
		{
			WriteWord(GetLabelCall(strLabel));
		}

		// Define a label. Set its address to the current position in file, and then go back over all past calls to this
		// label and overwrite the indicies we wrote in their place with the proper address. Then, return the header to
		// its current position.
		protected void DefineLabel(string strLabel)
		{
			if(!labelMap.ContainsKey(strLabel))
			{
				labelMap.Add(strLabel, new LabelInfo(labelMap.Count));
			}
			labelMap[strLabel].HasAssignedAddress = true;
			labelMap[strLabel].DestinationAddress = Convert.ToUInt16(outputStream.Position);// curAddress;

			//Console.Write(curAddress.ToString(), outputStream.Position);

			long curPosition = outputStream.Position;
			while(labelMap[strLabel].CallPositions.Count() > 0)
			{
				outputStream.Position = labelMap[strLabel].CallPositions[0];
				WriteWord(labelMap[strLabel].DestinationAddress);
				labelMap[strLabel].CallPositions.RemoveAt(0);
			}
			outputStream.Position = curPosition;
		}

		// Similar to the label functions above, we must also prep and later clarify branch ends in Sys2 and 3.
		// Because branches only have one start and one end, and always occur in that order, these functions
		// are less numerous and all-around smaller.
		//
		// Because of the way System code is structured, we can deal with branch end records first-in-last-out.
		// All nested branches are guarenteed to end before the master branch. In short, add each branch record
		// to the back of the list, and then pop them off the back as they resolve.
		protected ushort PrepBranchEnd()
		{
			if(branchStarts.Count < UInt16.MaxValue)
			{
				branchStarts.Push(outputStream.Position);
				return 0; // filler
			}
			else
			{
				// We can't have more active branches than UInt16 has values, though that's ASTRONOMICALLY unlikely
				// in an actual use case, and is probably the sign of some manner of error.
				RaiseError("Too many active branches in file (syntax error likely).");
				return 0;
			}
		}

		protected void DefineBranchEnd()
		{
			//Console.Write(curAddress.ToString(), outputStream.Position);

			long curPosition = outputStream.Position;
			outputStream.Position = branchStarts.Pop();
			WriteWord(Convert.ToUInt16(curPosition));
			outputStream.Position = curPosition;
		}

		protected string GetAndWriteStringTo(string delimeter)
		{
			string res = GetStringTo(delimeter);

			WriteText(res);
			return res;
		}

		protected string GetStringTo(string delimeter)
		{
			int delimeterIndex = curLine.IndexOf(delimeter, curColumn);

			if(delimeterIndex == -1)
			{
				RaiseError("Delimeter \"" + delimeter + "\" not found.");
				return curLine;
			}
			string res = curLine.Substring(curColumn, delimeterIndex - curColumn);
			curColumn += res.Length;

			return res;
		}

		// Write text for output up to the given delimeter, but not if it is preceded by an escape character.
		protected void WriteMessage(char delimeter)
		{
			bool escapeNext = false, delimeterFound = false;
			int count = 0;
			byte[] nextChar;
			
			for(int i = curColumn; i < curLine.Length; i++)
			{
				count++;

				// Gaiji. Process any message text starting with 0x and followed by four digits (and not exited 
				// at any point using a \ character) by directly inserting the two bytes it describes into the
				// message in its place.
				if((!escapeNext && curLine[i] == 'G' && curLine[i + 1] == '+') ||
					(!escapeNext && curLine[i] == '0' && curLine[i + 1] == 'x'))
				{
					// In unicode-mode, we need to remap gaiji characters to U+E000-E0BB, and encode in UTF-8.
					int decVal = 0;
					if(Int32.TryParse(curLine.Substring(i+2, 4), System.Globalization.NumberStyles.HexNumber, 
						System.Globalization.CultureInfo.InvariantCulture, out decVal))
					{
						if(parent.CurCompileOutMode == DecompilerForm.SourceEncodingMode.ShiftJIS)
						{
							WriteByte(decVal >> 8);
							WriteByte(decVal & 0xff);
						}
						else if(parent.CurCompileOutMode == DecompilerForm.SourceEncodingMode.UTF8)
						{
							int index = 0;
							if(0xeb9f <= decVal && decVal <= 0xebfc)
							{
								index = decVal - 0xeb9f;
							}
							else if(0xec40 <= decVal && decVal <= 0xec9e)
							{
								index = decVal - 0xec40 + 94;
							}
							// The GBK port uses different Gaiji area, 0xeb9f-0xebfc and 0xec40-0xec9e.
							// This is temporary code that allows existing .ADV files for the GBK port compile without modification.
							else if(0xff40 <= decVal && decVal <= 0xff9d)
							{
								index = decVal - 0xff40;
							}
							else if(0xff9e <= decVal && decVal <= 0xfffc)
							{
								index = decVal - 0xff9e + 94;
							}
							// Temporary code end.
							else
							{
								RaiseError("U+#### outside the gaiji range.");
							}

							// Encode it as a UTF-8 character U+E000+index.
							WriteByte(0xee);
							WriteByte(0x80 | index >> 6);
							WriteByte(0x80 | index & 0x3f);
						}

						i += 5;
						count += 5;

						continue;
					}
				}

				nextChar = parent.CompileOutputEncoding.GetBytes(curLine[i].ToString());
				
				foreach(byte b in nextChar)
				{
					WriteByte(b);
				}

				if(!escapeNext && curLine[i] == delimeter)
				{
					delimeterFound = true;
					break;
				}

				if(escapeNext)
				{
					// Preserving this line in case we use this function as a copy-paste for processing.
					// if(curLine[i] == '\\' || curLine[i] == '\'' || curLine[i] == '"')
					escapeNext = false;
				}
				else if(curLine[i] == '\\') { 
					escapeNext = true;
				}
			}

			curColumn += count;

			if(!delimeterFound)
			{
				RaiseError("Message does not end with close quotes.");
			}

			//SkipClosingMark(delimeter);
		}

		public void WriteByte(byte b)
		{
			outputStream.WriteByte(b);
			//curAddress++;
		}

		public void WriteByte(int i)
		{
			outputStream.WriteByte(Convert.ToByte(i));
			//curAddress++;
		}

		public void WriteByte(char c)
		{
			outputStream.WriteByte(Convert.ToByte(c));
			//curAddress++;
		}

		public void WriteShiftJISChar(string c)
		{
			byte[] bytes = Encoding.GetEncoding("shift_jis").GetBytes(c);
			outputStream.Write(bytes, 0, bytes.Length);

			lastMSXKana = c[0];
		}

		// Words are written little-endian.
		public void WriteWord(UInt16 w)
		{
			WriteByte(w & 0xff);
			WriteByte(w >> 8);
			//curAddress += 2;

			/*compile_data.push_back(data & 0xff);
			compile_data.push_back(data >> 8);
			compile_addr += 2;*/
		}

		protected void WriteText(string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			//List<byte> info2 = info.ToList();
			outputStream.Write(info, 0, info.Length);
			//curAddress += Convert.ToUInt16(info.Length);
		}

		// The address of the first menu call ([) in a file is written to index 0.
		protected void WriteFirstMenu()
		{
			long curAddr = outputStream.Position;
			outputStream.Seek(0, SeekOrigin.Begin);
			WriteWord(Convert.ToUInt16(curAddr-1));
			outputStream.Seek(curAddr, SeekOrigin.Begin);

			foundFirstMenu = true;
		}

		public void SetOutputFile(string outputFile)
		{
			if(outputStream != null) outputStream.Close();
			outputStream = File.Create(outputFile);
		}

		public void CloseOutputFile()
		{
			if(outputStream != null) outputStream.Close();
		}

		private static readonly Regex sWhitespace = new Regex(@"\s+");

		public bool CompilePage(string inputFile, string outputFile)
		{
			char cmd;
			activeSetMenu = false;

			SetOutputFile(outputFile);
			foundFirstMenu = false;

			// Skip the first two bytes. These bytes are reserved for WriteFirstMenu, or for a process 
			// at the end of this function if the former is never called.
			outputStream.Seek(2, SeekOrigin.Begin);

			string[] lines = File.ReadAllLines(inputFile, parent.CompileSourceEncoding); // Read lines in Shift-JIS.

			curFile = inputFile;
			curLine = "";
			curLineNum = 0;
			//curAddress = 0;
			labelMap.Clear();

			foreach(string line in lines)
			{
				curLineNum++;
				curColumn = 0;
				curLine = "";

				char curQuote = ' ';
				bool escapeNext = false;

				// Remove whitespace from line, unless it is inside of quote, while accounting for escape characters
				// inside of quotes.
				for(int i=0; i<line.Length; i++)
				{
					// If we are not in quotes, remove whitespace with impunity. While we're at this, check to see if the 
					// next char is a quote. If it is, add it and start quotes.
					// i
					if(curQuote == ' ')
					{
						if(line[i] != ' ' && line[i] != '\t')
							curLine += line[i];

						if(line[i] == '\'' || line[i] == '"')
							curQuote = line[i];
					}
					// If we are in quotes, allow whitespace, but keep an eye out for closing quotes and escape characters.
					else
					{
						if(!escapeNext)
						{
							curLine += line[i];
							if(line[i] == '\\')
							{
								escapeNext = true;
							}
							else if(line[i] == curQuote)
							{
								curQuote = ' ';
							}
						}
						else
						{
							if(line[i] == '\\' || line[i] == '\'' || line[i] == '"')
								curLine += line[i];

							escapeNext = false;
						}
					}
				}

				// Empty lines can be passed.
				if(curLine.Length == 0) continue;

				// All commands in Sys1-3 are one char at the start of the line.
				cmd = CPeekChar();
				curColumn++;

				// Do not write * commands, which have no physical presence in SCO files.
				if(cmd != '*' && cmd != ';')
				{
					if(cmd > 255)
					{
						RaiseError("Invalid command " + cmd + ".");
						return false;
					}
					WriteByte(cmd);
				}

				switch(cmd)
				{
					case '!':
						compile_cmd_calc();
						break;
					case '{':
						compile_cmd_branch();
						break;
					case '}':
						compile_cmd_end_branch();
						break;
					case '*':
						// Only exists during compile, as label definitions are erased.
						compile_cmd_label_definition();
						break;
					case '@':
						compile_cmd_label_jump();
						break;
					case '\\':
						compile_cmd_label_call();
						break;
					case '&':
						compile_cmd_page_jump();
						break;
					case '%':
						compile_cmd_page_call();
						break;
					case '$':
						compile_cmd_set_menu();
						break;
					case '[':
						if(!foundFirstMenu) WriteFirstMenu();
						compile_cmd_set_verbobj();
						break;
					case ':':
						if(!foundFirstMenu) WriteFirstMenu();
						compile_cmd_set_verbobj2();
						break;
					case ']':
						// Do nothing more.
						break;
					case 'A':
						compile_cmd_a();
						break;
					case 'B':
						compile_cmd_b();
						break;
					case 'D':
						compile_cmd_d();
						break;
					case 'E':
						compile_cmd_e();
						break;
					case 'F':
						compile_cmd_f();
						break;
					case 'G':
						compile_cmd_g();
						break;
					case 'H':
						compile_cmd_h();
						break;
					case 'I':
						compile_cmd_i();
						break;
					case 'J':
						compile_cmd_j();
						break;
					case 'K':
						compile_cmd_k();
						break;
					case 'L':
						compile_cmd_l();
						break;
					case 'M':
						compile_cmd_m();
						break;
					case 'N':
						compile_cmd_n();
						break;
					case 'O':
						compile_cmd_o();
						break;
					case 'P':
						compile_cmd_p();
						break;
					case 'Q':
						compile_cmd_q();
						break;
					case 'R':
						compile_cmd_r();
						break;
					case 'S':
						compile_cmd_s();
						break;
					case 'T':
						compile_cmd_t();
						break;
					case 'U':
						compile_cmd_u();
						break;
					case 'V':
						compile_cmd_v();
						break;
					case 'W':
						compile_cmd_w();
						break;
					case 'X':
						compile_cmd_x();
						break;
					case 'Y':
						compile_cmd_y();
						break;
					case 'Z':
						compile_cmd_z();
						break;
					case '"':
						WriteMessage('"');
						break;
					case '\'':
						WriteMessage('\'');
						break;
					case '\0':
						RaiseError("Null character in place of command. Null characters in this possition may " +
							"be innocent or or may sign of further errors. They must be manually investigated " +
							"and then deleted.");
						break;
					case ';':
						// Comment. Do nothing, unfortunately they can't be saved.
						break;
					default:
						RaiseError("Unknown command: " + cmd);
						break;
				}
			}

			// End of file (EOF).
			long EOFaddr = outputStream.Position;
			WriteByte(0x1A);

			// If WriteFirstMenu has never been called, fill the first two bytes with the location of the 
			// EOF character.
			if(!foundFirstMenu)
			{
				outputStream.Seek(0, SeekOrigin.Begin);
				WriteWord(Convert.ToUInt16(EOFaddr));
			}

			outputStream.Close();

			return true;
		}

		protected int varNameToIndex(string label)
		{
			if(label.Length == 3)
			{
				if(label == "RND") return 0;
				if(label == "D01") return 1;
				if(label == "D02") return 2;
				if(label == "D03") return 3;
				if(label == "D04") return 4;
				if(label == "D05") return 5;
				if(label == "D06") return 6;
				if(label == "D07") return 7;
				if(label == "D08") return 8;
				if(label == "D09") return 9;
				if(label == "D10") return 10;
				if(label == "D11") return 11;
				if(label == "D12") return 12;
				if(label == "D13") return 13;
				if(label == "D14") return 14;
				if(label == "D15") return 15;
				if(label == "D16") return 16;
				if(label == "D17") return 17;
				if(label == "D18") return 18;
				if(label == "D19") return 19;
				if(label == "D20") return 20;
				if(label == "U01") return 21;
				if(label == "U02") return 22;
				if(label == "U03") return 23;
				if(label == "U04") return 24;
				if(label == "U05") return 25;
				if(label == "U06") return 26;
				if(label == "U07") return 27;
				if(label == "U08") return 28;
				if(label == "U09") return 29;
				if(label == "U10") return 30;
				if(label == "U11") return 31;
				if(label == "U12") return 32;
				if(label == "U13") return 33;
				if(label == "U14") return 34;
				if(label == "U15") return 35;
				if(label == "U16") return 36;
				if(label == "U17") return 37;
				if(label == "U18") return 38;
				if(label == "U19") return 39;
				if(label == "U20") return 40;
				if(label == "B01") return 41;
				if(label == "B02") return 42;
				if(label == "B03") return 43;
				if(label == "B04") return 44;
				if(label == "B05") return 45;
				if(label == "B06") return 46;
				if(label == "B07") return 47;
				if(label == "B08") return 48;
				if(label == "B09") return 49;
				if(label == "B10") return 50;
				if(label == "B11") return 51;
				if(label == "B12") return 52;
				if(label == "B13") return 53;
				if(label == "B14") return 54;
				if(label == "B15") return 55;
				if(label == "B16") return 56;
				if(label == "B16") return 56;
				if(label == "M_X") return 57;
				if(label == "M_Y") return 58;
				else
				{
					RaiseError("Error, unknown var label " + label + ".");
					return -1;
				}
			}
			else if(label.Substring(0, 3) == "VAR")
			{
				try
				{
					UInt16 index = UInt16.Parse(label.Substring(3));

					return index;
				}
				catch(FormatException e) {
					RaiseError("Error, invalid arguement in VAR varable number" + label.Substring(3) +
						".", e);
					return -1;
				}
			}
			else
			{
				RaiseError("Error, unknown var label " + label + ".");

				return -1;
			}
		}


		// Cali functions.
		private bool CaliIsOp(char c)
		{
			if(operands.Contains(c)) return true;
			return false;
		}

		private bool CaliIsVarStart(char c)
		{
			if(varStarts.Contains(c)) return true;
			return false;
		}

		private bool CaliIsNumberedVar(string s)
		{
			if(s.Length != 7) return false;
			if(s.Substring(0, 3) != "VAR") return false;
			if(!Int32.TryParse(s.Substring(3), out int n)) return false;

			return true;
		}

		// Finds an operand in one direction or the other. The chief difference between this and IndexOf(Any) is
		// that it must ignore anyting inside code @ tags, while still returning indicies that remark relative to those
		// code tags.
		private int CaliFindOperandCommon(string calc, char operand, int curIndex, int start, int end, int step)
		{
			bool insideTag = false;

			for(int i = start; i != end; i += step)
			{
				// Only search for operands OUTSIDE of @ tags.
				if(calc[i] == '@')
				{
					insideTag = !insideTag;
				}
				else if(!insideTag && calc[i] == operand)
				{
					// We want the nearest index. If we're going forward, we want the lowest value. If we're going
					// backward, the highest.
					if((step > 0 && i < curIndex) || (i > curIndex))
					{
						return i;
					}
				}
			}

			return curIndex;
		}

		private int CaliFindNearestGivenOperand(string calc, char operand, int start, int end, int step = 1)
		{
			if(step == 0) return -1;

			int curIndex = STRING_MAX;
			if(step < 0) curIndex = -1;

			curIndex = CaliFindOperandCommon(calc, operand, curIndex, start, end, step);

			if(curIndex == STRING_MAX)
				return -1;

			return curIndex;
		}

		private int CaliFindNearestOperand(string calc, int start, int end, int step = 1)
		{
			if(step == 0) return -1;

			int curIndex = STRING_MAX;
			if(step < 0) curIndex = -1;

			foreach(char operand in operands)
				curIndex = CaliFindOperandCommon(calc, operand, curIndex, start, end, step);

			if(curIndex == STRING_MAX)
				return -1;

			return curIndex;
		}


		// Determine that the equation follows the pattern : operator, operand, operator, etc.
		private bool CaliValidateEquation(string calc)
		{
			int index = 0;
			int nextOperandIndex = -1;
			bool operatorNext = true;
			int calcLen = calc.Length;

			// There's actually just one operator in the func, but "operator" is reserved. operator1 is used elsewhere, so
			// was brought here for consistency.
			string operator1 = "";

			while(index < calcLen)
			{
				char c = calc[index];

				if(operatorNext)
				{
					// The operator must be followed by either an operand or the end of the equation.
					nextOperandIndex = CaliFindNearestOperand(calc, index, calcLen);
					//std::cout << "next" << nextOperandIndex << std::endl;

					if(nextOperandIndex == -1)
						nextOperandIndex = calcLen;

					operator1 = calc.Substring(index, nextOperandIndex - index);
					//std::cout << c << operator1 << std::endl;

					// There are three kinds of operators : constants, variables, and previously completed
					// equations marked by @code@ tags.
					if(c == '@')
					{
						// Because @code@ tags are dynamically inserted, they don't need to be validated, so do nothing,
						// just update the index.
					}
					else if(Char.IsNumber(c) || c == '|')
					{
						// Double check that the entire string from start to the next operator is an int.
						if(!Int32.TryParse(operator1, out int n))
						{
							RaiseError("Invalid constant: \"" + operator1 + "\"");
							return false;
						}
					}
					else if(CaliIsVarStart(c))
					{
						// If operator1 is not in valid vars, and is also not a numbered var (VARXXXX), error.
						if(!validVars.Contains(operator1) && !CaliIsNumberedVar(operator1))
						{
							RaiseError("Invalid variable: \"" + operator1 + "\"");
							return false;
						}
					}
					else
					{
						RaiseError("Invalid entry in operator slot. Starts with \"" + c + "\".");
						return false;
					}

					index = nextOperandIndex;
					operatorNext = false;
				}
				else
				{
					if(!CaliIsOp(c))
					{
						RaiseError("Invalid entry in operand slot: \"" + c + "\".");
						return false;
					}

					index++;
					operatorNext = true;
				}
			}

			return true;
		}

		// Break an equation into its component parts.
		private string CaliProcessEquation(string eq)
		{
			int opIndex = -1;
			int eqLen = eq.Length;
			int eqStart = 0, eqEnd = 0;
			string prevEq = "", operator1 = "", operator2 = "", code = "";

			// Begin converting equation to codes in listed (presumably PEDMAS) order.
			for(int i = 0; i < operands.Length; i++)
			{
				// Continue searching for the operator until non is found.
				while(true)
				{
					opIndex = CaliFindNearestGivenOperand(eq, operands[i], 0, eqLen);

					if(opIndex == -1)
						break;

					// Find the previous operand, if any.
					prevEq = eq.Substring(0, opIndex);
					eqStart = CaliFindNearestOperand(prevEq, prevEq.Length - 1, 0, -1) + 1;
					if(eqStart == -1) eqStart = 0;

					// And the next operand, if any.
					eqEnd = CaliFindNearestOperand(eq, opIndex + 1, eqLen);
					if(eqEnd == -1) eqEnd = eqLen;

					// One operator to the left of the operand, and one to the right.
					operator1 = eq.Substring(eqStart, opIndex - eqStart);
					operator2 = eq.Substring(opIndex + 1, eqEnd - (opIndex + 1));

					// Remove the @ signs from previously encoded parts of the equation.
					if(operator1[0] == '@') operator1 = operator1.Substring(1, operator1.Length - 2);
					if(operator2[0] == '@') operator2 = operator2.Substring(1, operator2.Length - 2);

					// Insert the code, including a comma that will be removed later, once we can use WriteByte to
					// divide the digits instead of a divider character.
					code = "@" + operator1 + "," + operator2 + "," + operands[i] + "@";

					// Remove the equation and replace it with the new code.
					eq = eq.Substring(0, eqStart) + code + eq.Substring(eqEnd);
					eqLen = eq.Length;
				}
			}
			return eq;
		}

		private int CaliFindNearestParen(string calc, int start)
		{
			int openIndex = calc.IndexOf("(", start);
			int closeIndex = calc.IndexOf(")", start);

			if(openIndex == -1)
				return closeIndex;
			else if(closeIndex == -1)
				return openIndex;
			else if(openIndex < closeIndex)
				return openIndex;

			return closeIndex;
		}

		private int CountChar(string s, char c)
		{
			int res = 0;
			for(int i=0;i<s.Length;i++)
			{
				if(s[i] == c) res++;
			}
			return res;
		}

		private int FindParenEnd(string eq, int parenStart)
		{
			// Find the matching close paren.Search for the nearest paren of either sort,
			// adding to a tally whenever a new one opens and subtracting each time one closes,
			// until we finally find a close when the tally is 0.
			int extraParenTally = 0;
			int nextIndex = parenStart + 1;

			while(true)
			{
				nextIndex = CaliFindNearestParen(eq, nextIndex);
				if(eq[nextIndex] == '(')
					extraParenTally += 1;
				else if(extraParenTally > 0)
					extraParenTally -= 1;
				else
					break;

				nextIndex += 1;
			}

			return nextIndex;
		}


		private static readonly Regex sCali = new Regex(@"\s\$@");

		protected void compile_cali(string delimeter)
		{
			// Grab the entire equation up to a delimeter.
			string eq = GetStringTo(delimeter);

			// Strip all spaces and tabs from the string to simplify matters, and remove @ and $ signs since we use
			// them for special purposes during processing.
			eq = sCali.Replace(eq, "");

			// Some sort of error, bail.
			if(eq == "")
			{
				RaiseError("Invalid calculation, probable syntax error.");
				return;
			}

			int eqLen = eq.Length;

			// If the entire equation is in matching parens, erase them. Start with a quick check to see if there
			// are parens at the start and end of the equation.
			if(eq[0] == '(' && eq[eqLen - 1] == ')')
			{
				// Confirm the closing parens are the counterpart to the opening ones.
				if(FindParenEnd(eq, 0) == eqLen - 1)
				{
					eq = eq.Substring(1, eqLen - 2);
					//eqLen -= 2;
				}
			}

			// Fast-track "calis" that are really just a single operator, with no operands.
			if(CaliFindNearestOperand(eq, 0, eq.Length) == -1)
			{
				CaliConvertAndWrite(eq);

				// End with a DEL character.
				WriteByte(0x7f);
				return;
			}

			// Isolate parentheses.The whole equation is considered our 0th paren.
			List<string> parens = new List<string> { eq };

			// Make sure there's an equal number of opening and closing parens.
			int cntOpen = CountChar(eq, '(');
			int cntClose = CountChar(eq, ')');

			if(cntOpen != cntClose)
			{
				RaiseError("Uneven number of opening and closing parentheses in calculation.");
				return;
			}

			// Navigate through the parens.
			int parenIndex = 0, parenStart = 0, parenEnd = 0;
			string expression = "";

			while(parenIndex < parens.Count)
			{
				parenStart = parens[parenIndex].IndexOf('(');

				while(parenStart > -1)
				{
					parenEnd = FindParenEnd(parens[parenIndex], parenStart);

					// Extract the expression and leave out the parens.
					expression = parens[parenIndex].Substring(parenStart + 1, parenEnd - parenStart - 1);
					parens.Add(expression);

					// Replace the expression(including the parens) with a $ tag indicating its place in parens.
					parens[parenIndex] = parens[parenIndex].Substring(0, parenStart) + "$" + (parens.Count - 1) +
						"$" + parens[parenIndex].Substring(parenEnd + 1);

					// Next loop.
					parenStart = parens[parenIndex].IndexOf("(", parenStart + 1);
				}

				parenIndex++;
			}

			// print(parens)

			int tagIndex = 0, endTagIndex = 0, subParenIndex = 0;
			// Process the parens in reverse.
			for(int i = parens.Count - 1; i >= 0; i--)
			{
				// Replace any $ tags with what remains in the paren of the same number.
				tagIndex = parens[i].IndexOf("$");

				while(tagIndex > -1)
				{
					endTagIndex = parens[i].IndexOf("$", tagIndex + 1);

					// std::cout << parens[i] << " " << tagIndex + 1 << " " << endTagIndex << std::endl;
					// std::cout << parens[i].Substring(tagIndex + 1, endTagIndex - tagIndex + 1) << std::endl;
					subParenIndex = Int32.Parse(parens[i].Substring(tagIndex + 1, endTagIndex - (tagIndex + 1)));

					parens[i] = parens[i].Substring(0, tagIndex) + parens[subParenIndex] +
						parens[i].Substring(endTagIndex + 1);

					// Next loop.
					tagIndex = parens[i].IndexOf("$");
				}

				if(!CaliValidateEquation(parens[i]))
				{
					RaiseError("Invalid equation segment '" + parens[i] + "'.");
					return;
				}

				parens[i] = CaliProcessEquation(parens[i]);
			}

			// Remove the final set of @ tags, if present(they might not be present if the calc is simple, like
			// a single constant).
			if(parens[0][0] == '@')
				parens[0] = parens[0].Substring(1, parens[0].Length - 2);


			// Output.
			// cout << parens[0])

			// Remember to remove commas from the code as we use writed().
			int startIndex = 0;
			int commaIndex = parens[0].IndexOf(",");
			while(commaIndex > -1)
			{
				CaliConvertAndWrite(parens[0].Substring(startIndex, commaIndex - startIndex));

				startIndex = commaIndex + 1;
				commaIndex = parens[0].IndexOf(",", startIndex);
			}
			// Convert the tail end as well.
			CaliConvertAndWrite(parens[0].Substring(startIndex));

			// End with a DEL character.
			WriteByte(0x7f);

			return;
		}

		protected void SkipClosingMark(char mark)
		{
			if(CPeekChar() != mark)
			{
				RaiseError("Warning: Function call does not end with correct closing mark: " + mark);
			}
			// This is just a nag, proceed whether we have the colon or not.
			curColumn++;
		}


		// Decompiling.
		public bool SetDecompileDisk(string diskFile, int diskNum)
		{
			try
			{
				decompileInput = File.OpenRead(diskFile);
				decompileDirectory = Path.GetDirectoryName(diskFile);
				curDisk = diskNum;
			}
			catch(DirectoryNotFoundException ex)
			{
				RaiseError("Source directory not found.", ex);
				return false;
			}
			catch(FileNotFoundException ex)
			{
				RaiseError("ADISK.DAT not found in source directory.", ex);
				return false;
			}

			linkSector = decompileInput.ReadByte();
			linkSector |= decompileInput.ReadByte() << 8;
			dataSector = decompileInput.ReadByte();
			dataSector |= decompileInput.ReadByte() << 8;

			deprecated_newStyleTag = false;

			return true;
		}

		public void CloseDecompileDisk()
		{
			if(decompileInput != null)
				decompileInput.Close();
		}

		protected byte DGetByte()
		{
			if(scenarioAddress > scenarioSize - 1)
			{
				decompileMode = DecompileModeType.SuddenEOF;
				return 0;
			}
			return scenarioData[scenarioAddress++];
		}

		protected char DGetChar()
		{
			return Convert.ToChar(DGetByte());
		}

		protected UInt16 DGetWord()
		{
			if(scenarioAddress > scenarioSize - 2)
			{
				decompileMode = DecompileModeType.SuddenEOF;

				if(scenarioAddress < scenarioSize - 1)
				{
					return Convert.ToUInt16(scenarioData[scenarioAddress]);
				}
				return 0;
			}
			UInt16 w = Convert.ToUInt16(scenarioData[scenarioAddress] | scenarioData[scenarioAddress + 1] << 8);
			scenarioAddress += 2;
			return w;
		}

		protected char DGetAndWriteChar()
		{
			try
			{
				char c = Convert.ToChar(scenarioData[scenarioAddress]);
				scenarioAddress++;

				WriteByte(c);

				return c;
			}
			catch(IndexOutOfRangeException ex)
			{
				if(!junkCodeMode)
				{
					RaiseError("DGetAndWriteChar called out of index, scenario address out of scope.", ex);
				}
				decompileMode = DecompileModeType.SuddenEOF;
				return '\0';
			}
		}

		protected char DPeekChar()
		{
			if(scenarioAddress == scenarioData.Length)
			{
				decompileMode = DecompileModeType.SuddenEOF;
				return '\0';
			}
			return Convert.ToChar(scenarioData[scenarioAddress]);
		}
		protected UInt16 DPeekWord()
		{
			if(scenarioAddress + 1 >= scenarioData.Length)
			{
				decompileMode = DecompileModeType.SuddenEOF;
				return '\0';
			}
			return Convert.ToUInt16(scenarioData[scenarioAddress] | scenarioData[scenarioAddress + 1] << 8);
		}

		protected void DWriteNewStyleMessage(char delimeter, bool overrideNewline = false)
		{
			bool escapeNext = false;
			char nextChar;

			while(true)
			{
				nextChar = DGetChar();

				if(!escapeNext && nextChar == delimeter)
				{
					break;
				}
				else if(decompileMode == DecompileModeType.SuddenEOF)
				{
					break;
				}

				ProcessMessageChar(nextChar, overrideNewline);

				if(escapeNext)
				{
					// Preserving this line in case we use this function as a copy-paste for processing.
					// if(curLine[i] == '\\' || curLine[i] == '\'' || curLine[i] == '"')
					escapeNext = false;
				}
				else if(nextChar == '\\') { 
					escapeNext = true;
				}
			}
		}

		protected void DGetAndWriteTextParam(char delimeter)
		{
			if(decompileMode == DecompileModeType.SuddenEOF) return;

			byte d = DGetByte();

			if(d == '\'' || d == '"')
			{  // SysEng
				char c;
				while((c = DGetChar()) != delimeter && decompileMode != DecompileModeType.SuddenEOF)
				{
					if(c == '\\')
						c = DGetChar();
					ProcessMessageChar(c, true);
				}
			}
			else
			{
				// Old-style text params do not necessarily use quotes. For example, M commands are in the format 
				// "M [newString]:" with the colon character demarking the end of the string. As a consequence,
				// the string cannot include a colon.
				//
				// Another unusual thing about text params in old style is that they don't run through the same
				// checks as message params (ProcessMessageChar()), and so CAN include Latin characters (such as for 
				// use with data file changes) and CAN'T include Gaiji. The only processing involved is whether or
				// not the the character is two-byte.
				while(d != delimeter)
				{
					if(DecompileMode == DecompileModeType.ProcessCode)
					{
						if((0x81 <= d && d <= 0x9f) || 0xe0 <= d)
						{
							// 2bytes
							WriteByte(d);
							WriteByte(DGetByte());
						}
						else
						{
							WriteByte(d);
						}
					}

					d = DGetByte();
				}
			}
		}

		public int CharToTextMode(char c)
		{
			return CharToTextMode(Convert.ToInt32(c));
		}

		public int CharToTextMode(byte c)
		{
			return CharToTextMode(c);
		}

		public int CharToTextMode(int code)
		{
			if(parent.CurTextMode == DecompilerForm.TextMode.Raw) return Convert.ToChar(code);

			int res;

			// If we've gotten this far, we've already discounted rdoRaw, so it can only be hankaku or zankaku.
			if(parent.CurTextMode == DecompilerForm.TextMode.Katakana)
			{
				switch(code)
				{
					case 0x8140: res = 0x20; break; // '　'
					case 0x8149: res = 0x21; break; // '！'
					case 0x8168: res = 0x22; break; // '”'
					case 0x8194: res = 0x23; break; // '＃'
					case 0x8190: res = 0x24; break; // '＄'
					case 0x8193: res = 0x25; break; // '％'
					case 0x8195: res = 0x26; break; // '＆'
					case 0x8166: res = 0x27; break; // '’'
					case 0x8169: res = 0x28; break; // '（'
					case 0x816a: res = 0x29; break; // '）'
					case 0x8196: res = 0x2a; break; // '＊'
					case 0x817b: res = 0x2b; break; // '＋'
					case 0x8143: res = 0x2c; break; // '，'
					case 0x817c: res = 0x2d; break; // '－'
					case 0x8144: res = 0x2e; break; // '．'
					case 0x815e: res = 0x2f; break; // '／'
					case 0x824f: res = 0x30; break; // '０'
					case 0x8250: res = 0x31; break; // '１'
					case 0x8251: res = 0x32; break; // '２'
					case 0x8252: res = 0x33; break; // '３'
					case 0x8253: res = 0x34; break; // '４'
					case 0x8254: res = 0x35; break; // '５'
					case 0x8255: res = 0x36; break; // '６'
					case 0x8256: res = 0x37; break; // '７'
					case 0x8257: res = 0x38; break; // '８'
					case 0x8258: res = 0x39; break; // '９'
					case 0x8146: res = 0x3a; break; // '：'
					case 0x8147: res = 0x3b; break; // '；'
					case 0x8183: res = 0x3c; break; // '＜'
					case 0x8181: res = 0x3d; break; // '＝'
					case 0x8184: res = 0x3e; break; // '＞'
					case 0x8148: res = 0x3f; break; // '？'
					case 0x8197: res = 0x40; break; // '＠'
					case 0x8260: res = 0x41; break; // 'Ａ'
					case 0x8261: res = 0x42; break; // 'Ｂ'
					case 0x8262: res = 0x43; break; // 'Ｃ'
					case 0x8263: res = 0x44; break; // 'Ｄ'
					case 0x8264: res = 0x45; break; // 'Ｅ'
					case 0x8265: res = 0x46; break; // 'Ｆ'
					case 0x8266: res = 0x47; break; // 'Ｇ'
					case 0x8267: res = 0x48; break; // 'Ｈ'
					case 0x8268: res = 0x49; break; // 'Ｉ'
					case 0x8269: res = 0x4a; break; // 'Ｊ'
					case 0x826a: res = 0x4b; break; // 'Ｋ'
					case 0x826b: res = 0x4c; break; // 'Ｌ'
					case 0x826c: res = 0x4d; break; // 'Ｍ'
					case 0x826d: res = 0x4e; break; // 'Ｎ'
					case 0x826e: res = 0x4f; break; // 'Ｏ'
					case 0x826f: res = 0x50; break; // 'Ｐ'
					case 0x8270: res = 0x51; break; // 'Ｑ'
					case 0x8271: res = 0x52; break; // 'Ｒ'
					case 0x8272: res = 0x53; break; // 'Ｓ'
					case 0x8273: res = 0x54; break; // 'Ｔ'
					case 0x8274: res = 0x55; break; // 'Ｕ'
					case 0x8275: res = 0x56; break; // 'Ｖ'
					case 0x8276: res = 0x57; break; // 'Ｗ'
					case 0x8277: res = 0x58; break; // 'Ｘ'
					case 0x8278: res = 0x59; break; // 'Ｙ'
					case 0x8279: res = 0x5a; break; // 'Ｚ'
					case 0x816d: res = 0x5b; break; // '［'
					case 0x818f: res = 0x5c; break; // '￥'
					case 0x816e: res = 0x5d; break; // '］'
					case 0x814f: res = 0x5e; break; // '＾'
					case 0x8151: res = 0x5f; break; // '＿'
					case 0x8165: res = 0x60; break; // '‘'
					case 0x8281: res = 0x61; break; // 'ａ'
					case 0x8282: res = 0x62; break; // 'ｂ'
					case 0x8283: res = 0x63; break; // 'ｃ'
					case 0x8284: res = 0x64; break; // 'ｄ'
					case 0x8285: res = 0x65; break; // 'ｅ'
					case 0x8286: res = 0x66; break; // 'ｆ'
					case 0x8287: res = 0x67; break; // 'ｇ'
					case 0x8288: res = 0x68; break; // 'ｈ'
					case 0x8289: res = 0x69; break; // 'ｉ'
					case 0x828a: res = 0x6a; break; // 'ｊ'
					case 0x828b: res = 0x6b; break; // 'ｋ'
					case 0x828c: res = 0x6c; break; // 'ｌ'
					case 0x828d: res = 0x6d; break; // 'ｍ'
					case 0x828e: res = 0x6e; break; // 'ｎ'
					case 0x828f: res = 0x6f; break; // 'ｏ'
					case 0x8290: res = 0x70; break; // 'ｐ'
					case 0x8291: res = 0x71; break; // 'ｑ'
					case 0x8292: res = 0x72; break; // 'ｒ'
					case 0x8293: res = 0x73; break; // 'ｓ'
					case 0x8294: res = 0x74; break; // 'ｔ'
					case 0x8295: res = 0x75; break; // 'ｕ'
					case 0x8296: res = 0x76; break; // 'ｖ'
					case 0x8297: res = 0x77; break; // 'ｗ'
					case 0x8298: res = 0x78; break; // 'ｘ'
					case 0x8299: res = 0x79; break; // 'ｙ'
					case 0x829a: res = 0x7a; break; // 'ｚ'
					case 0x816f: res = 0x7b; break; // '｛'
					case 0x8162: res = 0x7c; break; // '｜'
					case 0x8170: res = 0x7d; break; // '｝'
					case 0x8160: res = 0x7e; break; // '～'
					case 0x8142: res = 0xa1; break; // '。'
					case 0x8175: res = 0xa2; break; // '「'
					case 0x8176: res = 0xa3; break; // '」'
					case 0x8141: res = 0xa4; break; // '、'
					case 0x8145: res = 0xa5; break; // '・'
					case 0x82f0: res = 0xa6; break; // 'を'
					case 0x829f: res = 0xa7; break; // 'ぁ'
					case 0x82a1: res = 0xa8; break; // 'ぃ'
					case 0x82a3: res = 0xa9; break; // 'ぅ'
					case 0x82a5: res = 0xaa; break; // 'ぇ'
					case 0x82a7: res = 0xab; break; // 'ぉ'
					case 0x82e1: res = 0xac; break; // 'ゃ'
					case 0x82e3: res = 0xad; break; // 'ゅ'
					case 0x82e5: res = 0xae; break; // 'ょ'
					case 0x82c1: res = 0xaf; break; // 'っ'
					case 0x815b: res = 0xb0; break; // 'ー'
					case 0x82a0: res = 0xb1; break; // 'あ'
					case 0x82a2: res = 0xb2; break; // 'い'
					case 0x82a4: res = 0xb3; break; // 'う'
					case 0x82a6: res = 0xb4; break; // 'え'
					case 0x82a8: res = 0xb5; break; // 'お'
					case 0x82a9: res = 0xb6; break; // 'か'
					case 0x82ab: res = 0xb7; break; // 'き'
					case 0x82ad: res = 0xb8; break; // 'く'
					case 0x82af: res = 0xb9; break; // 'け'
					case 0x82b1: res = 0xba; break; // 'こ'
					case 0x82b3: res = 0xbb; break; // 'さ'
					case 0x82b5: res = 0xbc; break; // 'し'
					case 0x82b7: res = 0xbd; break; // 'す'
					case 0x82b9: res = 0xbe; break; // 'せ'
					case 0x82bb: res = 0xbf; break; // 'そ'
					case 0x82bd: res = 0xc0; break; // 'た'
					case 0x82bf: res = 0xc1; break; // 'ち'
					case 0x82c2: res = 0xc2; break; // 'つ'
					case 0x82c4: res = 0xc3; break; // 'て'
					case 0x82c6: res = 0xc4; break; // 'と'
					case 0x82c8: res = 0xc5; break; // 'な'
					case 0x82c9: res = 0xc6; break; // 'に'
					case 0x82ca: res = 0xc7; break; // 'ぬ'
					case 0x82cb: res = 0xc8; break; // 'ね'
					case 0x82cc: res = 0xc9; break; // 'の'
					case 0x82cd: res = 0xca; break; // 'は'
					case 0x82d0: res = 0xcb; break; // 'ひ'
					case 0x82d3: res = 0xcc; break; // 'ふ'
					case 0x82d6: res = 0xcd; break; // 'へ'
					case 0x82d9: res = 0xce; break; // 'ほ'
					case 0x82dc: res = 0xcf; break; // 'ま'
					case 0x82dd: res = 0xd0; break; // 'み'
					case 0x82de: res = 0xd1; break; // 'む'
					case 0x82df: res = 0xd2; break; // 'め'
					case 0x82e0: res = 0xd3; break; // 'も'
					case 0x82e2: res = 0xd4; break; // 'や'
					case 0x82e4: res = 0xd5; break; // 'ゆ'
					case 0x82e6: res = 0xd6; break; // 'よ'
					case 0x82e7: res = 0xd7; break; // 'ら'
					case 0x82e8: res = 0xd8; break; // 'り'
					case 0x82e9: res = 0xd9; break; // 'る'
					case 0x82ea: res = 0xda; break; // 'れ'
					case 0x82eb: res = 0xdb; break; // 'ろ'
					case 0x82ed: res = 0xdc; break; // 'わ'
					case 0x82f1: res = 0xdd; break; // 'ん'
					case 0x814a: res = 0xde; break; // '゛'
					case 0x814b: res = 0xdf; break; // '゜'
					default: res = code; break;
				}
			}
			else
			{
				switch(code)
				{
					case 0x20: res = 0x8140; break; // '　'
					case 0x21: res = 0x8149; break; // '！'
					case 0x22: res = 0x8168; break; // '”'
					case 0x23: res = 0x8194; break; // '＃'
					case 0x24: res = 0x8190; break; // '＄'
					case 0x25: res = 0x8193; break; // '％'
					case 0x26: res = 0x8195; break; // '＆'
					case 0x27: res = 0x8166; break; // '’'
					case 0x28: res = 0x8169; break; // '（'
					case 0x29: res = 0x816a; break; // '）'
					case 0x2a: res = 0x8196; break; // '＊'
					case 0x2b: res = 0x817b; break; // '＋'
					case 0x2c: res = 0x8143; break; // '，'
					case 0x2d: res = 0x817c; break; // '－'
					case 0x2e: res = 0x8144; break; // '．'
					case 0x2f: res = 0x815e; break; // '／'
					case 0x30: res = 0x824f; break; // '０'
					case 0x31: res = 0x8250; break; // '１'
					case 0x32: res = 0x8251; break; // '２'
					case 0x33: res = 0x8252; break; // '３'
					case 0x34: res = 0x8253; break; // '４'
					case 0x35: res = 0x8254; break; // '５'
					case 0x36: res = 0x8255; break; // '６'
					case 0x37: res = 0x8256; break; // '７'
					case 0x38: res = 0x8257; break; // '８'
					case 0x39: res = 0x8258; break; // '９'
					case 0x3a: res = 0x8146; break; // '：'
					case 0x3b: res = 0x8147; break; // '；'
					case 0x3c: res = 0x8183; break; // '＜'
					case 0x3d: res = 0x8181; break; // '＝'
					case 0x3e: res = 0x8184; break; // '＞'
					case 0x3f: res = 0x8148; break; // '？'
					case 0x40: res = 0x8197; break; // '＠'
					case 0x41: res = 0x8260; break; // 'Ａ'
					case 0x42: res = 0x8261; break; // 'Ｂ'
					case 0x43: res = 0x8262; break; // 'Ｃ'
					case 0x44: res = 0x8263; break; // 'Ｄ'
					case 0x45: res = 0x8264; break; // 'Ｅ'
					case 0x46: res = 0x8265; break; // 'Ｆ'
					case 0x47: res = 0x8266; break; // 'Ｇ'
					case 0x48: res = 0x8267; break; // 'Ｈ'
					case 0x49: res = 0x8268; break; // 'Ｉ'
					case 0x4a: res = 0x8269; break; // 'Ｊ'
					case 0x4b: res = 0x826a; break; // 'Ｋ'
					case 0x4c: res = 0x826b; break; // 'Ｌ'
					case 0x4d: res = 0x826c; break; // 'Ｍ'
					case 0x4e: res = 0x826d; break; // 'Ｎ'
					case 0x4f: res = 0x826e; break; // 'Ｏ'
					case 0x50: res = 0x826f; break; // 'Ｐ'
					case 0x51: res = 0x8270; break; // 'Ｑ'
					case 0x52: res = 0x8271; break; // 'Ｒ'
					case 0x53: res = 0x8272; break; // 'Ｓ'
					case 0x54: res = 0x8273; break; // 'Ｔ'
					case 0x55: res = 0x8274; break; // 'Ｕ'
					case 0x56: res = 0x8275; break; // 'Ｖ'
					case 0x57: res = 0x8276; break; // 'Ｗ'
					case 0x58: res = 0x8277; break; // 'Ｘ'
					case 0x59: res = 0x8278; break; // 'Ｙ'
					case 0x5a: res = 0x8279; break; // 'Ｚ'
					case 0x5b: res = 0x816d; break; // '［'
					case 0x5c: res = 0x818f; break; // '￥'
					case 0x5d: res = 0x816e; break; // '］'
					case 0x5e: res = 0x814f; break; // '＾'
					case 0x5f: res = 0x8151; break; // '＿'
					case 0x60: res = 0x8165; break; // '‘'
					case 0x61: res = 0x8281; break; // 'ａ'
					case 0x62: res = 0x8282; break; // 'ｂ'
					case 0x63: res = 0x8283; break; // 'ｃ'
					case 0x64: res = 0x8284; break; // 'ｄ'
					case 0x65: res = 0x8285; break; // 'ｅ'
					case 0x66: res = 0x8286; break; // 'ｆ'
					case 0x67: res = 0x8287; break; // 'ｇ'
					case 0x68: res = 0x8288; break; // 'ｈ'
					case 0x69: res = 0x8289; break; // 'ｉ'
					case 0x6a: res = 0x828a; break; // 'ｊ'
					case 0x6b: res = 0x828b; break; // 'ｋ'
					case 0x6c: res = 0x828c; break; // 'ｌ'
					case 0x6d: res = 0x828d; break; // 'ｍ'
					case 0x6e: res = 0x828e; break; // 'ｎ'
					case 0x6f: res = 0x828f; break; // 'ｏ'
					case 0x70: res = 0x8290; break; // 'ｐ'
					case 0x71: res = 0x8291; break; // 'ｑ'
					case 0x72: res = 0x8292; break; // 'ｒ'
					case 0x73: res = 0x8293; break; // 'ｓ'
					case 0x74: res = 0x8294; break; // 'ｔ'
					case 0x75: res = 0x8295; break; // 'ｕ'
					case 0x76: res = 0x8296; break; // 'ｖ'
					case 0x77: res = 0x8297; break; // 'ｗ'
					case 0x78: res = 0x8298; break; // 'ｘ'
					case 0x79: res = 0x8299; break; // 'ｙ'
					case 0x7a: res = 0x829a; break; // 'ｚ'
					case 0x7b: res = 0x816f; break; // '｛'
					case 0x7c: res = 0x8162; break; // '｜'
					case 0x7d: res = 0x8170; break; // '｝'
					case 0x7e: res = 0x8160; break; // '～'
					case 0xa1: res = 0x8142; break; // '。'
					case 0xa2: res = 0x8175; break; // '「'
					case 0xa3: res = 0x8176; break; // '」'
					case 0xa4: res = 0x8141; break; // '、'
					case 0xa5: res = 0x8145; break; // '・'
					case 0xa6: res = 0x82f0; break; // 'を'
					case 0xa7: res = 0x829f; break; // 'ぁ'
					case 0xa8: res = 0x82a1; break; // 'ぃ'
					case 0xa9: res = 0x82a3; break; // 'ぅ'
					case 0xaa: res = 0x82a5; break; // 'ぇ'
					case 0xab: res = 0x82a7; break; // 'ぉ'
					case 0xac: res = 0x82e1; break; // 'ゃ'
					case 0xad: res = 0x82e3; break; // 'ゅ'
					case 0xae: res = 0x82e5; break; // 'ょ'
					case 0xaf: res = 0x82c1; break; // 'っ'
					case 0xb0: res = 0x815b; break; // 'ー'
					case 0xb1: res = 0x82a0; break; // 'あ'
					case 0xb2: res = 0x82a2; break; // 'い'
					case 0xb3: res = 0x82a4; break; // 'う'
					case 0xb4: res = 0x82a6; break; // 'え'
					case 0xb5: res = 0x82a8; break; // 'お'
					case 0xb6: res = 0x82a9; break; // 'か'
					case 0xb7: res = 0x82ab; break; // 'き'
					case 0xb8: res = 0x82ad; break; // 'く'
					case 0xb9: res = 0x82af; break; // 'け'
					case 0xba: res = 0x82b1; break; // 'こ'
					case 0xbb: res = 0x82b3; break; // 'さ'
					case 0xbc: res = 0x82b5; break; // 'し'
					case 0xbd: res = 0x82b7; break; // 'す'
					case 0xbe: res = 0x82b9; break; // 'せ'
					case 0xbf: res = 0x82bb; break; // 'そ'
					case 0xc0: res = 0x82bd; break; // 'た'
					case 0xc1: res = 0x82bf; break; // 'ち'
					case 0xc2: res = 0x82c2; break; // 'つ'
					case 0xc3: res = 0x82c4; break; // 'て'
					case 0xc4: res = 0x82c6; break; // 'と'
					case 0xc5: res = 0x82c8; break; // 'な'
					case 0xc6: res = 0x82c9; break; // 'に'
					case 0xc7: res = 0x82ca; break; // 'ぬ'
					case 0xc8: res = 0x82cb; break; // 'ね'
					case 0xc9: res = 0x82cc; break; // 'の'
					case 0xca: res = 0x82cd; break; // 'は'
					case 0xcb: res = 0x82d0; break; // 'ひ'
					case 0xcc: res = 0x82d3; break; // 'ふ'
					case 0xcd: res = 0x82d6; break; // 'へ'
					case 0xce: res = 0x82d9; break; // 'ほ'
					case 0xcf: res = 0x82dc; break; // 'ま'
					case 0xd0: res = 0x82dd; break; // 'み'
					case 0xd1: res = 0x82de; break; // 'む'
					case 0xd2: res = 0x82df; break; // 'め'
					case 0xd3: res = 0x82e0; break; // 'も'
					case 0xd4: res = 0x82e2; break; // 'や'
					case 0xd5: res = 0x82e4; break; // 'ゆ'
					case 0xd6: res = 0x82e6; break; // 'よ'
					case 0xd7: res = 0x82e7; break; // 'ら'
					case 0xd8: res = 0x82e8; break; // 'り'
					case 0xd9: res = 0x82e9; break; // 'る'
					case 0xda: res = 0x82ea; break; // 'れ'
					case 0xdb: res = 0x82eb; break; // 'ろ'
					case 0xdc: res = 0x82ed; break; // 'わ'
					case 0xdd: res = 0x82f1; break; // 'ん'
					case 0xde: res = 0x814a; break; // '゛'
					case 0xdf: res = 0x814b; break; // '゜'
					default: res = code; break;
				}
			}

			return res;
		}

		public bool ProcessMessageChar(char c, bool overrideNewline=false)
		{
			if(decompileMode == DecompileModeType.SuddenEOF) return false;

			// Shift JIS handling.
			if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.ShiftJIS)
			{
				// Valid single-byte character.
				if(c <= 0x80 || (0xa1 <= c && c <= 0xdd))
				{
					if(activeTextOutput == ' ' && decompileMode != DecompileModeType.AG00 && !overrideNewline)
					{
						StartTextLine();
					}

					if(decompileMode == DecompileModeType.ProcessCode || decompileMode == DecompileModeType.AG00)
					{
						// If we are dealing with ShiftJIS characters outside of the ASCII character space,
						// convert them to the current text mode.
						int code;

						// Leave ASCII text as ASCII text.
						if(c <= 0x80)
						{
							code = Convert.ToInt32(c);
						}
						// Convert the rest of it depending on whether we're in zenkaku or hankaku mode.
						else
						{
							code = CharToTextMode(c);
						}
						if(code > 255)
						{
							WriteByte(code >> 8);
							WriteByte(code & 0xff);
						}
						else
						{
							WriteByte(code);
						}
					}
				}
				// Valid two-byte character.
				else if((0x81 <= c && c <= 0x9f) || 0xe0 <= c)
				{
					if(activeTextOutput == ' ' && decompileMode != DecompileModeType.AG00 && !overrideNewline)
					{
						StartTextLine();
					}

					// message (2 bytes)
					if(decompileMode == DecompileModeType.ProcessCode || decompileMode == DecompileModeType.AG00)
					{
						//char twoByte = shiftJIS.GetChars(new byte [] { Convert.ToByte(c), DGetByte() })[0];
						int twoByte;

						if(decompileMode == DecompileModeType.ProcessCode) twoByte = (c << 8) + DGetByte();
						else
						{
							twoByte = (c << 8) + parent.FileBytes[parent.FileIndex++];
						}

						int code = CharToTextMode(twoByte);

						// Gaiji. Gaiji are externally defined characters (defined in GAIJI.DAT) that no longer exist 
						// as a part of the Shift-JIS standard. We have to output their codes differently or else text 
						// editors will replace them with "best guesses." For this reason, we'll output the number as
						// "G+####" instead of the individual bytes.
						// 
						// We used to use 0x instead of G+. The compiler still recognizes the old form but the
						// decompiler will no longer produce it.
						if((0xeb9f <= code && code <= 0xebfc) || (0xec40 <= code && code <= 0xec9e))
						{
							WriteText("G+" + code.ToString("X4"));
						}

						// Regular two-byte characters. Just output them as-is.
						else
						{
							if(code > 255)
							{
								WriteByte(code >> 8);
								WriteByte(code & 0xff);
							}
							else
							{
								WriteByte(code);
							}
						}
						//byte[] bytes = shiftJIS.GetBytes(new char[] { parent.CharToTextMode(twoByte) } );
						//foreach(byte b in bytes) WriteByte(b);
					}
					else
					{
						scenarioAddress++;
					}
				}
				else
				{
					if(decompileMode == DecompileModeType.AG00)
					{
						WriteByte(c);
					}
					else
					{
						RaiseError("Unknown text output: \"" + c + "\" at page " + curPage + " addr " +
							scenarioAddress + "." + Environment.NewLine);
						return false;
					}
				}
			}
			else if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.UTF8)
			{
				// TODO
			}
			else if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.MSX)
			{
				// All MSX/PC88 characters are single-byte, which simplifies things immensely.
				if(activeTextOutput == ' ' && decompileMode != DecompileModeType.AG00 && !overrideNewline)
				{
					StartTextLine();
				}

				if(decompileMode == DecompileModeType.AG00 || decompileMode == DecompileModeType.ProcessCode)
				{
					return processMSXEncoding(c, overrideNewline);
				}
				
			}
			return true;
		}

		public bool IsKana (char c)
		{
			if(c == 0x27 || c == 0x20 || (0xa1 <= c && c <= 0xdd) || (0x81 <= c && c <= 0x9f) || 0xe0 <= c) { 
				return true;
			}
			return false;
		}

		private bool LoadScenario(int pageNum)
		{
			if(pageNum > (dataSector - linkSector) * 128 - 1)
			{
				RaiseError("Page number (" + pageNum + ") exceeds expected page count. There may be excess files " +
					"or errors in the ALD file's header.");
				return false;
			}

			decompileInput.Position = ((linkSector - 1) * 256 + (pageNum - 1) * 2);
			int diskIndex = decompileInput.ReadByte();
			int linkIndex = decompileInput.ReadByte();

			if(diskIndex == 0 || diskIndex == 0x1a)
			{
				return false;
			}

			// A??.DAT以外にリンクされている場合はファイルを開き直す
			if(diskIndex != curDisk)
			{

				string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				string diskFile = alphabet[diskIndex - 1] + "DISK.DAT";

				try
				{
					decompileInput = File.OpenRead(Path.Combine(decompileDirectory, diskFile));
					curDisk = diskIndex;
				}

				catch(DirectoryNotFoundException ex)
				{
					RaiseError("Source directory not found.", ex);
				}
				catch(FileNotFoundException ex)
				{
					RaiseError(diskFile + " not found in source directory.", ex);
				}
			}

			// データ取得
			decompileInput.Position = linkIndex * 2;
			int startSector = decompileInput.ReadByte();
			startSector |= decompileInput.ReadByte() << 8;
			int endSector = decompileInput.ReadByte();
			endSector |= decompileInput.ReadByte() << 8;

			scenarioSize = (endSector - startSector) * 256;

			if(scenarioSize < 0)
			{
				RaiseError("Scenario Size for page " + pageNum + " is negative. This may indicate an error in " +
					"the header, but also seems to occur when duplicate files are present across multiple disks. " +
					"Consider creating a combined Disk file by exporting SCO files using ALD Explorer.");

				return false;
			}
			if(startSector <= 0)
			{
				RaiseError("Start Sector for page " + pageNum + " is 0 or negative. This may indicate an error " +
					"in the header. Consider creating a combined Disk file by exporting SCO files using ALD " +
					"Explorer.");
			}

			scenarioData = new byte[scenarioSize];
			decompileInput.Position = (startSector - 1) * 256;
			decompileInput.Read(scenarioData, 0, scenarioSize);

			return true;
		}

		public void WriteElapsed(string msg, TimeSpan ts)
		{
			//string elapsedTime = String.Format(/*"{0:00}:{1:00}:{2:00}.{3.00}"*/ "{0:00}.{1:00}",
			//	/*ts.Hours, ts.Minutes,*/ ts.Seconds, ts.Milliseconds / 10);

			//if(ts.Milliseconds > 0)
			//{
				int tabCount = (30 - msg.Length) / 4;
				Console.WriteLine(msg + ":" + new string('\t', tabCount) + ts.TotalMilliseconds);
			//}
		}

		public bool DecompilePage(string outputDirectory, int pageNum)
		{
			curPage = pageNum;
			junkCodeMode = false;
			firstEOF = -1;
			dummiedCommandWarnings.Clear();

			/*Stopwatch sw = new Stopwatch();
			sw.Start();*/

			bool scenarioLoaded = LoadScenario(pageNum + 1);

			/*sw.Stop();
			WriteElapsed("Page " + curPage + ", LoadScenario", sw.Elapsed);
			sw.Reset();*/

			if(!scenarioLoaded) return false;


			// Check the first SCO file (dis0001.sco, aka page 0) for the tag "REV," which is added 
			// to the start of localization-friendly code created by Sys0Decompiler's compile process.
			// The code must be played and decompiled differently than the original.
			if(pageNum == 0 && scenarioSize > 4)
			{
				if(scenarioData[2] == 'R' && scenarioData[3] == 'E' && scenarioData[4] == 'V')
				{
					deprecated_newStyleTag = true;
				}
			}

			string filePath = Path.Combine(outputDirectory, "page" + pageNum.ToString("0000") + ".adv.tmp");
			outputStream = File.Create(filePath);

			// Travel through the file, cataloguing labels and branch ends (Sys2 and 3 only).
			labelAddresses.Clear();
			branchEndAddresses.Clear();
			setMenuRecords.Clear();

			parent.OnReportProgress(0, "Decompiling Page " + curPage + ", first pass...");

			//sw.Start();

			decompileMode = DecompileModeType.FindLabels;
			bool success = DecompileLoop(curPage);

			/*sw.Stop();
			WriteElapsed("Page " + curPage + ", FIND_LABELS total", sw.Elapsed);
			sw.Reset();*/

			if(!success) {
				outputStream.Close();
				File.Delete(filePath);
				return false;
			}

			// Sort our collection of addresses and remove duplicates.
			labelAddresses.Sort();
			labelAddresses = labelAddresses.Distinct().ToList();

			// The same for branch ends. Do NOT do distinct, as it is possible for multiple branch ends to
			// be at the same address, representing multiple end points in a row.
			branchEndAddresses.Sort();
			branchEndAddresses = branchEndAddresses.ToList();

			// Output warnings if there are any labels or branch ends supposedly outside of EOF. We cannot do this
			// on MSX due to ambiguity with the EOF character, which means there is unfortunately no way to
			// warn the user about potential problems. The manual should address this.
			if(parent.CurDecompileSourceMode != DecompilerForm.SourceEncodingMode.MSX)
			{
				if(firstEOF < 0)
				{
					RaiseError("EOF character not found in data.");
				}
				else
				{
					foreach(int labelAddress in labelAddresses)
					{
						if(labelAddress > firstEOF)
						{
							RaiseError("Label jump or call to address beyond first EOF character in page " + pageNum +
								". Referenced address: " + labelAddress + ". Code may exist beyond EOF. Consider enabling" +
								"Advanced Settings -> Output Junk Code.");
						}
					}
					foreach(int branchEndAddress in branchEndAddresses)
					{
						if(branchEndAddress > firstEOF)
						{
							RaiseError("Branch end beyond first EOF character in page " + pageNum + ".Referenced " +
								"address: " + branchEndAddress + ". Code may exist beyond EOF. Consider enabling" +
								"Advanced Settings -> Output Junk Code.");
						}
					}
				}
			}

			parent.OnReportProgress(0, "Decompiling Page " + curPage + ", second pass...");

			//sw.Start();

			decompileMode = DecompileModeType.ProcessCode;
			success = DecompileLoop(curPage);

			/*sw.Stop();
			WriteElapsed("Page " + curPage + ", DecompileMode.ProcessCode total", sw.Elapsed);
			sw.Reset();*/

			outputStream.Close();
			if(success)
			{
				// See if the original file exists. If it does, delete it.
				string origFile = filePath.Substring(0, filePath.Length - 4);
				if(File.Exists(origFile)) File.Delete(origFile);

				// Rename the temp file.
				File.Move(filePath, origFile);

				if(dummiedCommandWarnings.Count > 0)
				{
					dummiedCommandWarnings.Sort();

					string warningText = "Warning: Dummied commands (";

					foreach(string cmd in dummiedCommandWarnings)
					{
						warningText += cmd + ", ";
					}
					// Crop the final comma.
					warningText = warningText.Substring(0, warningText.Length - 2);

					warningText += ") found in page " + pageNum + ". These commands are not yet supported by " +
						"SysEng. Modification of the SysEnd source may be required to run this particular game.";

					RaiseWarning(warningText);
				}
			}
			else
			{
				File.Delete(filePath);
			}

			return success;
		}

		// The main loop for decompiling a page, processed twice, first in FIND_LABELS mode and then in the actual
		// decompilation via PROCESS_PAGE.
		//
		// We don't actually use curPage here, but it's helpful for breakpoints.
		public bool DecompileLoop(int curPage) {
			bool endOfFile = false;
			string msxEOFBuffer = "";
			int curLabelAddress = 0;
			int curBranchEndAddress = 0;

			junkCodeMode = false;
			activeSetMenu = false;
			activeTextOutput = ' ';
			nestingLevel = 0;
			lastMSXKana = ' ';

			//Stopwatch sw = new Stopwatch();

			scenarioAddress = 2;
			
			// Move past the REV tag on the first page if we're in new mode.
			if(deprecated_newStyleTag && curPage == 0) scenarioAddress = 5;

			fatalError = false;

			// The main decompile loop.  Travel thorugh the file, translating each line to text instead of
			// running them.
			while(scenarioAddress < scenarioSize &&	decompileMode != DecompileModeType.SuddenEOF &&
				(!endOfFile || junkCodeMode))
			{
				if(fatalError)
					return false;

				// アドレスの確認
				if(scenarioAddress >= scenarioSize)
				{
					RaiseError("Scenario addr incorrect.");
					return false;
				}

				//sw.Start();

				// Output branch ends. There should not be any labels and branch ends at the same address.
				// This process is exclusive to System 2 and 3.
				if(decompileMode == DecompileModeType.ProcessCode && curBranchEndAddress >= 0 && branchEndAddresses.Count > curBranchEndAddress)
				{
					// Unlike labels, multiple end points can be at the same address, representing two+ closed
					// blocks at once. So we use a loop instead of an if.
					while(scenarioAddress >= branchEndAddresses[curBranchEndAddress])
					{
						automaticBranchEnd();

						curBranchEndAddress++;
						if(curBranchEndAddress >= branchEndAddresses.Count)
						{
							curBranchEndAddress = -1;
							break;
						}
					}
				}

				// Output labels.
				if(decompileMode == DecompileModeType.ProcessCode && curLabelAddress >= 0 && labelAddresses.Count > curLabelAddress)
				{
					if(scenarioAddress >= labelAddresses[curLabelAddress])
					{
						StartLine();
						WriteText("*lbl" + labelAddresses[curLabelAddress].ToString("X") + ":" +
							Environment.NewLine);

						curLabelAddress++;
						if(curLabelAddress >= labelAddresses.Count) curLabelAddress = -1;
					}
				}

				/*sw.Stop();
				WriteElapsed("Output Labels", sw.Elapsed);
				sw.Reset();
				sw.Start();*/

				// １コマンド実行
				char cmd = DGetChar();

				// Because the MSX versions make ambiguous use of the EOF character, we can't output it or any
				// NUL characters that follow until we can confirm they're actually INSIDE the file. This happens
				// the moment we get any other command.
				if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.MSX && msxEOFBuffer.Length > 0)
				{
					if(cmd != '\0' && cmd != 0x1A)
					{
						foreach(char c in msxEOFBuffer)
						{
							ProcessMessageChar(c);
						}

						msxEOFBuffer = "";
					}
				}
					 
				switch(cmd)
				{
				case '!':
					decompile_cmd_calc();
					break;
				case '{':
					decompile_cmd_branch();
					break;
				case '}':
					decompile_cmd_branch_end();
					break;
				case '@':
					decompile_cmd_label_jump();
					break;
				case '\\':
					decompile_cmd_label_call();
					break;
				case '&':
					decompile_cmd_page_jump();
					break;
				case '%':
					decompile_cmd_page_call();
					break;
				case '$':
					decompile_cmd_set_menu();
					break;
				case '[':
					decompile_cmd_set_verbobj();
					break;
				case ':':
					decompile_cmd_set_verbobj2();
					break;
				case ']':
					decompile_cmd_open_menu();
					break;
				case 'A':
					decompile_cmd_a();
					break;
				case 'B':
					decompile_cmd_b();
					break;
				case 'D':
					decompile_cmd_d();
					break;
				case 'E':
					decompile_cmd_e();
					break;
				case 'F':
					decompile_cmd_f();
					break;
				case 'G':
					decompile_cmd_g();
					break;
				case 'H':
					decompile_cmd_h();
					break;
				case 'I':
					decompile_cmd_i();
					break;
				case 'J':
					decompile_cmd_j();
					break;
				case 'K':
					decompile_cmd_k();
					break;
				case 'L':
					decompile_cmd_l();
					break;
				case 'M':
					decompile_cmd_m();
					break;
				case 'N':
					decompile_cmd_n();
					break;
				case 'O':
					decompile_cmd_o();
					break;
				case 'P':
					decompile_cmd_p();
					break;
				case 'Q':
					decompile_cmd_q();
					break;
				case 'R':
					decompile_cmd_r();
					break;
				case 'S':
					decompile_cmd_s();
					break;
				case 'T':
					decompile_cmd_t();
					break;
				case 'U':
					decompile_cmd_u();
					break;
				case 'V':
					decompile_cmd_v();
					break;
				case 'W':
					decompile_cmd_w();
					break;
				case 'X':
					decompile_cmd_x();
					break;
				case 'Y':
					decompile_cmd_y();
					break;
				case 'Z':
					decompile_cmd_z();
					break;
				case '\'':
				case '"':
					DWriteNewStyleMessage(cmd);
					break;
				default:
					// Sometimes, you find "bubbles" of null characters in the gaps between functions due, I 
					// presume, to mis-assigned label jumps. So you might have valid code ending with a label 
					// jump, then a bubble, then the arriving side of another label jump. To keep everything 
					// properly aligned, we'll repeat null characters exactly as they appear, advising they be 
					// deleted.
					// 
					// In MSX mode, NUL characters represent message text spaces, so have to be output as well.
					if(cmd == '\0' /*|| cmd == 0x7F*/)
					{
						if(decompileMode == DecompileModeType.ProcessCode)
						{
							// In MSX mode, NUL characters are message text: specifically, spaces.
							if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.MSX)
							{
								// However, NUL characters should not be written if we have an active EOF Buffer.
								// This happens because of an ambiguity with the EOF character, see below for
								// more details. Instead, they should be appended to the buffer.
								if(msxEOFBuffer.Length > 0)
								{
									msxEOFBuffer += cmd;
								}
								else
								{
									bool res = ProcessMessageChar(cmd);
									if(!res)
										return false;
								}
							}
							else
							{
								WriteByte(cmd);
							}
						}
					}

					// End of File
					else if(cmd == 0x1A)
					{
						// On MSX, 0x1A is both the EOF character and the '[' message character depending on
						// context. In most situations, we should treat it as a '[', but if all the following
						// characters are NUL, we should drop them all. For this reason, unless Enable Junk
						// Code is active, we'll store the characters in a buffer and only output them if
						// anything else follows.
						if(parent.CurDecompileSourceMode == DecompilerForm.SourceEncodingMode.MSX &&
							!parent.OutputJunk())
						{
							msxEOFBuffer += cmd;
						}
						else
						{
							StartLine();

							endOfFile = true;

							if(firstEOF == -1) firstEOF = scenarioAddress;

							if(parent.OutputJunk())
							{
								junkCodeMode = true;

								if(decompileMode == DecompileModeType.ProcessCode)
									WriteByte(cmd);
							}
						}
					}

					else
					{
						// Old-style message, or error.
						bool res = ProcessMessageChar(cmd);
						if(!res)
							return false;
					}

					break;
				}

				/*sw.Stop();
				WriteElapsed("Command processing (" + cmd + ")", sw.Elapsed);
				sw.Reset();*/
			}

			return true;
		}

		protected void StartLine()
		{
			// End text output, if applicable.
			if(activeTextOutput != ' ')
			{
				activeTextOutput = ' ';
				lastMSXKana = ' ';

				if(decompileMode == DecompileModeType.ProcessCode)
					WriteText("'" + Environment.NewLine);
			}

			// Insert nesting.
			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText(new string(' ', nestingLevel * 4));
		}

		protected void StartTextLine()
		{
			activeTextOutput = '"';

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText(new string(' ', nestingLevel * 4) + "'");
			}
		}

		protected string varIndexToName(int index)
		{
			// The first 58 variables have proper names. Use them only if Full Label Set is checked.
			if(index <= 58  && parent.FullLabels())
			{
				switch(index)
				{
					case 0: return "RND";
					case 1: return "D01";
					case 2: return "D02";
					case 3: return "D03";
					case 4: return "D04";
					case 5: return "D05";
					case 6: return "D06";
					case 7: return "D07";
					case 8: return "D08";
					case 9: return "D09";
					case 10: return "D10";
					case 11: return "D11";
					case 12: return "D12";
					case 13: return "D13";
					case 14: return "D14";
					case 15: return "D15";
					case 16: return "D16";
					case 17: return "D17";
					case 18: return "D18";
					case 19: return "D19";
					case 20: return "D20";
					case 21: return "U01";
					case 22: return "U02";
					case 23: return "U03";
					case 24: return "U04";
					case 25: return "U05";
					case 26: return "U06";
					case 27: return "U07";
					case 28: return "U08";
					case 29: return "U09";
					case 30: return "U10";
					case 31: return "U11";
					case 32: return "U12";
					case 33: return "U13";
					case 34: return "U14";
					case 35: return "U15";
					case 36: return "U16";
					case 37: return "U17";
					case 38: return "U18";
					case 39: return "U19";
					case 40: return "U20";
					case 41: return "B01";
					case 42: return "B02";
					case 43: return "B03";
					case 44: return "B04";
					case 45: return "B05";
					case 46: return "B06";
					case 47: return "B07";
					case 48: return "B08";
					case 49: return "B09";
					case 50: return "B10";
					case 51: return "B11";
					case 52: return "B12";
					case 53: return "B13";
					case 54: return "B14";
					case 55: return "B15";
					case 56: return "B16";
					case 57: return "M_X";
					case 58: return "M_Y";
					default:
						RaiseError("Index to label error: " + index + ".");
						return "";
				}
			}

			// All remaining variables are dubbed "VAR" and then a number. All variables will use this format
			// on request.
			else
			{
				return "VAR" + index.ToString("0000");
			}
		}


		protected void RaiseError(string errorMessage)
		{
			RaiseError(errorMessage, new Exception());
		}

		protected void RaiseWarning(string errorMessage)
		{
			RaiseError(errorMessage, new Exception(), true);
		}

		protected void RaiseError(string errorMessage, Exception ex, bool isWarning = false)
		{
			//errorMessage += "()";

			string exMessage = ex.Message;
			string defaultExceptionText = ((Exception)Activator.CreateInstance(ex.GetType())).Message;

			if(exMessage == defaultExceptionText && ex.TargetSite != null)
			{
				string functionName = ex.TargetSite.Name;
				if(functionName == "err")
				{
					var stackTraceLines = ex.StackTrace.Split("\r\n");
					foreach(var line in stackTraceLines)
					{
						const string lookFor = "at ";
						int indexOfAt = line.IndexOf(lookFor);
						if(indexOfAt == -1)
						{
							continue;
						}
						int indexAfterAt = indexOfAt + lookFor.Length;

						int indexOfOpenParen = line.IndexOf('(', indexAfterAt);
						if(indexOfOpenParen == -1) indexOfOpenParen = line.Length;
						functionName = line.Substring(indexAfterAt, indexOfOpenParen - indexAfterAt);
						int lastDot = functionName.LastIndexOf('.');
						if(lastDot == -1) lastDot = -1;
						functionName = functionName.Substring(lastDot + 1);
						if(functionName == "err")
						{

						}
						else
						{
							break;
						}

					}

					//ex.StackTrace
				}

				exMessage = "Exception from " + functionName;
			}
			if(ex.GetType().FullName == "System.Exception")
				errorMessage = errorMessage + " (file \"" + curFile + ", line " + curLineNum + ", column " + curColumn +
				").";
			else
				errorMessage = errorMessage + " (file \"" + curFile + ", line " + curLineNum + ", column " + curColumn + 
				"): " + exMessage;
			parent.OnError(new ErrorEventArgs(new Exception(errorMessage, ex)));
		}

		protected void RaiseDummiedCommandWarning(string cmd)
		{
			if(!dummiedCommandWarnings.Contains(cmd))
			{
				dummiedCommandWarnings.Add(cmd);
			}
		}

		public DecompileModeType DecompileMode
		{
			get
			{
				return decompileMode;
			}
			set
			{
				decompileMode = value;
			}
		}



		private bool processMSXEncoding(char c, bool overrideNewline)
		{
			int charVal = Convert.ToInt32(c);

			switch(charVal)
			{
				case 0x00: MSXWriteNonKana(' '); break;
				case 0x01: MSXWriteNonKana('!'); break;
				case 0x02: MSXWriteNonKana('@'); break; // should be impossible outside of AG00, since it's a command
				case 0x03: MSXWriteNonKana('#'); break;
				case 0x04: MSXWriteNonKana('$'); break;
				case 0x05: MSXWriteNonKana('%'); break;
				case 0x06: MSXWriteNonKana('&'); break;
				case 0x07: MSXWriteNonKana('\''); break; // impossible outside of AG00
				case 0x08: MSXWriteNonKana('('); break;
				case 0x09: MSXWriteNonKana(')'); break;
				case 0x0A: MSXWriteNonKana('*'); break;
				case 0x0B: MSXWriteNonKana('+'); break;
				case 0x0C: MSXWriteNonKana(','); break;
				case 0x0D: MSXWriteNonKana('-'); break;
				case 0x0E: MSXWriteNonKana('.'); break;
				case 0x0F: MSXWriteNonKana('/'); break; // impossible outside of AG00
				case 0x10: MSXWriteNonKana('0'); break;
				case 0x11: MSXWriteNonKana('1'); break;
				case 0x12: MSXWriteNonKana('2'); break;
				case 0x13: MSXWriteNonKana('3'); break;
				case 0x14: MSXWriteNonKana('4'); break;
				case 0x15: MSXWriteNonKana('5'); break;
				case 0x16: MSXWriteNonKana('6'); break;
				case 0x17: MSXWriteNonKana('7'); break;
				case 0x18: MSXWriteNonKana('8'); break;
				case 0x19: MSXWriteNonKana('9'); break;
				case 0x1A: MSXWriteNonKana('['); break;
				case 0x1B: MSXWriteNonKana(']'); break;
				case 0x1C: MSXWriteNonKana('<'); break;
				case 0x1D: MSXWriteNonKana('='); break;
				case 0x1E: MSXWriteNonKana('>'); break;
				case 0x1F: MSXWriteNonKana('?'); break;
				// 0x20 invalid
				// 0x21 through 5f are mostly impossible outside of AG00 due to being ASCII characters/commands. The
				// symbols are guesswork, since most of them are identical to the above.
				case 0x21: MSXWriteNonKana('!'); break;
				case 0x22: MSXWriteNonKana('"'); break; 
				case 0x23: MSXWriteNonKana('#'); break;
				case 0x24: MSXWriteNonKana('$'); break;
				case 0x25: MSXWriteNonKana('%'); break;
				case 0x26: MSXWriteNonKana('&'); break;
				case 0x27: MSXWriteNonKana('\''); break;
				case 0x28: MSXWriteNonKana('('); break;
				case 0x29: MSXWriteNonKana(')'); break;
				case 0x2A: MSXWriteNonKana('*'); break;
				case 0x2B: MSXWriteNonKana('+'); break;
				case 0x2C: MSXWriteNonKana(','); break;
				case 0x2D: MSXWriteNonKana('-'); break;
				case 0x2E: MSXWriteNonKana('.'); break;
				case 0x2F: MSXWriteNonKana('/'); break;
				case 0x30: MSXWriteNonKana('0'); break;
				case 0x31: MSXWriteNonKana('1'); break;
				case 0x32: MSXWriteNonKana('2'); break;
				case 0x33: MSXWriteNonKana('3'); break;
				case 0x34: MSXWriteNonKana('4'); break;
				case 0x35: MSXWriteNonKana('5'); break;
				case 0x36: MSXWriteNonKana('6'); break;
				case 0x37: MSXWriteNonKana('7'); break;
				case 0x38: MSXWriteNonKana('8'); break;
				case 0x39: MSXWriteNonKana('9'); break;
				case 0x3A: MSXWriteNonKana(':'); break;
				case 0x3B: MSXWriteNonKana(';'); break;
				case 0x3C: MSXWriteNonKana('<'); break;
				case 0x3D: MSXWriteNonKana('='); break;
				case 0x3E: MSXWriteNonKana('>'); break;
				case 0x3F: MSXWriteNonKana('?'); break;
				case 0x40: MSXWriteNonKana('@'); break;
				case 0x41: MSXWriteNonKana('A'); break;
				case 0x42: MSXWriteNonKana('B'); break;
				case 0x43: MSXWriteNonKana('C'); break;
				case 0x44: MSXWriteNonKana('D'); break;
				case 0x45: MSXWriteNonKana('E'); break;
				case 0x46: MSXWriteNonKana('F'); break;
				case 0x47: MSXWriteNonKana('G'); break;
				case 0x48: MSXWriteNonKana('H'); break;
				case 0x49: MSXWriteNonKana('I'); break;
				case 0x4A: MSXWriteNonKana('J'); break;
				case 0x4B: MSXWriteNonKana('K'); break;
				case 0x4C: MSXWriteNonKana('L'); break;
				case 0x4D: MSXWriteNonKana('M'); break;
				case 0x4E: MSXWriteNonKana('N'); break;
				case 0x4F: MSXWriteNonKana('O'); break;
				case 0x50: MSXWriteNonKana('P'); break;
				case 0x51: MSXWriteNonKana('Q'); break;
				case 0x52: MSXWriteNonKana('R'); break;
				case 0x53: MSXWriteNonKana('S'); break;
				case 0x54: MSXWriteNonKana('T'); break;
				case 0x55: MSXWriteNonKana('U'); break;
				case 0x56: MSXWriteNonKana('V'); break;
				case 0x57: MSXWriteNonKana('W'); break;
				case 0x58: MSXWriteNonKana('X'); break;
				case 0x59: MSXWriteNonKana('Y'); break;
				case 0x5A: MSXWriteNonKana('Z'); break;
				case 0x5B: MSXWriteNonKana('['); break;
				case 0x5C: MSXWriteNonKana('¥'); break;
				case 0x5D: MSXWriteNonKana(']'); break;
				case 0x5E: MSXWriteNonKana('^'); break;
				case 0x5F: MSXWriteNonKana('_'); break;
				case 0x60: WriteShiftJISChar("＠"); break;
				case 0x61: WriteShiftJISChar("Ａ"); break;
				case 0x62: WriteShiftJISChar("Ｂ"); break;
				case 0x63: WriteShiftJISChar("Ｃ"); break;
				case 0x64: WriteShiftJISChar("Ｄ"); break;
				case 0x65: WriteShiftJISChar("Ｅ"); break;
				case 0x66: WriteShiftJISChar("Ｆ"); break;
				case 0x67: WriteShiftJISChar("Ｇ"); break;
				case 0x68: WriteShiftJISChar("Ｈ"); break;
				case 0x69: WriteShiftJISChar("Ｉ"); break;
				case 0x6A: WriteShiftJISChar("Ｊ"); break;
				case 0x6B: WriteShiftJISChar("Ｋ"); break;
				case 0x6C: WriteShiftJISChar("Ｌ"); break;
				case 0x6D: WriteShiftJISChar("Ｍ"); break;
				case 0x6E: WriteShiftJISChar("Ｎ"); break;
				case 0x6F: WriteShiftJISChar("Ｏ"); break;
				case 0x70: WriteShiftJISChar("Ｐ"); break;
				case 0x71: WriteShiftJISChar("Ｑ"); break;
				case 0x72: WriteShiftJISChar("Ｒ"); break;
				case 0x73: WriteShiftJISChar("Ｓ"); break;
				case 0x74: WriteShiftJISChar("Ｔ"); break;
				case 0x75: WriteShiftJISChar("Ｕ"); break;
				case 0x76: WriteShiftJISChar("Ｖ"); break;
				case 0x77: WriteShiftJISChar("Ｗ"); break;
				case 0x78: WriteShiftJISChar("Ｘ"); break;
				case 0x79: WriteShiftJISChar("Ｙ"); break;
				case 0x7A: WriteShiftJISChar("Ｚ"); break;
				case 0x7B: WriteShiftJISChar("｛"); break; // impossible outside of AG00
				case 0x7C: WriteShiftJISChar("¥"); break;
				case 0x7D: WriteShiftJISChar("｝"); break; // impossible outside of AG00
				case 0x7E: WriteShiftJISChar("＾"); break;
				// 0x7F to 0x85 invalid
				case 0x86: WriteShiftJISChar("を"); break;
				case 0x87: WriteShiftJISChar("ぁ"); break;
				case 0x88: WriteShiftJISChar("ぃ"); break;
				case 0x89: WriteShiftJISChar("ぅ"); break;
				case 0x8A: WriteShiftJISChar("ぇ"); break;
				case 0x8B: WriteShiftJISChar("ぉ"); break;
				case 0x8C: WriteShiftJISChar("ゃ"); break;
				case 0x8D: WriteShiftJISChar("ゅ"); break;
				case 0x8E: WriteShiftJISChar("ょ"); break;
				case 0x8F: WriteShiftJISChar("っ"); break;
				// 0x90 invalid
				case 0x91: WriteShiftJISChar("あ"); break;
				case 0x92: WriteShiftJISChar("い"); break;
				case 0x93: WriteShiftJISChar("う"); break;
				case 0x94: WriteShiftJISChar("え"); break;
				case 0x95: WriteShiftJISChar("お"); break;
				case 0x96: WriteShiftJISChar("か"); break;
				case 0x97: WriteShiftJISChar("き"); break;
				case 0x98: WriteShiftJISChar("く"); break;
				case 0x99: WriteShiftJISChar("け"); break;
				case 0x9A: WriteShiftJISChar("こ"); break;
				case 0x9B: WriteShiftJISChar("さ"); break;
				case 0x9C: WriteShiftJISChar("し"); break;
				case 0x9D: WriteShiftJISChar("す"); break;
				case 0x9E: WriteShiftJISChar("せ"); break;
				case 0x9F: WriteShiftJISChar("そ"); break;
				// 0xA0 invalid
				case 0xA1: WriteShiftJISChar("｡"); break;
				case 0xA2: WriteShiftJISChar("｢"); break;
				case 0xA3: WriteShiftJISChar("｣"); break;
				case 0xA4: WriteShiftJISChar("､"); break;
				case 0xA5: WriteShiftJISChar("・"); break;
				case 0xA6: WriteShiftJISChar("ヲ"); break;
				case 0xA7: WriteShiftJISChar("ァ"); break;
				case 0xA8: WriteShiftJISChar("ィ"); break;
				case 0xA9: WriteShiftJISChar("ゥ"); break;
				case 0xAA: WriteShiftJISChar("ェ"); break;
				case 0xAB: WriteShiftJISChar("ォ"); break;
				case 0xAC: WriteShiftJISChar("ャ"); break;
				case 0xAD: WriteShiftJISChar("ュ"); break;
				case 0xAE: WriteShiftJISChar("ョ"); break;
				case 0xAF: WriteShiftJISChar("ッ"); break;
				case 0xB0: WriteShiftJISChar("ー"); break;
				case 0xB1: WriteShiftJISChar("ア"); break;
				case 0xB2: WriteShiftJISChar("イ"); break;
				case 0xB3: WriteShiftJISChar("ウ"); break;
				case 0xB4: WriteShiftJISChar("エ"); break;
				case 0xB5: WriteShiftJISChar("オ"); break;
				case 0xB6: WriteShiftJISChar("カ"); break;
				case 0xB7: WriteShiftJISChar("キ"); break;
				case 0xB8: WriteShiftJISChar("ク"); break;
				case 0xB9: WriteShiftJISChar("ケ"); break;
				case 0xBA: WriteShiftJISChar("コ"); break;
				case 0xBB: WriteShiftJISChar("サ"); break;
				case 0xBC: WriteShiftJISChar("シ"); break;
				case 0xBD: WriteShiftJISChar("ス"); break;
				case 0xBE: WriteShiftJISChar("セ"); break;
				case 0xBF: WriteShiftJISChar("ソ"); break;
				case 0xC0: WriteShiftJISChar("タ"); break;
				case 0xC1: WriteShiftJISChar("チ"); break;
				case 0xC2: WriteShiftJISChar("ツ"); break;
				case 0xC3: WriteShiftJISChar("テ"); break;
				case 0xC4: WriteShiftJISChar("ト"); break;
				case 0xC5: WriteShiftJISChar("ナ"); break;
				case 0xC6: WriteShiftJISChar("ニ"); break;
				case 0xC7: WriteShiftJISChar("ヌ"); break;
				case 0xC8: WriteShiftJISChar("ネ"); break;
				case 0xC9: WriteShiftJISChar("ノ"); break;
				case 0xCA: WriteShiftJISChar("ハ"); break;
				case 0xCB: WriteShiftJISChar("ヒ"); break;
				case 0xCC: WriteShiftJISChar("フ"); break;
				case 0xCD: WriteShiftJISChar("ヘ"); break;
				case 0xCE: WriteShiftJISChar("ホ"); break;
				case 0xCF: WriteShiftJISChar("マ"); break;
				case 0xD0: WriteShiftJISChar("ミ"); break;
				case 0xD1: WriteShiftJISChar("ム"); break;
				case 0xD2: WriteShiftJISChar("メ"); break;
				case 0xD3: WriteShiftJISChar("モ"); break;
				case 0xD4: WriteShiftJISChar("ヤ"); break;
				case 0xD5: WriteShiftJISChar("ユ"); break;
				case 0xD6: WriteShiftJISChar("ヨ"); break;
				case 0xD7: WriteShiftJISChar("ラ"); break;
				case 0xD8: WriteShiftJISChar("リ"); break;
				case 0xD9: WriteShiftJISChar("ル"); break;
				case 0xDA: WriteShiftJISChar("レ"); break;
				case 0xDB: WriteShiftJISChar("ロ"); break;
				case 0xDC: WriteShiftJISChar("ワ"); break;
				case 0xDD: WriteShiftJISChar("ン"); break;
				case 0xDE:
					if(parent.MergeDiacritic() && lastMSXKana != ' ') MSXConvertLastKana('゛');
					else WriteShiftJISChar("゛");
					break;
				case 0xDF:
					if(parent.MergeDiacritic() && lastMSXKana != ' ') MSXConvertLastKana('゜');
					else WriteShiftJISChar("゜");
					break;
				case 0xE0: WriteShiftJISChar("た"); break;
				case 0xE1: WriteShiftJISChar("ち"); break;
				case 0xE2: WriteShiftJISChar("つ"); break;
				case 0xE3: WriteShiftJISChar("て"); break;
				case 0xE4: WriteShiftJISChar("と"); break;
				case 0xE5: WriteShiftJISChar("な"); break;
				case 0xE6: WriteShiftJISChar("に"); break;
				case 0xE7: WriteShiftJISChar("ぬ"); break;
				case 0xE8: WriteShiftJISChar("ね"); break;
				case 0xE9: WriteShiftJISChar("の"); break;
				case 0xEA: WriteShiftJISChar("は"); break;
				case 0xEB: WriteShiftJISChar("ひ"); break;
				case 0xEC: WriteShiftJISChar("ふ"); break;
				case 0xED: WriteShiftJISChar("へ"); break;
				case 0xEE: WriteShiftJISChar("ほ"); break;
				case 0xEF: WriteShiftJISChar("ま"); break;
				case 0xF0: WriteShiftJISChar("み"); break;
				case 0xF1: WriteShiftJISChar("む"); break;
				case 0xF2: WriteShiftJISChar("め"); break;
				case 0xF3: WriteShiftJISChar("も"); break;
				case 0xF4: WriteShiftJISChar("や"); break;
				case 0xF5: WriteShiftJISChar("ゆ"); break;
				case 0xF6: WriteShiftJISChar("よ"); break;
				case 0xF7: WriteShiftJISChar("ら"); break;
				case 0xF8: WriteShiftJISChar("り"); break;
				case 0xF9: WriteShiftJISChar("る"); break;
				case 0xFA: WriteShiftJISChar("れ"); break;
				case 0xFB: WriteShiftJISChar("ろ"); break;
				case 0xFC: WriteShiftJISChar("わ"); break;
				case 0xFD: WriteShiftJISChar("ん"); break;
				// 0xFE - 0xFF invalid
				default:
					if(decompileMode == DecompileModeType.AG00)
					{
						RaiseError("Unknown text output: \"" + c + "\" at AG00 byte index " + parent.FileIndex +
							"." + Environment.NewLine);
					}
					else
					{
						RaiseError("Unknown text output: \"" + c + "\" at page " + curPage + " addr " +
							scenarioAddress + "." + Environment.NewLine);
					}
					return false;
			}

			return true;
		}

		private void MSXWriteNonKana(char c)
		{
			WriteByte(c);
			lastMSXKana = ' ';
		}

		private void MSXConvertLastKana(char diacritic)
		{
			char replacementChar = ' ';
			if(diacritic == '゛')
			{
				switch(lastMSXKana)
				{
					// Hiragana
					case 'か': replacementChar = 'が'; break;
					case 'き': replacementChar = 'ぎ'; break;
					case 'く': replacementChar = 'ぐ'; break;
					case 'け': replacementChar = 'げ'; break;
					case 'こ': replacementChar = 'ご'; break;

					case 'さ': replacementChar = 'ざ'; break;
					case 'し': replacementChar = 'じ'; break;
					case 'す': replacementChar = 'ず'; break;
					case 'せ': replacementChar = 'ぜ'; break;
					case 'そ': replacementChar = 'ぞ'; break;

					case 'た': replacementChar = 'だ'; break;
					case 'ち': replacementChar = 'ぢ'; break;
					case 'つ': replacementChar = 'づ'; break;
					case 'て': replacementChar = 'で'; break;
					case 'と': replacementChar = 'ど'; break;

					case 'は': replacementChar = 'ば'; break;
					case 'ひ': replacementChar = 'び'; break;
					case 'ふ': replacementChar = 'ぶ'; break;
					case 'へ': replacementChar = 'べ'; break;
					case 'ほ': replacementChar = 'ぼ'; break;

					// Katakana
					case 'カ': replacementChar = 'ガ'; break;
					case 'キ': replacementChar = 'ギ'; break;
					case 'ク': replacementChar = 'グ'; break;
					case 'ケ': replacementChar = 'ゲ'; break;
					case 'コ': replacementChar = 'ゴ'; break;

					case 'サ': replacementChar = 'ザ'; break;
					case 'シ': replacementChar = 'ジ'; break;
					case 'ス': replacementChar = 'ズ'; break;
					case 'セ': replacementChar = 'ゼ'; break;
					case 'ソ': replacementChar = 'ゾ'; break;

					case 'タ': replacementChar = 'ダ'; break;
					case 'チ': replacementChar = 'ヂ'; break;
					case 'ツ': replacementChar = 'ヅ'; break;
					case 'テ': replacementChar = 'デ'; break;
					case 'ト': replacementChar = 'ド'; break;

					case 'ハ': replacementChar = 'バ'; break;
					case 'ヒ': replacementChar = 'ビ'; break;
					case 'フ': replacementChar = 'ブ'; break;
					case 'ヘ': replacementChar = 'ベ'; break;
					case 'ホ': replacementChar = 'ボ'; break;
				}
			}
			else
			{
				switch(lastMSXKana)
				{
					case 'は': replacementChar = 'ぱ'; break;
					case 'ひ': replacementChar = 'ぴ'; break;
					case 'ふ': replacementChar = 'ぷ'; break;
					case 'へ': replacementChar = 'ぺ'; break;
					case 'ほ': replacementChar = 'ぽ'; break;

					case 'ハ': replacementChar = 'パ'; break;
					case 'ヒ': replacementChar = 'ピ'; break;
					case 'フ': replacementChar = 'プ'; break;
					case 'ヘ': replacementChar = 'ペ'; break;
					case 'ホ': replacementChar = 'ポ'; break;
				}
			}

			if(replacementChar != ' ')
			{
				outputStream.Position -= 2;
				WriteShiftJISChar(Convert.ToString(replacementChar));
			}

			lastMSXKana = ' ';
		}
	}
}
