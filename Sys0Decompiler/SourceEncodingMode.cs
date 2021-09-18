using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys0Decompiler
{
	public abstract class SourceEncodingMode
	{
		// Returns Unicode codepoint of the first character of *s, and advances *s
		// to the next character.
		public abstract int nextCodepoint(char c);

		// Returns byte length of the first character of c.
		public abstract int mblen(char c);

		// Returns the number of characters in c.
		public abstract int mbslen(char c);
	}

	/*public class ShiftJISEncodingMode : SourceEncodingMode
	{
		public int nextCodepoint(char c)
		{
			UInt16 code = *(*s)++;
			if(is_2byte(code))
				code = (code << 8) | (uint8) * (*s)++;
			return sjis_to_unicode(code);
		}

		public UInt16 ConvertToUTF8(UInt16 code)
		{

		}

		private bool is_2byte(char c)
		{
			return (0x81 <= c && c <= 0x9f) || 0xe0 <= c;
		}
	}

	public class UTF8EncodingMode : SourceEncodingMode
	{

	}

	public class MSXEncodingMode : SourceEncodingMode
	{
		public UInt16 ConvertToShiftJIS(UInt16 code)
		{

		}
		public UInt16 ConvertToUTF8(UInt16 code)
		{

		}
	}*/
}
