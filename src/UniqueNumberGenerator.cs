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

namespace NP.Utilities
{
    public static class UniqueNumberGenerator
    {
        // just do not call it more often than once per second...
        public static long Generate()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(2020,1,1)).TotalSeconds;
        }
    }
}