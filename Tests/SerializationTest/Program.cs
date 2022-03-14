// See https://aka.ms/new-console-template for more information

using NP.Utilities;
using SerializationTest;

Person p = new Person() { Name = "Joe", Age = 20 };

p.SerializeToFile("MyFile.xml");

Person p1 = XmlSerializationUtils.DeserializeFromFile<Person>("MyFile.xml", true);

Console.WriteLine(p1);
