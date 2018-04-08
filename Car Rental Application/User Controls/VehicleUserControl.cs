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
    public partial class VehicleUserControl : UserControl
    {
        protected RentVehicleUserControl rentVehicleUserControl;
        protected ReturnFromRentUserControl returnFromRentUserControl;
        protected MainWindow mainWindow;
        protected string vehicleName;
        protected short damagePercent;
        protected short fuelPercentage;
        protected short id;
        public virtual void GetDetails() { }
        public VehicleUserControl()
        {
            InitializeComponent();
        }
        public void LinkToMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public void LinkToRentMenu(RentVehicleUserControl rentVehicleUserControl) { this.rentVehicleUserControl = rentVehicleUserControl; }
        public void LinkToReturnMenu(ReturnFromRentUserControl returnFromRentUserControl) { this.returnFromRentUserControl = returnFromRentUserControl; }

        public virtual void SetVehicleFuelPercentage(short fuelPercentage) { this.fuelPercentage = fuelPercentage; }
        public virtual void SetVehicleDamagePercentage(short damagePercent) { this.damagePercent = damagePercent; }
        public string GetVehicleName() { return vehicleName; }
        public short GetFuelPercentage() { return fuelPercentage; }
        public short GetDamagePercentage() { return damagePercent; }
        public short GetVehicleID() { return id; }

    }
}
