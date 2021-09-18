using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sys0Decompiler
{
	class System3 : SystemVersion
	{
		public System3(DecompilerForm p)
			:base(p)
		{
			operands = new char[] { '*', '/', '+', '-', '=', '>', '<', '\\' };
			varStarts = new char[] { 'R', 'D', 'U', 'B', 'M', 'V' };
			validVars = new string[]  {
				"RND", "D01", "D02", "D03", "D04", "D05", "D06", "D07", "D08", "D09", "D10", "D11", "D12", "D13", "D14",
				"D15", "D16", "D17", "D18", "D19", "D20", "U01", "U02", "U03", "U04", "U05", "U06", "U07", "U08", "U09",
				"U10", "U11", "U12", "U13", "U14", "U15", "U16", "U17", "U18", "U19", "U20", "B01", "B02", "B03", "B04",
				"B05", "B06", "B07", "B08", "B09", "B10", "B11", "B12", "B13", "B14", "B15", "B16", "M_X", "M_Y"
			};
			validYCmds = new int[] {
				1, 2, 3, 4, 7, 8, 10, 13, 14, 16, 17, 18, 19, 25, 26, 27, 28, 30, 31, 32, 40, 41, 42, 43, 45, 46,
				60, 61, 70, 71, 73, 80, 81, 82, 100, 101, 102, 103, 104, 105, 106, 221, 222, 223, 224, 225, 226, 
				227, 228, 229, 230, 231, 232, 234, 236, 238, 239, 240, 250, 251, 252, 253, 254, 255
			};

			// 54 in System 1, 53 in System 2 and 3.
			numConst8BitMax = 53;
		}

		protected override void compile_cmd_calc()
		{
			// !
			int index = varNameToIndex(GetStringTo(":"));

			if(index == -1)	return;

			WriteVariableIndex(index);

			// Skip colon.
			curColumn++;

			// calc
			compile_cali("!");

			SkipClosingMark('!');
		}

		protected override void compile_cmd_branch()
		{
			// {
			compile_cali(":");

			SkipClosingMark(':');

			WriteWord(PrepBranchEnd());
		}

		protected override void compile_cmd_end_branch()
		{
			// We must erase the automatic write of the command character, since manual } characters do not
			// exist in System 3.
			outputStream.Position = outputStream.Position - 1;

			DefineBranchEnd();
		}

		protected override void compile_cmd_label_definition()
		{
			// *
			// We have to erase the label after marking its position in the file.
			string label = GetStringTo(":");

			DefineLabel(label);

			SkipClosingMark(':');
		}

		protected override void compile_cmd_label_jump()
		{
			// @
			string label = GetStringTo(":");

			WriteLabelCall(label);

			SkipClosingMark(':');
		}

		protected override void compile_cmd_label_call()
		{
			// \
			string label = GetStringTo(":");

			// Label call can be in the format \lblXXXX, or in the format \0, meaning return.
			int intVal;
			if(Int32.TryParse(label, out intVal))
			{
				if(intVal == 0)
				{
					WriteWord(0);
				}
				else
				{
					RaiseError("\\ Commands cannot be followed by a number other than 0 (indicating 'return').");
				}
			}
			else
			{
				WriteLabelCall(label);
			}

			SkipClosingMark(':');
		}

		protected override void compile_cmd_page_jump()
		{
			// &
			// next_page
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_page_call()
		{
			// %
			// next_page
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_set_menu()
		{
			// $
			if(!activeSetMenu)
			{
				activeSetMenu = true;

				WriteLabelCall(GetStringTo("$"));
				SkipClosingMark('$');
			}
			else
			{
				activeSetMenu = false;
			}
		}

		protected override void compile_cmd_set_verbobj()
		{
			// [
			// Address
			string label = GetStringTo(",");

			// Skip comma.
			curColumn++;

			// verb
			CGetAndWriteNumericByte(-1);

			// Skip comma.
			curColumn++;

			// obj
			CGetAndWriteNumericByte(-1);

			WriteLabelCall(label);

			SkipClosingMark(':');
		}

		protected override void compile_cmd_set_verbobj2()
		{
			// :
			//condition
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// Address
			string label = GetStringTo(",");

			// Skip comma.
			curColumn++;

			// verb
			CGetAndWriteNumericByte(-1);

			// Skip comma.
			curColumn++;

			// obj
			CGetAndWriteNumericByte(-1);

			WriteLabelCall(label);

			SkipClosingMark(':');
		}

		protected override void compile_cmd_a()
		{
			// "A" commands stand on their own and do not even use colons.
		}

		protected override void compile_cmd_b()
		{
			// cmd
			CGetAndWriteNumericByte();

			// Skip comma.
			curColumn++;

			// index
			compile_cali(",");
			curColumn++;

			// p1-p5
			compile_cali(",");
			curColumn++;
			compile_cali(",");
			curColumn++;
			compile_cali(",");
			curColumn++;
			compile_cali(",");
			curColumn++;
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_d()
		{
			// Unused.
		}

		protected override void compile_cmd_e()
		{
			// index
			compile_cali(",");
			curColumn++;

			// color
			compile_cali(",");
			curColumn++;

			// sx
			compile_cali(",");
			curColumn++;

			// sy
			compile_cali(",");
			curColumn++;

			// ex
			compile_cali(",");
			curColumn++;

			// ey
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_f()
		{
			// "F" commands stand on their own and do not even use colon.
		}

		protected override void compile_cmd_g()
		{
			// Page
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_h()
		{
			// length
			CGetAndWriteNumericByte();
			curColumn++;

			// val
			compile_cali(":");
		}

		protected override void compile_cmd_i()
		{
			// sx
			compile_cali(",");
			curColumn++;

			// sy
			compile_cali(",");
			curColumn++;

			// ex
			compile_cali(",");
			curColumn++;

			// ey
			compile_cali(",");
			curColumn++;

			// dx
			compile_cali(",");
			curColumn++;

			// dy
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_j()
		{
			// x
			compile_cali(",");
			curColumn++;

			// y
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_k()
		{
			// cmd
			CGetAndWriteNumericByte();
			SkipClosingMark(':');
		}

		protected override void compile_cmd_l()
		{
			// Index
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_m()
		{
			CGetAndWriteTextParam();
			SkipClosingMark(':');
		}

		protected override void compile_cmd_n()
		{
			// cmd
			CGetAndWriteNumericByte();
			curColumn++;

			// src
			compile_cali(",");
			curColumn++;

			// dest
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_o()
		{
			// cmd
			compile_cali(",");
			curColumn++;

			// dest
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_p()
		{
			// index
			compile_cali(",");
			curColumn++;

			// r
			compile_cali(",");
			curColumn++;

			// g
			compile_cali(",");
			curColumn++;

			// b
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_q()
		{
			// index
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_r()
		{
			// "R" commands stand on their own and do not even use colon.
		}

		protected override void compile_cmd_s()
		{
			// page
			CGetAndWriteNumericByte();

			SkipClosingMark(':');
		}

		protected override void compile_cmd_t()
		{
			// x
			compile_cali(",");
			curColumn++;

			// y
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_u()
		{
			// page
			compile_cali(",");

			// Skip comma
			curColumn++;

			// transparent
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_v()
		{
			// cmd
			compile_cali(",");
			curColumn++;

			// index
			compile_cali(":");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_w()
		{
			// x
			compile_cali(",");
			curColumn++;

			// y
			compile_cali(",");
			curColumn++;

			// color
			compile_cali(",");
			SkipClosingMark(':');
		}

		protected override void compile_cmd_x()
		{
			// index
			CGetAndWriteNumericByte();

			SkipClosingMark(':');
		}

		protected override void compile_cmd_y()
		{
			// cmd
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// param
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_z()
		{
			// cmd
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// param
			compile_cali(":");

			SkipClosingMark(':');
		}


		// Decompile Commands.

		protected override void decompile_cmd_calc()
		{
			// !
			StartLine();

			int index = DGetByte();
			if(0x80 <= index && index <= 0xbf)
			{
				index &= 0x3f;
			}
			else
			{
				index = ((index & 0x3f) << 8) | DGetByte();
			}

			string name = varIndexToName(index);
			string calc = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("!" + name + ":" + calc + "!" + Environment.NewLine);
		}

		protected override void decompile_cmd_branch()
		{
			// { 
			StartLine();

			string condition = decompile_cali();
			int endAddr = DGetWord();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText("{ " + condition + ":" + Environment.NewLine);
			}
			else if(decompileMode == DecompileModeType.FindLabels)
			{
				branchEndAddresses.Add(endAddr);
			}
			else if(decompileMode == DecompileModeType.SuddenEOF)
			{
				// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
				// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
				if(endAddr != 0)
				{
					WriteByte(endAddr);
				}
			}

			nestingLevel++;
		}

		protected override void decompile_cmd_branch_end()
		{
			RaiseError("Manual branch ends should not exist in compiled System 3 code.");
		}

		protected override void automaticBranchEnd() { 
			// }
			nestingLevel--;
			if(nestingLevel < 0) nestingLevel = 0;

			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText('}' + Environment.NewLine);
		}

		protected override void decompile_cmd_label_jump()
		{
			// @
			StartLine();

			int next_addr = DGetWord();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText("@lbl" + next_addr.ToString("X") + ":" + Environment.NewLine);
			}
			else if(decompileMode == DecompileModeType.FindLabels)
			{
				labelAddresses.Add(next_addr);
			}
			else if(decompileMode == DecompileModeType.SuddenEOF)
			{
				// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
				// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
				if(next_addr != 0)
				{
					WriteByte(next_addr);
				}
			}
		}

		protected override void decompile_cmd_label_call()
		{
			// \
			StartLine();

			int next_addr = DGetWord();

			// If address is 0, this is actually a return call, which should simply say "\0". It does not
			// contain any information about labels.
			if(next_addr == 0)
			{
				if(decompileMode == DecompileModeType.ProcessCode)
				{
					WriteText("\\0:" + Environment.NewLine);
				}
			}
			// If the address is anything else, it contains information about a label.
			else
			{
				if(decompileMode == DecompileModeType.ProcessCode)
				{
					WriteText("\\lbl" + next_addr.ToString("X") + ":" + Environment.NewLine);
				}
				else if(decompileMode == DecompileModeType.FindLabels)
				{
					labelAddresses.Add(next_addr);
				}
				else if(decompileMode == DecompileModeType.SuddenEOF)
				{
					// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
					// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
					if(next_addr != 0)
					{
						WriteByte(next_addr);
					}
				}
			}
		}

		protected override void decompile_cmd_page_jump()
		{
			// &
			StartLine();

			string next_page = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("&" + next_page + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_page_call()
		{
			// %
			StartLine();

			string next_page = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("%" + next_page + ":" + Environment.NewLine);
		}


		protected override void decompile_cmd_set_menu()
		{
			// $
			StartLine();

			if(!activeSetMenu)
			{
				activeSetMenu = true;

				// Old-style $ commands use the following format: $label,Text$. New-style insert quotes around the
				// text, $label,"Text"$.
				int menu_addr = DGetWord();

				if(decompileMode == DecompileModeType.ProcessCode)
				{
					WriteText("$ lbl" + menu_addr.ToString("X") + " $" + Environment.NewLine);
				}
				else if(decompileMode == DecompileModeType.FindLabels)
				{
					labelAddresses.Add(menu_addr);
				}
				else if(decompileMode == DecompileModeType.SuddenEOF)
				{
					// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
					// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
					if(menu_addr != 0)
					{
						WriteByte(menu_addr);
					}
					return;
				}
			}
			else
			{
				activeSetMenu = false;

				if(decompileMode == DecompileModeType.ProcessCode)
				{
					WriteText("$" + Environment.NewLine);
				}
			}
		}

		protected override void decompile_cmd_open_menu()
		{
			// [
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("]" + Environment.NewLine);

			// There's quite a lot of code after this in the original, but I don't believe we need to
			// decompile it.
		}

		protected override void decompile_cmd_set_verbobj()
		{
			// :
			StartLine();

			int verb = DGetByte();
			int obj = DGetByte();
			int addr = DGetWord();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText("[ lbl" + addr.ToString("X") + ", " + (verb+1) + ", " + (obj+1) + ":" +
					Environment.NewLine);
			}
			else if(decompileMode == DecompileModeType.FindLabels)
			{
				labelAddresses.Add(addr);
			}
			else if(decompileMode == DecompileModeType.SuddenEOF)
			{
				// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
				// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
				if(addr != 0)
				{
					WriteByte(addr);
				}
			}
		}

		protected override void decompile_cmd_set_verbobj2()
		{
			StartLine();

			string condition = decompile_cali();
			int verb = DGetByte();
			int obj = DGetByte();
			int addr = DGetWord();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText(": " + condition + ", lbl" + addr.ToString("X") + ", " + (verb+1) + ", " + (obj+1) + 
					":" + Environment.NewLine);
			}
			else if(decompileMode == DecompileModeType.FindLabels)
			{
				labelAddresses.Add(addr);
			}
			else if(decompileMode == DecompileModeType.SuddenEOF)
			{
				// DGetWord can theoretically find a byte of data before a sudden EOF. We'll print it if we 
				// can, but since a 0 is indistinguishable from no byte at all, it will be lost.
				if(addr != 0)
				{
					WriteByte(addr);
				}
			}
		}

		protected override void decompile_cmd_a()
		{
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("A" + Environment.NewLine);
		}

		protected override void decompile_cmd_b()
		{
			StartLine();

			byte cmd = DGetByte();
			string index = decompile_cali();
			string p1 = decompile_cali();
			string p2 = decompile_cali();
			string p3 = decompile_cali();
			string p4 = decompile_cali();
			string p5 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("B " + cmd + ", " + index + ", " + p1 + ", " + p2 + ", " + p3 + ", " + p4 + ", " + p5 +
					 ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_d()
		{
			// Unused.
		}

		protected override void decompile_cmd_e()
		{
			StartLine();

			string index = decompile_cali();
			//byte color;
			string color;

			/*try
			{*/
				//color = Convert.ToByte(decompile_cali());
				color = decompile_cali();
			/*}
			catch(FormatException e)
			{
				RaiseError("Color value in E statement is non-numeral (byte)");
				return;
			}
			catch(OverflowException e)
			{
				RaiseError("Color value in E statement is outside of the range 0-255.");
				return;
			}*/

			string sx = decompile_cali();
			string sy = decompile_cali();
			string ex = decompile_cali();
			string ey = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("E " + index + ", " + color + ", " + sx + ", " + sy + ", " + ex + ", " + ey + ":" +
					Environment.NewLine);
		}

		protected override void decompile_cmd_f()
		{
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("F" + Environment.NewLine);
		}

		protected override void decompile_cmd_g()
		{
			StartLine();

			string page = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("G " + page + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_h()
		{
			StartLine();

			byte length = DGetByte();
			string val = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("H " + length + ", " + val + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_i()
		{
			StartLine();

			string sx = decompile_cali();
			string sy = decompile_cali();
			string ex = decompile_cali();
			string ey = decompile_cali();
			string dx = decompile_cali();
			string dy = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("I " + sx + ", " + sy + ", " + ex + ", " + ey + ", " + dx + ", " + dy + ":" +
					Environment.NewLine);
		}

		protected override void decompile_cmd_j()
		{
			StartLine();

			string x = decompile_cali();
			string y = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("J " + x + ", " + y + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_k()
		{
			StartLine();

			byte cmd = DGetByte();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("K " + cmd + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_l()
		{
			StartLine();

			string index = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("L " + index + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_m()
		{
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("M ");

			char peek = DPeekChar();
			if(peek == '\'' || peek == '"')
			{
				peek = DGetChar();
				DGetAndWriteMessage((byte)peek);
			}
			else
			{
				DGetAndWriteMessage(0);
				scenarioAddress++; // skip end terminator
			}

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText(":" + Environment.NewLine);
		}

		protected override void decompile_cmd_n()
		{
			RaiseDummiedCommandWarning("N");

			StartLine();

			// Toshiya dummies N commands, so throw a warning if they appear. He does document them to the
			// extent that we know their params, so there should not be any errors.
			byte cmd = DGetByte();
			string src = decompile_cali();
			string dest = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("N " + cmd + ", " + src + ", " + dest + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_o()
		{
			StartLine();

			//byte cmd;
			string cmd;
			/*try
			{*/
				//cmd = Convert.ToByte(decompile_cali());
				cmd = decompile_cali();
			/*}
			catch(FormatException e)
			{
				RaiseError("Command value in O statement is non-numeral (byte)");
				return;
			}
			catch(OverflowException e)
			{
				RaiseError("Command value in O statement is outside of the range 0-255.");
				return;
			}*/

			string val;

			// During operation, O commands vary their parameter depending on the cmd: cmd 0 uses cali(), and 
			// any other value uses cali2. How should we decompile this? At a bit of a loss, I'm going to try
			// to use just cali()?
			/*if(cmd == 0)
			{*/
				val = decompile_cali();
			/*}
			else
			{
				val = decompile_cali2();
			}*/

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("O " + cmd + ", " + val + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_p()
		{
			StartLine();

			string index = decompile_cali();
			string r = decompile_cali();
			string g = decompile_cali();
			string b = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("P " + index + ", " + r + ", " + g + ", " + b + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_q()
		{
			StartLine();

			string index = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("Q " + index + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_r()
		{
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("R" + Environment.NewLine);
		}

		protected override void decompile_cmd_s()
		{
			StartLine();

			int page = DGetByte();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("S " + page + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_t()
		{
			StartLine();

			string x = decompile_cali();
			string y = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("T " + x + ", " + y + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_u()
		{
			StartLine();

			string page = decompile_cali();
			string transparent = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("U " + page + ", " + transparent + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_v()
		{
			StartLine();

			string cmd = decompile_cali();
			string index = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("V " + cmd + ", " + index + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_w()
		{
			StartLine();

			string x = decompile_cali();
			string y = decompile_cali();
			//byte color;
			string color;

			/*try
			{*/
				//color = Convert.ToByte(decompile_cali());
				color = decompile_cali();
			/*}
			catch(FormatException e)
			{
				RaiseError("Color value in E statement is non-numeral (byte)");
				return;
			}
			catch(OverflowException e)
			{
				RaiseError("Color value in E statement is outside of the range 0-255.");
				return;
			}*/

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("W " + x + ", " + y + ", " + color + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_x()
		{
			StartLine();

			int index = DGetByte();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("X " + index + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_y()
		{
			StartLine();

			string cmd = decompile_cali();
			string param = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText("Y " + cmd + ", " + param + ":" + Environment.NewLine);

				if(!validYCmds.Contains<int>(Convert.ToInt32(cmd))) {
					RaiseDummiedCommandWarning("Y" + cmd);
				}

				if(cmd == "8" && param == "31")
				{
					RaiseError("Page " + curPage + " includes a call to Y 8, 31:. This " +
						"is used to switch code archives with subsequent M commands. Sys0Decompiler only creates " +
						"combined code archives. For this reason, such calls (both the Y and the M) should be " +
						"eliminated before re-compilation. See the manual for more details.");
				}
			}
		}

		protected override void decompile_cmd_z()
		{
			StartLine();

			if(specialCase != SpecialCase.Patton)
			{
				string cmd = decompile_cali();
				string param = decompile_cali();

				if(decompileMode == DecompileModeType.ProcessCode)
					WriteText("Z " + cmd + ", " + param + ":" + Environment.NewLine);
			}

			else { 
				// Nise Makuri Tower uses three-param Zs.
				string cmd = decompile_cali();
				string param = decompile_cali();
				string p2 = decompile_cali();

				if(decompileMode == DecompileModeType.ProcessCode)
					WriteText("Z " + cmd + ", " + param + ", " + p2 + ":" + Environment.NewLine);
			}
		}

		// Cali, the function that translates calculations into results, seems to work on the following
		// assumptions:
		//
		//	1. It finds a number, either a const or a variable. Finding an operand or comparator first would
		//		return an error.
		//  2. At any point from here on out, a 0x7f (DEL) character marks the end of the calculation.
		//		If there are more than a single number still in memory, it returns an error.
		//  3. The system finds another number. Finding an operand or comparator without at least two numbers
		//		results in an error.
		//	4. The system finds either another number or an operand/comparator. If the latter, it applies the
		//		operand or comparator to the previous two numbers in memory, applying the most-recent to second-
		//		most-recent.
		//  5. The process continues until 0x7f (DEL) is found, at which point its error check is run. In short,
		//		there must be exactly one less comparator than there are numbers.
		//
		//
		// So, to reverse this process into code:
		//
		//	-  0x74 (DEL) has no equivalent in decompiled code.
		//  -  To ensure math is decompiled correctly, all operations should be enclosed in parantheses.
		//  -  Go through the process, storing var labels and consts as we go. Store calculations in a seperate
		//		array. Then output them to string in reverse order.

		//9423==-*
		//9*(4-(2==3))

		// 942==3-*
		// ((4==2)-3)*9


		// 9[(4==2)] 3-*


		protected void decompile_calc_step(Stack<string> calculations, string operand)
		{
			string operator2 = calculations.Pop();
			string operator1 = calculations.Pop();

			calculations.Push("(" + operator1 + operand + operator2 + ")");
		}



		protected override string decompile_cali()
		{
			UInt32[] cali = new UInt32[256];
			Stack<string> operators = new Stack<string>();
			//std::stack<string> allOperatorsAsReceived;
			int curVal;

			int p = 1;
			fatalError = true;

			//Stopwatch sw = new Stopwatch();

			while(p > 0)
			{
				//sw.Start();
				byte dat = DGetByte();
				if(decompileMode == DecompileModeType.SuddenEOF) break;

				// 除算はサポートしない？
				// Single-char var index.
				if(0x80 <= dat && dat <= 0xbf)
				{
					curVal = dat & 0x3f;
					operators.Push(varIndexToName(curVal));
					//allOperatorsAsReceived.push(varIndexToName(curVal));
					//cali[p++] = var[curVal];
					p++;

					/*sw.Stop();
					WriteElapsed("Cali Single-Char Var", sw.Elapsed);
					sw.Reset();*/
				}
				// Double-char var index.
				else if(0xc0 <= dat && dat <= 0xff)
				{
					curVal = ((dat & 0x3f) << 8) | DGetByte();
					operators.Push(varIndexToName(curVal));
					//allOperatorsAsReceived.push(varIndexToName(curVal));
					//cali[p++] = var[curVal];
					p++;

					/*sw.Stop();
					WriteElapsed("Cali Double-Char Var", sw.Elapsed);
					sw.Reset();*/
				}
				// Double-char const digit.
				else if(0x00 <= dat && dat <= 0x3f)
				{
					curVal = ((dat & 0x3f) << 8) | DGetByte();
					operators.Push(curVal.ToString());
					//allOperatorsAsReceived.push(std::to_string(curVal));
					//cali[p++] = curVal;
					p++;

					/*sw.Stop();
					WriteElapsed("Cali Double-Char Const", sw.Elapsed);
					sw.Reset();*/
				}
				// Single-char const digit.
				else if(0x40 <= dat && dat <= 0x76)
				{
					curVal = dat & 0x3f;
					operators.Push(curVal.ToString());
					//allOperatorsAsReceived.push(std::to_string(curVal));
					//cali[p++] = curVal;
					p++;

					/*sw.Stop();
					WriteElapsed("Cali Single-Char Const", sw.Elapsed);
					sw.Reset();*/
				}
				// Multiplication.
				else if(dat == 0x77)
				{
					decompile_calc_step(operators, " * ");
					//allOperatorsAsReceived.push("*");
					/*cali[p - 2] = cali[p - 2] * cali[p - 1];
					if(cali[p - 2] > 65535)
					{
						cali[p - 2] = 65535;
					}*/
					p--;

					/*sw.Stop();
					WriteElapsed("Multiplication", sw.Elapsed);
					sw.Reset();*/
				}
				// Division
				else if(dat == 0x78)
				{
					decompile_calc_step(operators, " / ");
					p--;
				}
				// Addition.
				else if(dat == 0x79)
				{
					decompile_calc_step(operators, " + ");
					//allOperatorsAsReceived.push("+");
					cali[p - 2] = cali[p - 2] + cali[p - 1];
					/*if(cali[p - 2] > 65535)
					{
						cali[p - 2] = 65535;
					}*/
					p--;

					/*sw.Stop();
					WriteElapsed("Addition", sw.Elapsed);
					sw.Reset();*/
				}
				// Subtraction.
				else if(dat == 0x7a)
				{
					decompile_calc_step(operators, " - ");
					//allOperatorsAsReceived.push("-");
					/*if(cali[p - 2] > cali[p - 1])
					{
						cali[p - 2] = cali[p - 2] - cali[p - 1];
					}
					else
					{
						cali[p - 2] = 0;
					}*/
					p--;

					/*sw.Stop();
					WriteElapsed("Subtraction", sw.Elapsed);
					sw.Reset();*/
				}
				// Equality.
				else if(dat == 0x7b)
				{
					decompile_calc_step(operators, " = ");
					//allOperatorsAsReceived.push("==");
					//cali[p - 2] = (cali[p - 2] == cali[p - 1]) ? 1 : 0;
					p--;

					/*sw.Stop();
					WriteElapsed("Equality", sw.Elapsed);
					sw.Reset();*/
				}
				// Less than.
				else if(dat == 0x7c)
				{
					decompile_calc_step(operators, " < ");
					//allOperatorsAsReceived.push("<");
					//cali[p - 2] = (cali[p - 2] < cali[p - 1]) ? 1 : 0;
					p--;

					/*sw.Stop();
					WriteElapsed("Less Than", sw.Elapsed);
					sw.Reset();*/
				}
				// Greater than.
				else if(dat == 0x7d)
				{
					decompile_calc_step(operators, " > ");
					//allOperatorsAsReceived.push(">");
					//cali[p - 2] = (cali[p - 2] > cali[p - 1]) ? 1 : 0;
					p--;

					/*sw.Stop();
					WriteElapsed("Greater Than", sw.Elapsed);
					sw.Reset();*/
				}
				// Inequality.
				else if(dat == 0x7e)
				{
					decompile_calc_step(operators, " \\ ");
					//allOperatorsAsReceived.push("!=");
					//cali[p - 2] = (cali[p - 2] != cali[p - 1]) ? 1 : 0;
					p--;

					/*sw.Stop();
					WriteElapsed("Inequality", sw.Elapsed);
					sw.Reset();*/
				}
				// End of calculation.
				else if(dat == 0x7f)
				{
					if(p == 2)
					{
						fatalError = false;
					}
					p = 0;
				}
			}

			if(operators.Count == 0)
			{
				RaiseError("Calculation with no operators.");
				return "";
			}

			string res = operators.Peek();

			// If there is a superfluous outer layer of parantheses, crop it.
			if(res[0] == '(' && res[res.Length - 1] == ')')
				res = res.Substring(1, res.Length - 2);

			return res;
		}

		protected override string decompile_cali2()
		{
			UInt16 dat = DGetByte();
			string res = "";

			if(0x80 <= dat && dat <= 0xbf)
			{
				res = (dat & 0x3f).ToString();
			}
			else if(0xc0 <= dat && dat <= 0xff)
			{
				res = (((dat & 0x3f) + 8) | DGetByte()).ToString();
			}
			else
			{
				fatalError = true;
			}
			if(DGetByte() != 0x7f)
			{
				fatalError = true;
			}
			return res;
		}

		protected override void CompileWriteOperand(char c)
		{
			switch(c)
			{
				case '*':
					WriteByte(0x77);
					break;
				case '/':
					WriteByte(0x78);
					break;
				case '+':
					WriteByte(0x79);
					break;
				case '-':
					WriteByte(0x7a);
					break;
				case '=':
					WriteByte(0x7b);
					break;
				case '<':
					WriteByte(0x7c);
					break;
				case '>':
					WriteByte(0x7d);
					break;
				case '\\':
					WriteByte(0x7e);
					break;
			}
		}
	}
}
