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
