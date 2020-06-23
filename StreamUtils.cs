using System.IO;

namespace NP.Utilities
{
    public static class StreamUtils
    {
        public static string FromMemoryStream(this MemoryStream memoryStream)
        {
            memoryStream.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(memoryStream);

            string str = reader.ReadToEnd();

            return str;
        }
        
        public static MemoryStream ToMemoryStream(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            MemoryStream memoryStream = new MemoryStream();

            StreamWriter writer = new StreamWriter(memoryStream);

            writer.Write(str);

            writer.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
