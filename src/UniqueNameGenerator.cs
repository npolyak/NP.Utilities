﻿// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Utilities
{

    public static class UniqueNameGenerator
    {
        public const string UNDERSCORE = "_";

        // assumes that str ends with _<number>
        // if not - number returned is 0
        private static int GetEndNumber(this string str)
        {
            string ending = str.SubstrFromTo(UNDERSCORE, null, false);

            if (ending.IsStrNullOrWhiteSpace())
            {
                return 0;
            }

            if (Int32.TryParse(ending, out int result))
            {
                return result;
            }

            return 0;
        }

        // generate the number after the max number assuming that 
        // the names end with _<number>
        public static int GenerateUniqueNumber(this IEnumerable<string> names)
        {
            int maxNumber = 0;

            IEnumerable<int> allNumbers = names.Select(name => name.GetEndNumber()).ToList();
            if (allNumbers.Count() > 0)
                maxNumber = allNumbers.Max();

            return maxNumber + 1;
        }

        public static string GetUniqueName(this IEnumerable<string> names, string prefix)
        {
            return $"{prefix}{UNDERSCORE}{names.GenerateUniqueNumber()}";
        }
    }
}
