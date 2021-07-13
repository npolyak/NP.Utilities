// (c) Nick Polyak 2021 - http://awebpros.com/
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
using System.Diagnostics;

namespace NP.Utilities
{
    public class StopWatch
    {
        public static StopWatch TheStopWatch { get; } =
            new StopWatch();

        DateTime _startDate;
        public StopWatch()
        {
            Reset();
        }

        public void Reset()
        {
            _startDate = DateTime.Now;
        }

        public double Diff()
        {
            DateTime now = DateTime.Now;

            return now.Subtract(_startDate).TotalMilliseconds;
        }

        public string GetDiffStr(string str = null)
        {
            str = "" + str.NullToEmpty() + " " + Diff();

            return str;
        }

        public void PrintDiff(string str = null)
        {
            str = GetDiffStr(str);

            Console.WriteLine(str);
        }

        public void PrintDiffToDebug(string str = null)
        {
            str = GetDiffStr(str);

            Debug.WriteLine(str);
        }

        public static void ResetStatic()
        {
            TheStopWatch.Reset();
        }

        public static void PrintDifference(string str = null)
        {
            TheStopWatch.PrintDiff(str);
        }

        public static void PrintDifferenceToDebug(string str = null)
        {
            TheStopWatch.PrintDiffToDebug(str);
        }
    }
}
