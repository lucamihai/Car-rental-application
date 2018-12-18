﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Car_Rental_Application.Classes;

namespace Car_Rental_Application.User_Controls
{
    public partial class AvailableSedanUserControl : SedanUserControl
    {
        public AvailableSedanUserControl()
        {
            InitializeComponent();
        }

        public AvailableSedanUserControl(string vehicleName)
        {
            InitializeComponent();
            short id = IDManagement.GetLowestAvailableID();

            ID = id;
            vehicleNameValueLabel.Text = vehicleName;
            FuelPercentage = 100;
            DamagePercentage = 0;
            IDManagement.MarkIDAsUnavailable(id);

            UpdateLanguage(Program.Language);
        }

        public AvailableSedanUserControl(string vehicleName, short fuelPercent, short damagePercent)
        {
            InitializeComponent();
            short id = IDManagement.GetLowestAvailableID();

            ID = id;
            vehicleNameValueLabel.Text = vehicleName;
            FuelPercentage = fuelPercent;
            DamagePercentage = damagePercent;
            IDManagement.MarkIDAsUnavailable(id);

            UpdateLanguage(Program.Language);
        }

        public AvailableSedanUserControl(VehicleUserControl availableVehicle)
        {
            InitializeComponent();
            ID = availableVehicle.ID;
            vehicleNameValueLabel.Text = availableVehicle.VehicleName;
            FuelPercentage = availableVehicle.FuelPercentage;
            DamagePercentage = availableVehicle.DamagePercentage;
            IDManagement.MarkIDAsUnavailable(ID);

            UpdateLanguage(Program.Language);
        }

        public AvailableSedanUserControl(RentedSedanUserControl rentedSedan)
        {
            InitializeComponent();

            short returnedVehicleID = rentedSedan.ID;
            string returnedVehicleName = rentedSedan.VehicleName;
            short returnedVehicleFuelPercentage = rentedSedan.FuelPercentage;
            short returnedVehicleDamagePercentage = rentedSedan.DamagePercentage;

            ID = returnedVehicleID;
            vehicleNameValueLabel.Text = returnedVehicleName;
            FuelPercentage = returnedVehicleFuelPercentage;
            DamagePercentage = returnedVehicleDamagePercentage;

            UpdateLanguage(Program.Language);
        }

        #region Properties

        public override string Details
        {
            get
            {
                string details = "";
                details += "Sedan " + VehicleName + ", ";
                details += "registered with the id " + ID.ToString() + ", ";
                details += "has " + FuelPercentage.ToString() + " % fuel and ";
                details += "and is " + DamagePercentage.ToString() + " % damaged";

                return details;
            }
        }

        public override bool Selected
        {
            get
            {
                return checkboxSelect.Checked;
            }
            set
            {
                checkboxSelect.Checked = value;
            }
        }

        public override short ID
        {
            get
            {
                return Convert.ToInt16(IDValueLabel.Text);
            }
            protected set
            {
                IDValueLabel.Text = value.ToString();
                IDManagement.MarkIDAsUnavailable(value);
            }
        }

        public override string VehicleName
        {
            get
            {
                return vehicleNameValueLabel.Text;
            }
            protected set
            {
                vehicleNameValueLabel.Text = value;
            }
        }

        public override short FuelPercentage
        {
            get
            {
                return Convert.ToInt16(fuelPercentValueLabel.Text);
            }
            set
            {
                fuelPercentValueLabel.Text = value.ToString();
            }
        }

        public override short DamagePercentage
        {
            get
            {
                return Convert.ToInt16(damagePercentValueLabel.Text);
            }
            set
            {
                damagePercentValueLabel.Text = value.ToString();
            }
        }

        #endregion

        public void FromDatabase(short id, string name, short fuel, short damage)
        {
            ID = id;
            vehicleNameValueLabel.Text = VehicleName;
            FuelPercentage = fuel;
            DamagePercentage = damage;
        }

        private void selectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int indexOfCurrentVehicle = mainWindow.GetIndexOfAvailableVehicle(this);

            if (checkboxSelect.Checked == true)
            {
                mainWindow.indexesOfSelectedAvailableCars.Add(indexOfCurrentVehicle);
                return;
            }

            mainWindow.indexesOfSelectedAvailableCars.Remove(indexOfCurrentVehicle);
        }

        private void buttonRent_Click(object sender, EventArgs e)
        {
            mainWindow.RentForm(this);
        }

        public override void UpdateLanguage(Language language)
        {
            labelID.Text = language.Translate("ID");
            labelVehicleName.Text = language.Translate("Vehicle name");
            labelVehicleType.Text = language.Translate("Vehicle type");
            labelVehicleTypeValue.Text = language.Translate("Sedan");
            labelDamagePercentage.Text = language.Translate("Damage percentage");
            labelFuelPercentage.Text = language.Translate("Fuel percentage");

            checkboxSelect.Text = language.Translate("Select");

            buttonRent.Text = language.Translate("Rent");
        }
    }
}
