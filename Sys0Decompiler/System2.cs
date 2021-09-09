using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sys0Decompiler
{
	class System2 : SystemVersion
	{
		public System2(DecompilerForm p)
			: base(p)
		{
			operands = new char[] { '*', '/', '+', '-', '=', '>', '<', '\\' };
			varStarts = new char[] { 'R', 'D', 'U', 'B', 'M', 'V' };
			validVars = new string[]  {
				"RND", "D01", "D02", "D03", "D04", "D05", "D06", "D07", "D08", "D09", "D10", "D11", "D12", "D13", "D14",
				"D15", "D16", "D17", "D18", "D19", "D20", "U01", "U02", "U03", "U04", "U05", "U06", "U07", "U08", "U09",
				"U10", "U11", "U12", "U13", "U14", "U15", "U16", "U17", "U18", "U19", "U20", "B01", "B02", "B03", "B04",
				"B05", "B06", "B07", "B08", "B09", "B10", "B11", "B12", "B13", "B14", "B15", "B16", "M_X", "M_Y"
			};
			validCmds = new char[]
			{
				'!', '{', '}', '*', '@', '\\', '&', '%', '$', '[', ':', ']', 'A', 'B', 'D', 'E', 'F', 'G', 'H',
				'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y'/*, 'Z'*/
			};
			validYCmds = new int[] {
				1, 2, 3, 4, 5, 6, 7, 8, 14, 25, 26, 27, 40, 41, 42, 43, 221, 222, 223, 224, 225, 226, 227, 228, 229, 240,
				252, 254, 255
			};

			// 54 in System 1, 53 in System 2 and 3.
			numConst8BitMax = 53;
		}
		protected override void compile_cmd_calc()
		{
			// !
			int index = varNameToIndex(GetStringTo(":"));

			if(index == -1) return;

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
		}

		protected override void compile_cmd_end_branch()
		{
			// System 2 does not process end branches, they are essentially only for readability's sake.
			return;
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
			compile_cali(",");

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
			// Some programs use eight-param D commands, others use 3. The user will have to select which.
			if(parent.DCommandMode() == 8)
			{
				// p1
				compile_cali(",");

				// Skip comma.
				curColumn++;

				// p2-p8
				compile_cali(",");
				curColumn++;
				compile_cali(",");
				curColumn++;
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
			else {
				// p1
				compile_cali(",");
				curColumn++;
				compile_cali(",");
				curColumn++;
				compile_cali(":");

				SkipClosingMark(':');
			}
		}

		protected override void compile_cmd_e()
		{
			RaiseDummiedCommandWarning("E");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2-p3
			compile_cali(",");
			curColumn++;
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
			RaiseDummiedCommandWarning("I");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2-p3
			compile_cali(",");
			curColumn++;
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_j()
		{
			RaiseDummiedCommandWarning("J");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_k()
		{
			// "K" commands are apparently unused, but TOTO also treats them as equivalent to R? He says
			// one thing a mere line away from the other, I don't know what to say.
			RaiseDummiedCommandWarning("K");
		}

		protected override void compile_cmd_l()
		{
			// Index
			CGetAndWriteNumericByte();

			SkipClosingMark(':');
		}

		protected override void compile_cmd_m()
		{
			CGetAndWriteTextParam();
			SkipClosingMark(':');
		}

		protected override void compile_cmd_n()
		{
			RaiseDummiedCommandWarning("N");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_o()
		{
			// st
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// width
			compile_cali(",");
			curColumn++;

			// height
			compile_cali(":");

			SkipClosingMark(':');
		}

		protected override void compile_cmd_p()
		{
			// param
			CGetAndWriteNumericByte();

			SkipClosingMark(':');
		}

		protected override void compile_cmd_q()
		{
			// index
			CGetAndWriteNumericByte();

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
			RaiseDummiedCommandWarning("T");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2-p3
			compile_cali(",");
			curColumn++;
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
			RaiseDummiedCommandWarning("V");

			// p1
			CGetAndWriteNumericByte();

			// Skip comma.
			curColumn++;

			// p2-p28
			for(int i=0; i<28; i++)
			{
				compile_cali(",");
				curColumn++;
			}

			// p30
			compile_cali(":");

			SkipClosingMark(':');
		}
		protected override void compile_cmd_w()
		{
			RaiseDummiedCommandWarning("W");

			// p1
			compile_cali(",");

			// Skip comma.
			curColumn++;

			// p2-p4
			compile_cali(",");
			curColumn++;
			compile_cali(",");
			curColumn++;
			compile_cali(":");

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
			RaiseDummiedCommandWarning("Z");

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

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("{ " + condition + ":" + Environment.NewLine);

			nestingLevel++;
		}

		protected override void decompile_cmd_branch_end()
		{
			// }
			nestingLevel--;
			if(nestingLevel < 0) nestingLevel = 0;

			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText('}' + Environment.NewLine);
		}

		protected override void automaticBranchEnd()
		{
			RaiseError("Automatic branch ends should not occur in System 2 code.");
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
				WriteText("[ lbl" + addr.ToString("X") + ", " + (verb + 1) + ", " + (obj + 1) + ":" +
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
				WriteText(": " + condition + ", lbl" + addr.ToString("X") + ", " + (verb + 1) + ", " + (obj + 1) +
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

			string cmd = decompile_cali();
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
			RaiseDummiedCommandWarning("D");

			StartLine();

			string p1, p2, p3;

			// Some programs use eight-param D commands, others use 3. The user will have to select which.
			if(parent.DCommandMode() == 8)
			{
				p1 = decompile_cali();
				p2 = decompile_cali();
				p3 = decompile_cali();
				string p4 = decompile_cali();
				string p5 = decompile_cali();
				string p6 = decompile_cali();
				string p7 = decompile_cali();
				string p8 = decompile_cali();

				if(decompileMode == DecompileModeType.ProcessCode)
					WriteText("D " + p1 + ", " + p2 + ", " + p3 + ", " + p4 + ", " + p5 +
						 ", " + p6 + ", " + p7 + ", " + p8 + ":" + Environment.NewLine);
			}
			else
			{
				p1 = decompile_cali();
				p2 = decompile_cali();
				p3 = decompile_cali();

				if(decompileMode == DecompileModeType.ProcessCode)
					WriteText("D " + p1 + ", " + p2 + ", " + p3 + ":" + Environment.NewLine);
			}
		}

		protected override void decompile_cmd_e()
		{
			RaiseDummiedCommandWarning("E");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();
			string p3 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("E " + p1 + ", " + p2 + ", " + p3 + ":" + Environment.NewLine);
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
			RaiseDummiedCommandWarning("I");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();
			//string p3 = decompile_cali();
			 byte p3 = DGetByte(); // Rance III System 2

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("I " + p1 + ", " + p2 + ", " + p3 + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_j()
		{
			RaiseDummiedCommandWarning("J");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("J " + p1 + ", " + p2 + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_k()
		{
			RaiseDummiedCommandWarning("K");

			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("K" + Environment.NewLine);
		}

		protected override void decompile_cmd_l()
		{
			StartLine();

			int index = DGetByte();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("L " + index + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_m()
		{
			StartLine();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("M '");

			DGetAndWriteTextParam(':');

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("':" + Environment.NewLine);
		}

		protected override void decompile_cmd_n()
		{
			RaiseDummiedCommandWarning("N");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("N " + p1 + ", " + p2 + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_o()
		{
			StartLine();

			string st = decompile_cali();
			string width = decompile_cali();
			string height = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("O " + st + ", " + width + ", " + height + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_p()
		{
			StartLine();

			int param = DGetByte();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("P " + param + ":" + Environment.NewLine);
		}

		protected override void decompile_cmd_q()
		{
			StartLine();

			int index = DGetByte();

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
			RaiseDummiedCommandWarning("T");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();
			string p3 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("T " + p1 + ", " + p2 + ", " + p3 + ":" + Environment.NewLine);
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
			RaiseDummiedCommandWarning("V");

			StartLine();

			string res = "V";
			int cnt = 0;

			int cmd = DGetByte();

			res += " " + Convert.ToString(cmd);

			// V has 29 cali parameters.
			for(int i=0; i<29; i++)
			{
				res += ", " + decompile_cali();
			}
			res += ":";

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText(res + Environment.NewLine);

			/*string p1 = decompile_cali();
			string p2 = decompile_cali();

				WriteText("V " + p1 + ", " + p2 + ":" + Environment.NewLine);*/
		}

		protected override void decompile_cmd_w()
		{
			RaiseDummiedCommandWarning("W");

			StartLine();

			string p1 = decompile_cali();
			string p2 = decompile_cali();
			string p3 = decompile_cali();
			string p4 = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
				WriteText("W " + p1 + ", " + p2 + ", " + p3 + ", " + p4 + ":" + Environment.NewLine);
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

				if(!validYCmds.Contains<int>(Convert.ToInt32(cmd)))
				{
					RaiseDummiedCommandWarning("Y " + cmd);
				}
			}
		}

		protected override void decompile_cmd_z()
		{
			RaiseDummiedCommandWarning("Z");

			StartLine();

			string cmd = decompile_cali();
			string param = decompile_cali();

			if(decompileMode == DecompileModeType.ProcessCode)
			{
				WriteText("Z " + cmd + ", " + param + ":" + Environment.NewLine);

				RaiseDummiedCommandWarning("Z " + cmd);
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
			if(calculations.Count < 2)
			{
				RaiseError("Calculation stack undersized for calc step.");
				return;
			}
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
					else
					{
						RaiseError("Calculation error, page " + curPage + " final addr " + scenarioAddress);
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
