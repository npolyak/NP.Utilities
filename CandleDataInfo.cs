using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NP.Utilities
{
    public class CandleDataInfo
    {
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CandleDataInfo candleDataInfo)
            {
                return this.DateTime == candleDataInfo.DateTime;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return DateTime.GetHashCode();
        }

        public override string ToString()
        {
            return $"{DateTime:MM/dd/yyyy hh:mm tt}";
        }
    }

    public static class CandleDataInfoHelper
    {
        public static IEnumerable<CandleDataInfo> ParseData(this TextReader dataReader)
        {
            List<CandleDataInfo> result = new List<CandleDataInfo>();

            string line;
            while ((line = dataReader.ReadLine()) != null)
            {
                if (line.IsNullOrEmpty())
                    continue;

                string[] data = line.Split(',');

                CandleDataInfo dataItem = new CandleDataInfo()
                {
                    DateTime = DateTime.Parse(data[0], CultureInfo.InvariantCulture),
                    Open = double.Parse(data[1], CultureInfo.InvariantCulture),
                    High = double.Parse(data[2], CultureInfo.InvariantCulture),
                    Low = double.Parse(data[3], CultureInfo.InvariantCulture),
                    Close = double.Parse(data[4], CultureInfo.InvariantCulture)
                };

                result.Add(dataItem);
            }

            return result;
        }
    }
}
