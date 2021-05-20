using System;

namespace PDCoreTest
{
    public class Employee
    {
        public string FirstName { get; set; }

        public void Speak()
        {
            Console.WriteLine("Hi, my name is {0}", FirstName);
        }
    }
}
