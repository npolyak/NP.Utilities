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
