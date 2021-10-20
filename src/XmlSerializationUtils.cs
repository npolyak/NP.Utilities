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
using NP.Utilities.BasicInterfaces;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NP.Utilities
{
    public static class XmlSerializationUtils
    {
        public static string Serialize<T>(this T objToSerialize, params Type[] extraTypes)
        {
            IPreSaveable saveable = objToSerialize as IPreSaveable;

            saveable?.BeforeSave();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), extraTypes);

            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = UTF8Encoding.ASCII;
            settings.Indent = true;
            settings.NewLineChars = "\n";
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                xmlSerializer.Serialize(xmlWriter, objToSerialize);
            }

            return stringBuilder.ToString().ReplaceEncoding();
        }

        public static void SerializeToFile<T>(this T objToSerialize, string filePath, params Type[] extraTypes)
        {
            string serializationStr = objToSerialize.Serialize<T>(extraTypes);

            using StreamWriter writer = new StreamWriter(filePath);

            writer.Write(serializationStr);

            writer.Flush();
        }

        public static T Deserialize<T>(string xmlStr, params Type[] extraTypes)
        {
            if (xmlStr.IsNullOrEmpty())
            {
                return default(T);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), extraTypes);

            using (StringReader stringReader = new StringReader(xmlStr))
            {
                T result = (T)xmlSerializer.Deserialize(stringReader);

                IPastRestorable restorable =
                    result as IPastRestorable;
                if (restorable != null)
                {
                    restorable.AfterRestore();
                }

                return result;
            }
        }

        public static T DeserializeFromFile<T>(string filePath, params Type[] extraTypes)
        {
            using StreamReader reader = new StreamReader(filePath);

            string serializationStr = reader.ReadToEnd();

            return Deserialize<T>(serializationStr, extraTypes);
        }

        static string ReplaceEncoding(this string str)
        {
            return str.Replace
            (
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }
    }
}
