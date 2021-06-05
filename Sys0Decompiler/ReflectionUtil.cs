using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sys0Decompiler
{

    public static partial class ReflectionUtil
    {
        public static string ToString(object obj)
        {
            StringBuilder sb = new StringBuilder();
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

            bool needComma = false;
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(int) || field.FieldType == typeof(string) || field.FieldType.IsEnum)
                {
                    Util.PrintComma(sb, ref needComma);
                    sb.Append(field.Name + " = " + (field.GetValue(obj) ?? "null").ToString());
                }
            }
            return sb.ToString();

        }
    }

    public static partial class Util
    {
        public static void PrintComma(StringBuilder sb, ref bool needComma)
        {
            if (needComma)
            {
                sb.Append(", ");
            }
            else
            {
                needComma = true;
            }
        }

    }
}
