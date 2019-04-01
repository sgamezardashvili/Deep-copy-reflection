using System;
using System.Collections.Generic;

namespace DeepCopy
{
    public class Person
    {
        public string FirstName { get; set; }

        public int Age { get; set; }

        public Person Father { get; set; }

        public List<string> Addresses { get; set; }

        public List<int> ChildAges { get; set; }

        public List<Person> Childs { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var person = new Person
            {
                Age = 51,
                FirstName = "John",
                Addresses = new List<string> { "Address1", "Address2" },
                ChildAges = new List<int> { 35 },
                Father = new Person
                {
                    Age = 80,
                    FirstName = "Nick"
                },
                Childs = new List<Person> {
                    new Person
                    {
                        FirstName = "Anna",
                        Age = 35,
                        ChildAges = new List<int> { 11, 6 },
                        Childs = new List<Person> {
                            new Person
                            {
                                FirstName = "Mike",
                                Age = 11
                            },
                            new Person
                            {
                                FirstName = "Jack",
                                Age = 6
                            },
                        }
                    }
                }
            };

            var copy = DeepCopy(person);

            foreach (var property in copy.GetType().GetProperties())
            {
                Console.WriteLine($"{property?.Name} | {property.GetValue(copy)}");
            }
        }

        static object DeepCopy(object self)
        {
            Type type = self.GetType();

            var copy = Activator.CreateInstance(type);

            CopyProperties(type, self, copy);

            return copy;
        }

        static void CopyProperties(Type type, object self, object copy)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(self) == null)
                    continue;

                var propertyType = property.PropertyType;

                if (propertyType.IsGenericType && propertyType.Name.Contains("List"))
                {
                    type.GetProperty(property.Name).SetValue(copy, property.GetValue(self) as System.Collections.IList);
                    continue;
                }

                if (propertyType.IsClass && propertyType.Name != "String")
                    CopyProperties(propertyType, property.GetValue(self), property.GetValue(self));

                type.GetProperty(property.Name).SetValue(copy, property.GetValue(self));
            }
        }

    }
}

