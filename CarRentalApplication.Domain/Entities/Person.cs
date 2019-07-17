﻿using System.Diagnostics.CodeAnalysis;

namespace CarRentalApplication.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Person
    {
        public Person(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}