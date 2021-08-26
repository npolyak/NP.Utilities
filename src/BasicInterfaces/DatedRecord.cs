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

using System;

namespace NP.Utilities.BasicInterfaces
{
    public class DatedRecord<T>
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Record { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DatedRecord<T> targetRecord)
            {
                return TimeStamp == targetRecord.TimeStamp &&
                    Record.ObjEquals(targetRecord.Record);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TimeStamp.GetHashCode() ^ Record.GetHashCodeExtension();
        }
    }

    public static class DatedRecordExtensions
    {
        public static DatedRecord<T> ToDatedRecord<T>(T record)
        {
            return new DatedRecord<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Record = record
            };
        }
    }
}
