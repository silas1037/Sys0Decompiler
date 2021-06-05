using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Sys0Decompiler
{
    public static partial class Extensions
    {
        public static void WriteToStream(this Stream streamToReadFrom, Stream streamToWriteTo)
        {
            WriteToStream(streamToReadFrom, streamToWriteTo, streamToReadFrom.Length - streamToReadFrom.Position);
        }

        public static void WriteToStream(this Stream streamToReadFrom, Stream streamToWriteTo, long byteCount)
        {
            int bufferSize = 65536;
            byte[] buffer = new byte[bufferSize];

            long bytesRemaining = byteCount;
            while (bytesRemaining > 0)
            {
                int bytesToRead = bufferSize;
                if (bytesToRead > bytesRemaining)
                {
                    bytesToRead = (int)bytesRemaining;
                }
                int bytesRead = streamToReadFrom.Read(buffer, 0, bytesToRead);
                if (bytesRead != bytesToRead)
                {
                    //???
                    if (bytesRead == 0)
                    {
                        throw new IOException("Failed to read from the stream");
                    }
                }
                streamToWriteTo.Write(buffer, 0, bytesRead);
                bytesRemaining -= bytesRead;
            }

        }


        /// <summary>
        /// Adds the elements of the specified sequence to the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="collection">The collection to add items to.</param>
        /// <param name="sequence">
        /// The sequence whose elements should be added to the end of the collection.
        /// </param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequence)
        {
            foreach (var item in sequence)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are
        /// delimited by a specified string.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">A string that delimits the substrings in this string.</param>
        /// <returns>
        /// An array whose elements contain the substrings in this string that are delimited
        /// by the separator string.
        /// </returns>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new string[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Joins substrings together with separator strings.
        /// </summary>
        /// <param name="strArr">The sequence of strings to join together.</param>
        /// <param name="joinString">The string to separate the strings by</param>
        /// <returns>A string which is the result of joining the strings together.</returns>
        public static string Join(this IEnumerable<string> strArr, string joinString)
        {
            StringBuilder sb = new StringBuilder();
            int count = strArr.Count();
            int i = 0;
            foreach (var str in strArr)
            {
                sb.Append(str);
                if (i < count - 1)
                {
                    sb.Append(joinString);
                }
                i++;
            }
            return sb.ToString();
        }

        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.GetOrDefault(key, default(TValue));
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public static T GetOrDefault<T>(this IList<T> list, int index, T defaultValue)
        {
            if (index >= 0 && index < list.Count)
            {
                return list[index];
            }
            else
            {
                return defaultValue;
            }
        }

        static Encoding shiftJis = Encoding.GetEncoding("gbk");

        public static string ReadStringFixedSize(this BinaryReader br, int length)
        {
            return ReadStringFixedSize(br, length, shiftJis);
        }

        public static string ReadStringFixedSize(this BinaryReader br, int length, Encoding encoding)
        {
            byte[] bytes = br.ReadBytes(length);
            int zeroIndex = Array.IndexOf<byte>(bytes, 0);
            if (zeroIndex == -1)
            {
                zeroIndex = bytes.Length;
            }
            return encoding.GetString(bytes, 0, zeroIndex);
        }

        public static void WriteStringFixedSize(this BinaryWriter bw, string str, int length)
        {
            WriteStringFixedSize(bw, str, length, shiftJis);
        }

        public static void WriteStringFixedSize(this BinaryWriter bw, string str, int length, Encoding encoding)
        {
            byte[] bytes = new byte[length];
            encoding.GetBytes(str, 0, str.Length, bytes, 0);
            bw.Write(bytes);
        }

        public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class, new()
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                var newValue = new TValue();
                dictionary.Add(key, newValue);
                return newValue;
            }
        }

        public static T GetOrNull<T>(this IList<T> list, int index)
        {
            if (index >= list.Count)
            {
                return default(T);
            }
            else
            {
                var value = list[index];
                return value;
            }
        }

        public static void SetOrAdd<T>(this IList<T> list, int index, T value)
        {
            if (index >= list.Count)
            {
                while (index > list.Count)
                {
                    list.Add(default(T));
                }
                list.Add(value);
            }
            else
            {
                list[index] = value;
            }
        }

        public static bool AnyEqualTo<T>(this IEnumerable<T> sequence, T valueToCompareTo) where T : IEquatable<T>
        {
            return sequence.Any(b => b.Equals(valueToCompareTo));
        }
    }

    public static partial class Extensions
    {
        static readonly byte[] zeroes = new byte[4096];
        public static void WriteZeroes(this Stream stream, int count)
        {
            while (count > 0)
            {
                int numberToWrite = count;
                if (numberToWrite > zeroes.Length)
                {
                    numberToWrite = zeroes.Length;
                }
                stream.Write(zeroes, 0, numberToWrite);

                count -= numberToWrite;
            }
        }
        public static void WriteZeroes(this Stream stream, long count)
        {
            while (count > 0)
            {
                long numberToWrite = count;
                if (numberToWrite > zeroes.Length)
                {
                    numberToWrite = zeroes.Length;
                }
                stream.Write(zeroes, 0, (int)numberToWrite);

                count -= numberToWrite;
            }
        }

    }

}
