﻿using CarRentalApplication.Translating;

namespace CarRentalApplication.EntityViews
{
    public partial class MinivanView : VehicleView
    {
        public MinivanView()
        {
            InitializeComponent();
        }

        public MinivanView(string vehicleName, short fuelPercent = 0, short damagePercent = 0) : base(vehicleName, fuelPercent, damagePercent)
        {

        }
        public MinivanView(short id, string vehicleName, short fuelPercent = 0, short damagePercent = 0) : base(id, vehicleName, fuelPercent, damagePercent)
        {

        }


        public MinivanView(VehicleView vehicle) : base(vehicle)
        {

        }

        public override void UpdateLanguage(Language language)
        {
            labelId.Text = language.Translate("ID");
            labelVehicleName.Text = language.Translate("Vehicle name");
            labelVehicleType.Text = language.Translate("Vehicle type");
            labelVehicleTypeValue.Text = language.Translate("Minivan");
            labelDamagePercentage.Text = language.Translate("Damage percentage");
            labelFuelPercentage.Text = language.Translate("Fuel percentage");

            checkboxSelect.Text = language.Translate("Select");

            buttonRent.Text = language.Translate("Rent");
        }

        public override object Clone()
        {
            return new MinivanView(Id, VehicleName, FuelPercentage, DamagePercentage);
        }
    }
}