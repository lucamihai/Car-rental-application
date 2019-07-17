﻿using System.Diagnostics.CodeAnalysis;
using CarRentalApplication.Domain.Entities;
using FluentValidation;

namespace CarRentalApplication.Domain.Validators
{
    [ExcludeFromCodeCoverage]
    public class VehicleValidator : AbstractValidator<Vehicle>
    {
        public VehicleValidator()
        {
            ValidateName();
            ValidateType();
            ValidateFuelPercentage();
            ValidateDamagePercentage();
        }

        private void ValidateName()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }

        private void ValidateType()
        {
            RuleFor(x => x.Type)
                .NotEmpty();
        }

        private void ValidateFuelPercentage()
        {
            RuleFor(x => x.FuelPercentage)
                .ExclusiveBetween((short) 0, (short) 100);
        }

        private void ValidateDamagePercentage()
        {
            RuleFor(x => x.DamagePercentage)
                .ExclusiveBetween((short)0, (short)100);
        }
    }
}