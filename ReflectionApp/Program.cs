using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReflectionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintClassInfoAndSay(typeof(Person));
            PrintClassInfoAndSay(typeof(Pet));

            using (StreamReader reader = new StreamReader(new FileStream("data.csv", FileMode.Open)))
            {
                string ns = typeof(Program).Namespace;
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(';');
                    string className = ns + "." + data[0];
                    var humanType = Type.GetType(className);
                    var instance = Activator.CreateInstance(humanType);
                    var propertyValues = data.TakeLast(data.Length - 1).ToArray();
                    var propertyInfos = humanType.GetProperties();
                    for (int i = 0; i < propertyInfos.Length; i++)
                    {
                        //Не записывать SecretValue
                        var propertyValue = propertyValues[i];
                        var propertyInfo = propertyInfos[i];
                        var convertedValue = Convert.ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(instance, convertedValue);
                    }
                    Console.WriteLine(instance);
                }
            }
            
        }

        private static void PrintClassInfoAndSay(Type objectType)
        {
            var typeFullName = objectType.FullName;
            PropertyInfo[] properties = objectType.GetProperties();
            string propertiesString = "";
            PropertyInfo? nameProperty = null;
            if (properties.Length > 0)
            {
                propertiesString = properties.Select(p => p.Name).Aggregate((s1, s2) => s1 + " " + s2 + " ");
                nameProperty = properties.First(p => p.Name.Equals("Name"));
            }

            Console.WriteLine("Type name: {0} \nProperties: {1}", typeFullName, propertiesString);
            MethodInfo? methodInfo = objectType.GetMethod("Say");
            
            if (methodInfo != null && nameProperty != null)
            {
                var instance = Activator.CreateInstance(objectType); //Person p = new Person();
                nameProperty.SetValue(instance, "Jack");         //p.Name = "Jack";
                methodInfo.Invoke(instance, new object[] { });   //p.Say();
            }
        }
    }

    class Pet
    {
        public string Name { get; set; }
        public void Say()
        {
            Console.WriteLine("My name is {0}", Name);
        }
        
    }
    
    class Person
    {

        public string Name { get; set; }
        public string Email { get; set; }

        public void Say()
        {
            Console.WriteLine("Hello! I'm {0}", Name);
        }
    }

    class Participant
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [SecretValue]
        public int Paid { get; set; }
        public override string ToString()
        {
            return Name+"-"+Email+"-"+Paid;
        }
    }


    class Presenter
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Topic { get; set; }
        [SecretValue]
        public int Salary { get; set; }
        public override string ToString()
        {
            return Name+"-"+Email+"-"+Topic+"-"+Salary;
        }
    }

    internal class SecretValueAttribute : Attribute
    {
    }
    
}