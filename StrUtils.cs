// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NP.Utilities
{
    public static class StrUtils
    {
        public const string UNDERSCORE = "_";
        public const string PERIOD = ".";

        public const string COMMA_SEPARATOR = ", ";
        public const string PLAIN_PROP_PATH_LINK_SEPARATOR = ".";
        public const char BINDING_PATH_LINK_SEPARATOR = '/';

        public static string NullToEmpty(this string str)
        {
            return str ?? string.Empty;
        }

        public static bool StartsWith(this string str, IEnumerable<char> charsToFind, int idx = 0)
        {
            if (charsToFind == null)
                return false;

            int i = idx;
            foreach (char c in charsToFind)
            {
                if (c != str[i])
                    return false;

                i++;
            }

            return true;
        }

        public static (string firstRegion, string lastRegion) SplitIntoTwo(this string str, string separator, bool fromBeginningOrEnd = true)
        {
            int idx = 
                fromBeginningOrEnd ? str.IndexOf(separator) : str.LastIndexOf(separator);

            if (idx < 0)
            {
                return (str, null);
            }

            return (str.Substring(0, idx), str.Substring(idx + separator.Length));
        }

        public static string ReverseStr(this string str)
        {
            return new string(str.Reverse().ToArray());
        }

        public static (string firstRegion, string lastRegion) SplitIntoTwo
        (
            this string str, 
            Func<char, bool> splitConditionFunc, 
            bool fromBeginningOrEnd = true)
        {
            int len = str.Length;

            string strToCheckCondition = fromBeginningOrEnd ? str : str.ReverseStr();

            int i = 0;
            for (; i < len; i++)
            {
                if (splitConditionFunc(strToCheckCondition[i]))
                {
                    break;
                }
            }

            if (!fromBeginningOrEnd)
            {
                i = len - i;
            }

            return (str.Substring(0, i), str.Substring(i));
        }

        public static (string firstRegion, string secondRegion, string lastRegion) 
            SplitIntoThree(this string str, string separator)
        {
            (string firstRegion, string remainder) = str.SplitIntoTwo(separator);

            (string secondRegion, string lastRegion) = remainder.SplitIntoTwo(separator);

            return (firstRegion, secondRegion, lastRegion);
        }

        /// splits into 3 regions:
        /// 1. Before the start
        /// 2. [start to end)
        /// 3. [end to str's end)
        public static (string beginning, string foundStr, string end) SplitFromTo
        (
            this string str,
            string start,
            string end,
            bool firstOrLast = true // first by default
        )
        {
            var defaultResult = (string.Empty, string.Empty, string.Empty);

            if (str == null)
                return defaultResult;

            int startIdx = 0;
            int endIdx = str.Length;

            if (!string.IsNullOrEmpty(start))
            {
                startIdx = firstOrLast ? str.IndexOf(start) : str.LastIndexOf(start);

                if (startIdx < 0)
                {
                    startIdx = 0;
                }
                else
                {
                    startIdx += start.Length;
                }
            }

            if (!string.IsNullOrEmpty(end))
            {
                int endIndex = firstOrLast ? str.IndexOf(end) : str.LastIndexOf(end);

                if (endIndex >= 0)
                {
                    endIdx = endIndex;
                }
            }

            if (endIdx <= startIdx)
                return defaultResult;

            return (str.Substring(0, startIdx), str.Substring(startIdx, endIdx - startIdx), str.Substring(startIdx));
        }

        public static string SubstrFromTo
        (
            this string str,
            string start,
            string end,
            bool firstOrLast = true // first by default
        )
        {
            return str.SplitFromTo(start, end, firstOrLast).foundStr;
        }

        public static string FirstCharToLowerCase(this string str, bool checkForChange = false)
        {
            char firstChar = str[0];

            char lowerFirstChar = Char.ToLowerInvariant(firstChar);

            if ( (firstChar == lowerFirstChar) && checkForChange)
                throw new Exception($"Error: {str} does not start with a lower case char");

            return lowerFirstChar + str.Substring(1);
        }

        public static string LastChunk
        (
            this string str,
            string start,
            string end = null
        ) => SubstrFromTo(str, start, end, false);

        public static string StrConcat<T>
        (
            this IEnumerable<T> items,
            Func<T, string> toStr = null,
            string separator = COMMA_SEPARATOR
        )
        {
            if (items.IsNullOrEmpty())
                return null;

            if (toStr == null)
            {
                toStr = (item) => item.ToString();
            }

            string result = string.Empty;

            bool firstIteration = true;
            foreach (T item in items)
            {
                if (firstIteration)
                {
                    firstIteration = false;
                }
                else
                {
                    result += separator;
                }

                result += toStr(item);
            }

            return result;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static (string, string, string) BreakStrAtSeparator(this string strToBreak, string separator, int idxToStartFrom = 0, bool firstOrLast = true)
        {
            int idx = 
                firstOrLast ? 
                    strToBreak.IndexOf(separator, idxToStartFrom) : 
                    strToBreak.LastIndexOf(separator, idxToStartFrom);

            if (idx < 0)
            {
                return (strToBreak, "", "");
            }

            return (strToBreak.Substring(0, idx), separator, strToBreak.Substring(idx + separator.Length));
        }


        public static IEnumerable<string> SplitPath(this string path, char separator = '/')
        {
            return 
                path?.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries) ?? 
                      Enumerable.Empty<string>();
        }

        public static string StreamToString(this MemoryStream stream)
        {
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        public static string StreamWriterToString(this TextWriter streamWriter)
        {
            streamWriter.Flush();

            return (((StreamWriter)streamWriter).BaseStream as MemoryStream).StreamToString();
        }

        public static string CollectionToStr(this IEnumerable coll, string separator = ", ")
        {
            return coll?.Cast<object>().StrConcat(null, separator);
        }

        public static void Throw(this string str) => throw new Exception(str);

        public static TEnum Parse<TEnum>(this string str)
            where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), str);
        }

        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }
    }
}
