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
using System.Collections.Generic;

namespace NP.Utilities
{
    public static class StrUtils
    {
        public const string UNDERSCORE = "_";
        public const string PERIOD = ".";

        public const string COMMA_SEPARATOR = ", ";
        public const string PLAIN_PATH_LINK_SEPARATOR = ".";

        public static string NullToEmpty(this string str)
        {
            if (str == null)
                return string.Empty;

            return str;
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

        public static string SubstrFromTo
        (
            this string str,
            string start,
            string end,
            bool firstOrLast = true // first by default
        )
        {
            if (str == null)
                return string.Empty;

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
                return string.Empty;

            return str.Substring(startIdx, endIdx - startIdx);
        }

        public static string FirstCharToLowerCase(this string str, bool checkForChange = false)
        {
            char firstChar = str[0];

            char lowerFirstChar = Char.ToLowerInvariant(firstChar);

            if ( (firstChar == lowerFirstChar) && checkForChange)
                throw new Exception($"Error: {str} does not start with a lower case char");

            return lowerFirstChar + str.Substring(1);
        }


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

        public static (string, string) BreakStrAtSeparator(this string strToBreak, string separator, bool firstOrLast = true)
        {
            int idx = 
                firstOrLast ? 
                    strToBreak.IndexOf(separator) : 
                    strToBreak.LastIndexOf(separator);

            if (idx < 0)
            {
                return (strToBreak, null);
            }

            return (strToBreak.Substring(0, idx), strToBreak.Substring(idx + separator.Length));
        }
    }
}
