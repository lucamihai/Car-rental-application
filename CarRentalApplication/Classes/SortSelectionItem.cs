﻿using System.Diagnostics.CodeAnalysis;
using CarRentalApplication.Enums;

namespace CarRentalApplication.Classes
{
    [ExcludeFromCodeCoverage]
    public class SortSelectionItem
    {
        public SortSelectionItem(string name, SortOptions sortOptions)
        {
            Name = name;
            SortOptions = sortOptions;
        }

        public string Name { get; private set; }

        public SortOptions SortOptions { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
