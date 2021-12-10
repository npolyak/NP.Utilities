using System;
using System.ComponentModel;
using System.Globalization;

namespace NP.Utilities
{
    public class Point2DTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string[] values = ((string)value).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (values.Length != 2)
            {
                throw new ArgumentException("Point2D should have two values");
            }

            double x = values[0].ToDouble("Point2D.X");
            double y = values[0].ToDouble("Point2D.Y");

            return new Point2D(x, y);
        }
    }
}
