using NP.Utilities.BasicInterfaces;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NP.Utilities
{
    public static class XmlSerializationUtils
    {
        public static string Serialize<T>(this T objToSerialize)
        {
            IPreSaveable saveable = objToSerialize as IPreSaveable;

            if (saveable != null)
            {
                saveable.BeforeSave();
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

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

        public static T Deserialize<T>(string xmlStr)
        {
            if (xmlStr.IsNullOrEmpty())
            {
                return default(T);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

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

        static string ReplaceEncoding(this string str)
        {
            return str.Replace
            (
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }
    }
}
