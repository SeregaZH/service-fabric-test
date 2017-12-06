using System;

namespace SFTestStateless.Intergation.Tests
{
    public class PersonProxy
    {
        public PersonProxy(string firstName, string lastName, DateTime dateOfBirdth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirdth = dateOfBirdth;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public DateTime DateOfBirdth { get; }
    }
}
