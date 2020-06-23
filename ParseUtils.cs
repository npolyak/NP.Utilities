namespace NP.Utilities
{
    public static class ParseUtils
    {
        public static string TryParseStr(this string str, out decimal d)
        {
            if (!decimal.TryParse(str, out d))
            {
                return $"value '{str}' cannot be parsed as a decimal";
            }

            return null;
        }
    }
}
