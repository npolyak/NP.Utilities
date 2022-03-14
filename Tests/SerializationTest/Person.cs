using System.Xml.Serialization;

namespace SerializationTest
{
    public class Person
    {
        [XmlAttribute]
        public int Age { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"age = {Age}, name = {Name}";
        }
    }
}
