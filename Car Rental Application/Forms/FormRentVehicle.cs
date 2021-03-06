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
        public Vehicle Vehicle { get; set; }
        public Rental Rental { get; set; }

        public FormRentVehicle()
        {
            InitializeComponent();
        }

        public FormRentVehicle(Vehicle vehicle)
        {
            InitializeComponent();

            Vehicle = vehicle;

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
                errorLabel.Text = ErrorMessages.OWNER_NAME_NOT_PROVIDED;
                return;
            }

            if (textBoxOwnerPhoneNumber.Text == "")
            {
                errorLabel.Text = ErrorMessages.OWNER_PHONE_NOT_PROVIDED;
                return;
            }

            string ownerName = textBoxOwnerName.Text;
            string ownerPhoneNumber = textBoxOwnerPhoneNumber.Text;
            Person owner = new Person(ownerName, ownerPhoneNumber);

            Rental = new Rental(Vehicle, owner, dateTimePickerReturnDate.Value);

            if (Rental != null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
