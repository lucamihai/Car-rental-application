﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Car_Rental_Application.Classes;
using Car_Rental_Application.User_Controls;

namespace Car_Rental_Application.Forms
{
    public partial class FormRentVehicle : Form
    {
        public VehicleUserControl AvailableVehicle { get; set; }
        public VehicleUserControl VehicleToBeRented { get; set; }

        string errorOwnerNameNotProvided;
        string errorOwnerPhoneNumberNotProvided;

        public FormRentVehicle()
        {
            InitializeComponent();
        }

        public FormRentVehicle(VehicleUserControl availableVehicle)
        {
            InitializeComponent();

            AvailableVehicle = availableVehicle;

            errorOwnerNameNotProvided = Program.Language.Translate("Owner's name must be provided");
            errorOwnerPhoneNumberNotProvided = Program.Language.Translate("Owner's phone must be provided");

            labelOwnerName.Text = Program.Language.Translate("Owner name");
            labelOwnerPhoneNumber.Text = Program.Language.Translate("Owner phone number");
            labelReturnDate.Text = Program.Language.Translate("Return date");

            buttonRent.Text = Program.Language.Translate("Rent");
            buttonCancel.Text = Program.Language.Translate("Cancel");
        }

        private void buttonRent_Click(object sender, EventArgs e)
        {
            if (textBoxOwnerName.Text == "")
            {
                errorLabel.Text = "Owner's name must be provided";
                return;
            }

            if (textBoxOwnerPhoneNumber.Text == "")
            {
                errorLabel.Text = "Owner's phone must be provided";
                return;
            }

            string ownerName = textBoxOwnerName.Text;
            string ownerPhoneNumber = textBoxOwnerPhoneNumber.Text;
            Person owner = new Person(ownerName, ownerPhoneNumber);

            if (AvailableVehicle.GetType() == (new AvailableSedanUserControl()).GetType())
            {
                VehicleToBeRented = new RentedSedanUserControl(AvailableVehicle, owner, dateTimePickerReturnDate.Value);
            }

            if (AvailableVehicle.GetType() == (new AvailableMinivanUserControl()).GetType())
            {
                VehicleToBeRented = new RentedMinivanUserControl(AvailableVehicle, owner, dateTimePickerReturnDate.Value);
            }

            if (VehicleToBeRented != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
