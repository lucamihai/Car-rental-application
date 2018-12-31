﻿using Car_Rental_Application.Classes;
using Car_Rental_Application.User_Controls;
using Car_Rental_Application.Forms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace Car_Rental_Application
{
    public partial class MainWindow : Form
    {
        List <Rental> rentals;
        List <Vehicle> vehicles;

        VehicleSorter availableCarsSorter;
        RentalSorter rentalSorter;

        DateTime programTime;

        ReturnedVehiclesLogManager returnedVehiclesLogManager;

        List<int> indexesOfSelectedVehicles = new List<int>();
        List<int> indexesOfSelectedRentals = new List<int>();
            
        public MainWindow()
        {
            InitializeComponent();

            errorLabel.Text = "";
            
            returnedVehiclesLogManager = new ReturnedVehiclesLogManager();
            returnedVehiclesLogManager.Path = "log.txt";

            rentals = new List<Rental>();
            vehicles = new List<Vehicle>();

            availableCarsSorter = new VehicleSorter();
            rentalSorter = new RentalSorter();

            saveToDatabaseToolStripMenuItem.Available = false;
            loadFromDatabaseToolStripMenuItem.Available = false;

            InitializeSortOptionsForAvailableVehicles();
            InitializeSortOptionsForRentedVehicles();

            timerProgramDateUpdater.Start();

            IDManagement.InitializeIndexes();          
        }

        void InitializeSortOptionsForAvailableVehicles()
        {
            sortAvailableSelectionComboBox.Items.Clear();

            string textID = Program.Language.Translate("ID");
            string textName = Program.Language.Translate("Name");
            string textType = Program.Language.Translate("Type");
            string textFuelPercentage = Program.Language.Translate("Fuel percentage");
            string textDamagePercentage = Program.Language.Translate("Damage percentage");

            SortSelectionItem selectionID = new SortSelectionItem(textID, Constants.SORT_BY_VEHICLE_ID);
            SortSelectionItem selectionName = new SortSelectionItem(textName, Constants.SORT_BY_VEHICLE_NAME);
            SortSelectionItem selectionType = new SortSelectionItem(textType, Constants.SORT_BY_VEHICLE_TYPE);
            SortSelectionItem selectionFuelPercentage = new SortSelectionItem(textFuelPercentage, Constants.SORT_BY_VEHICLE_FUEL_PERCENTAGE);
            SortSelectionItem selectionDamagePercentage = new SortSelectionItem(textDamagePercentage, Constants.SORT_BY_VEHICLE_DAMAGE_PERCENTAGE);

            sortAvailableSelectionComboBox.Items.Add(selectionID);
            sortAvailableSelectionComboBox.Items.Add(selectionName);
            sortAvailableSelectionComboBox.Items.Add(selectionType);
            sortAvailableSelectionComboBox.Items.Add(selectionFuelPercentage);
            sortAvailableSelectionComboBox.Items.Add(selectionDamagePercentage);

            sortAvailableSelectionComboBox.SelectedIndex = 0;
        }

        void InitializeSortOptionsForRentedVehicles()
        {
            sortRentedSelectionComboBox.Items.Clear();

            string textID = Program.Language.Translate("ID");
            string textName = Program.Language.Translate("Name");
            string textType = Program.Language.Translate("Type");
            string textOwnerName = Program.Language.Translate("Owner name");
            string textOwnerPhone = Program.Language.Translate("Owner phone");
            string textReturnDate = Program.Language.Translate("Return date");

            SortSelectionItem selectionID = new SortSelectionItem(textID, Constants.SORT_BY_VEHICLE_ID);
            SortSelectionItem selectionName = new SortSelectionItem(textName, Constants.SORT_BY_VEHICLE_NAME);
            SortSelectionItem selectionType = new SortSelectionItem(textType, Constants.SORT_BY_VEHICLE_TYPE);
            SortSelectionItem selectionOwnerName = new SortSelectionItem(textOwnerName, Constants.SORT_BY_VEHICLE_OWNER_NAME);
            SortSelectionItem selectionOwnerPhone = new SortSelectionItem(textOwnerPhone, Constants.SORT_BY_VEHICLE_OWNER_PHONE_NUMBER);
            SortSelectionItem selectionReturnDate = new SortSelectionItem(textReturnDate, Constants.SORT_BY_VEHICLE_RETURN_DATE);

            sortRentedSelectionComboBox.Items.Add(selectionID);
            sortRentedSelectionComboBox.Items.Add(selectionName);
            sortRentedSelectionComboBox.Items.Add(selectionType);
            sortRentedSelectionComboBox.Items.Add(selectionOwnerName);
            sortRentedSelectionComboBox.Items.Add(selectionOwnerPhone);
            sortRentedSelectionComboBox.Items.Add(selectionReturnDate);

            sortRentedSelectionComboBox.SelectedIndex = 0;
        }

        public void AddToRentalList(Rental rental)
        {
            rentals.Add(rental);
        }

        public int GetVehicleIndex(Vehicle vehicle)
        {
            return vehicles.IndexOf(vehicle);
        }

        public int GetRentalIndex(Rental rental)
        {
            return rentals.IndexOf(rental);
        }

        public void AddVehicle(Vehicle vehicle)
        {
            availableCarsElementsPanel.VerticalScroll.Value = 0;
            vehicles.Add(vehicle);
            PopulateVehiclesPanel();
        }

        void AddRental(Rental rental)
        {
            rentedCarsElementsPanel.VerticalScroll.Value = 0;
            rentals.Add(rental);
            PopulateRentalsPanel();
        }

        private void AddVehicle(object sender, EventArgs e)
        {
            FormAddVehicle formAddVehicle = new FormAddVehicle();

            var result = formAddVehicle.ShowDialog();
            if (result == DialogResult.OK)
            {
                Vehicle vehicleToBeAdded = formAddVehicle.Vehicle;
                AddVehicle(vehicleToBeAdded);
            }
        }

        public void RentForm(Vehicle vehicle)
        {
            FormRentVehicle formRentVehicle = new FormRentVehicle(vehicle);

            var result = formRentVehicle.ShowDialog();
            if (result == DialogResult.OK)
            {
                Rental rental = formRentVehicle.Rental;
                AddRental(rental);
                RemoveVehicleFromList(vehicle);
            }
        }

        public void ReturnForm(Vehicle vehicle)
        {
            FormReturnVehicle formReturnVehicle = new FormReturnVehicle(vehicle);

            var result = formReturnVehicle.ShowDialog();
            if (result == DialogResult.OK)
            {
                Vehicle returnedVehicle = formReturnVehicle.ReturnedVehicle;
                string orderDetails = formReturnVehicle.OrderDetails;

                returnedVehiclesLogManager.WriteToLog(orderDetails);

                //ReturnVehicleFromRent(returnedVehicle);
                //RemoveRentedCarFromList(vehicle);
            }
        }


        #region Toolstrip Menu

        #region Database

        private void connectToDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectToSQL();
        }

        private void loadFromDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string action = "load vehicles from the database";
            string consequence = "existing vehicles in the program will be removed";
            FormConfirmation formConfirmation = new FormConfirmation(action, consequence);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            GetAvailableVehiclesFromSQLDatabase();
            GetRentedVehiclesFromSQLDatabase();
        }

        private void saveToDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string action = "save vehicles to the database";
            string consequence = "existing vehicles in the database will be removed";
            FormConfirmation formConfirmation = new FormConfirmation(action, consequence);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            ClearAvailableVehiclesDatabase();
            ClearRentedVehiclesDatabase();

            //foreach (VehicleUserControl vehicle in availableVehicles)
            //    SaveAvailableVehicleToSQLDatabase(vehicle);

            //foreach (Vehicle vehicle in rentals)
                //SaveRentedVehicleToSQLDatabase(vehicle);
        }

        #endregion

        #region Local file

        private void saveToLocalFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string action = "save vehicles to local file";
            string consequence = "existing local file will be deleted";
            FormConfirmation formConfirmation = new FormConfirmation(action, consequence);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            StoreVehiclesToXMLFile(vehicles, "vehicles.xml");
            //StoreVehiclesToXMLFile(rentedVehicles, "rentedVehiclesList.xml");
        }

        private void loadFromLocalFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string action = "load vehicles from local file";
            string consequence = "existing vehicles in the program will be removed";
            FormConfirmation formConfirmation = new FormConfirmation(action, consequence);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            vehicles.Clear();
            rentals.Clear();

            List<Vehicle> ImportedVehicles = ReadVehiclesFromXMLFile("vehicles.xml");
            //List<VehicleUserControl> listOfImportedRentedVehicles = ReadVehiclesFromXMLFile("rentedVehiclesList.xml");

            foreach (Vehicle vehicle in ImportedVehicles)
            {
                if (vehicle.Name == "Sedan")
                    vehicles.Add(new Sedan(vehicle));

                if (vehicle.Name == "Minivan")
                    vehicles.Add(new Minivan(vehicle));
            }

            /*
            foreach (VehicleUserControl vehicle in listOfImportedRentedVehicles)
            {
                if (vehicle.Name == "RentedSedanUserControl")
                    rentedVehicles.Add(new RentedSedanUserControl(vehicle));

                if (vehicle.Name == "RentedMinivanUserControl")
                    rentedVehicles.Add(new RentedMinivanUserControl(vehicle));
            }
            */

            //foreach (Vehicle vehicle in rentals)
            {
                //vehicle.configureRentedVehicle(RentVehicleConfiguration.GetRentConfiguration());
                //IDManagement.MarkRentIDAsUnavailable(vehicle.RentID);
            }

            PopulateVehiclesPanel();
            //PopulateRentedVehiclesPanel();
        }

        #endregion

        #region Order logs

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(returnedVehiclesLogManager.Path) || new FileInfo(returnedVehiclesLogManager.Path).Length == 0)
            {
                errorLabel.Text = "There is no log created";
                timerClearErrors.Start();

                return;
            }

            errorLabel.Text = "";
            Process.Start(returnedVehiclesLogManager.Path);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(returnedVehiclesLogManager.Path) || new FileInfo(returnedVehiclesLogManager.Path).Length == 0)
            {
                return;
            }

            string action = "delete the existing log";
            FormConfirmation formConfirmation = new FormConfirmation(action);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            string oldLogManagerPath = returnedVehiclesLogManager.Path;
            returnedVehiclesLogManager = new ReturnedVehiclesLogManager();
            returnedVehiclesLogManager.Path = oldLogManagerPath;

            if (File.Exists(returnedVehiclesLogManager.Path))
            {
                File.Delete(returnedVehiclesLogManager.Path);
            }
        }

        #endregion

        #region Language

        private void languageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLanguages formLanguages = new FormLanguages();

            var result = formLanguages.ShowDialog();
            if (result == DialogResult.OK)
            {
                Language chosenLanguage = formLanguages.ChosenLanguage;
                UpdateLanguage(chosenLanguage);
                Program.Language = chosenLanguage;
            }
        }

        #endregion

        #endregion


        #region SQL Server

        SqlConnection sqlConnection;

        void ConnectToSQL()
        {
            try
            {
                Console.WriteLine("Connecting to SQL SERVER");
                sqlConnection = new SqlConnection(SQLConnectionString());
                sqlConnection.Open();
                Console.WriteLine("Connected!");
                sqlConnection.Close();

                saveToDatabaseToolStripMenuItem.Available = true;
                loadFromDatabaseToolStripMenuItem.Available = true;
                connectToDatabaseToolStripMenuItem.Available = false;
            }
            catch (SqlException s)
            {
                if (s.Number == 40615)
                {
                    errorLabel.Text = "Error 40615" + Environment.NewLine + "This IP isn't allowed to access the database." +
                        Environment.NewLine + "Contact the database owner";
                }
                else
                {
                    errorLabel.Text = "Error " + s.Number.ToString();
                }
                timerClearErrors.Start();
            }
        }

        public string SQLConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "tcp:carrentals.database.windows.net,1433";
            builder.UserID = "mihai";
            builder.Password = "Luca123456789";
            builder.InitialCatalog = "carrentals";

            return builder.ConnectionString;
        }

        void ClearAvailableVehiclesDatabase()
        {
            sqlConnection.Open();

            string query = "DELETE FROM availableVehicles";
            SqlCommand myCommand = new SqlCommand(query, sqlConnection);
            myCommand.ExecuteNonQuery();

            sqlConnection.Close();
        }

        void ClearRentedVehiclesDatabase()
        {
            sqlConnection.Open();

            string query = "DELETE FROM rentedVehicles";
            SqlCommand myCommand = new SqlCommand(query, sqlConnection);
            myCommand.ExecuteNonQuery();

            sqlConnection.Close();
        }

        void SaveAvailableVehicleToSQLDatabase(Vehicle vehicle)
        {
            string vehicleType = "";
            if (vehicle.GetType().Name == "Sedan")
                vehicleType = "sedan";
            if (vehicle.GetType().Name == "Minivan")
                vehicleType = "minivan";

            string query = "INSERT INTO availableVehicles (id, name, type, fuel, damage)";
            query += " VALUES (@id, @name, @type, @fuel, @damage)";

            sqlConnection.Open();

            SqlCommand myCommand = new SqlCommand(query, sqlConnection);
            myCommand.Parameters.AddWithValue("@id", vehicle.ID);
            myCommand.Parameters.AddWithValue("@name", vehicle.VehicleName);
            myCommand.Parameters.AddWithValue("@type", vehicleType);
            myCommand.Parameters.AddWithValue("@fuel", vehicle.FuelPercentage);
            myCommand.Parameters.AddWithValue("@damage", vehicle.DamagePercentage);
            myCommand.ExecuteNonQuery();

            sqlConnection.Close();
        }

        void SaveRentedVehicleToSQLDatabase(Vehicle vehicle)
        {
            string vehicleType = "";
            if (vehicle.GetType().Name == "Sedan")
                vehicleType = "sedan";
            if (vehicle.GetType().Name == "Minivan")
                vehicleType = "minivan";


            string query = "INSERT INTO rentedVehicles (id, name, type, fuel, damage, rentID, ownerName, ownerPhone, returnDate)";
            query += " VALUES (@id, @name, @type, @fuel, @damage, @rentID, @ownerName, @ownerPhone, @returnDate)";

            sqlConnection.Open();

            /*
            SqlCommand myCommand = new SqlCommand(query, sqlConnection);
            myCommand.Parameters.AddWithValue("@id", vehicle.ID);
            myCommand.Parameters.AddWithValue("@name", vehicle.VehicleName);
            myCommand.Parameters.AddWithValue("@type", vehicleType);
            myCommand.Parameters.AddWithValue("@fuel", vehicle.FuelPercentage);
            myCommand.Parameters.AddWithValue("@damage", vehicle.DamagePercentage);
            myCommand.Parameters.AddWithValue("@rentID", vehicle.RentID);
            myCommand.Parameters.AddWithValue("@ownerName", vehicle.Owner.Name);
            myCommand.Parameters.AddWithValue("@ownerPhone", vehicle.Owner.PhoneNumber);
            myCommand.Parameters.AddWithValue("@returnDate", vehicle.ReturnDate.ToShortDateString());
            myCommand.ExecuteNonQuery();
            */

            sqlConnection.Close();
        }

        void GetAvailableVehiclesFromSQLDatabase()
        {
            string sqlQuery = "SELECT id, name, type, fuel, damage FROM availableVehicles";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            try
            {
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                //availableVehicles.Clear();

                while (sqlDataReader.Read())
                {
                    short vehicleID = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("id"));
                    string vehicleName = sqlDataReader["name"].ToString();
                    string vehicleType = sqlDataReader["type"].ToString();
                    short vehicleFuelPercentage = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("fuel"));
                    short vehicleDamagePercentage = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("damage"));

                    if (vehicleType == "sedan")
                    {
                        Sedan sedan = new Sedan(vehicleID, vehicleName, vehicleFuelPercentage, vehicleDamagePercentage);
                        //availableVehicles.Add(sedan);
                    }

                    if (vehicleType == "minivan")
                    {
                        Minivan minivan = new Minivan(vehicleID, vehicleName, vehicleFuelPercentage, vehicleDamagePercentage);
                        //availableVehicles.Add(minivan);
                    }
                }

                sqlDataReader.Close();
                PopulateVehiclesPanel();
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            finally
            {
                sqlConnection.Close();
            }
        }

        void GetRentedVehiclesFromSQLDatabase()
        {
            string sqlQuery = "SELECT id, name, type, fuel, damage, rentID, ownerName, ownerPhone, returnDate FROM rentedVehicles";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            try
            {
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                rentals.Clear();
                while (sqlDataReader.Read())
                {
                    short id = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("id"));
                    string name = sqlDataReader["name"].ToString();
                    string type = sqlDataReader["type"].ToString();
                    short fuel = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("fuel"));
                    short damage = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("damage"));
                    short rentID = sqlDataReader.GetInt16(sqlDataReader.GetOrdinal("rentID"));
                    string returnDateString = sqlDataReader["returnDate"].ToString();
                    DateTime returnDate = DateTime.Parse(returnDateString);

                    string ownerName = sqlDataReader["ownerName"].ToString();
                    string ownerPhone = sqlDataReader["ownerPhone"].ToString();
                    Person owner = new Person(ownerName, ownerPhone);

                    if (type == "sedan")
                    {
                        //RentedSedanUserControl sedan = new RentedSedanUserControl(id, name, fuel, damage, rentID, owner, returnDate);
                        //rentedVehicles.Add(sedan);
                    }

                    if (type == "minivan")
                    {
                        //RentedMinivanUserControl minivan = new RentedMinivanUserControl(id, name, fuel, damage, rentID, owner, returnDate);
                        //rentedVehicles.Add(minivan);
                    }
                }

                sqlDataReader.Close();
                PopulateRentalsPanel();
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            finally
            {
                sqlConnection.Close();
            }
        }

        #endregion


        #region XML save and load

        public void StoreVehiclesToXMLFile(List<Vehicle> vehicles, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Vehicle>));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (FileStream stream = File.OpenWrite(filePath))
            {
                serializer.Serialize(stream, vehicles);
            }
        }

        public List<Vehicle> ReadVehiclesFromXMLFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<Vehicle>();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Vehicle>));
            using (FileStream stream = File.OpenRead(filePath))
            {
                List<Vehicle> deserializedList = (List<Vehicle>)serializer.Deserialize(stream);
                return deserializedList;
            }
        }

        #endregion


        #region Language changing

        void UpdateLanguage(Language language)
        {
            buttonAddAvailableVehicle.Text = language.Translate("Add vehicle");
            buttonSelectAllAvailable.Text = language.Translate("Select all");
            buttonSelectAllRented.Text = language.Translate("Select all");
            buttonRemoveLastAvailableVehicle.Text = language.Translate("Remove last");
            buttonRemoveLastRentedCar.Text = language.Translate("Remove last");
            buttonRemoveSelectedAvailableCars.Text = language.Translate("Remove selected");
            buttonRemoveSelectedRentedCars.Text = language.Translate("Remove selected");
            buttonSortAvailableVehicles.Text = language.Translate("Sort");
            buttonSortRentedVehicles.Text = language.Translate("Sort");

            labelAvailableVehicles.Text = language.Translate("Available cars");
            labelRentedVehicles.Text = language.Translate("Rented cars");

            connectToDatabaseToolStripMenuItem.Text = language.Translate("Connect to database");
            loadFromDatabaseToolStripMenuItem.Text = language.Translate("Load from database");
            saveToDatabaseToolStripMenuItem.Text = language.Translate("Save to database");
            loadFromLocalFileToolStripMenuItem.Text = language.Translate("Load from local file");
            saveToLocalFileToolStripMenuItem.Text = language.Translate("Save to local file");
            orderLogsToolStripMenuItem.Text = language.Translate("Order logs");
            openToolStripMenuItem.Text = language.Translate("Open");
            deleteToolStripMenuItem.Text = language.Translate("Delete");
            languageToolStripMenuItem.Text = language.Translate("Language");

            UpdateLanguageForExistingVehicles(language);
            UpdateLanguageForSortSelections(language);
        }

        void UpdateLanguageForExistingVehicles(Language language)
        {
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.UpdateLanguage(language);
            }
        }

        void UpdateLanguageForExistingRentedVehicles(Language language)
        {
            //foreach (Vehicle vehicle in rentals)
            {
                //vehicle.UpdateLanguage(language);
            }
        }

        void UpdateLanguageForSortSelections(Language language)
        {
            foreach (SortSelectionItem sortSelectionItem in sortAvailableSelectionComboBox.Items)
            {
                sortSelectionItem.UpdateLanguage(language);
            }

            foreach(SortSelectionItem sortSelectionItem in sortRentedSelectionComboBox.Items)
            {
                sortSelectionItem.UpdateLanguage(language);
            }
        }

        #endregion


        #region Vehicle selection / deselection

        #region Select / deselect single vehicle

        public void SelectVehicle(int indexOfVehicle)
        {
            if (!indexesOfSelectedVehicles.Contains(indexOfVehicle))
                indexesOfSelectedVehicles.Add(indexOfVehicle);
        }

        public void DeselectVehicle(int indexOfVehicle)
        {
            indexesOfSelectedVehicles.Remove(indexOfVehicle);
        }

        public void SelectRentedVehicle(int indexOfAvailableVehicle)
        {
            indexesOfSelectedRentals.Add(indexOfAvailableVehicle);
        }

        public void DeselectRentedVehicle(int indexOfAvailableVehicle)
        {
            indexesOfSelectedRentals.Remove(indexOfAvailableVehicle);
        }

        #endregion

        #region Select / deselect all vehicles

        private void SelectAllVehicles(object sender, EventArgs e)
        {
            if (vehicles.Count < 1)
            {
                errorLabel.Text = "There are no available vehicles to select";
                timerClearErrors.Start();
                return;
            }

            bool areAllSelected = true;
            foreach (Vehicle vehicle in vehicles)
            {
                if (!vehicle.Selected)
                {
                    areAllSelected = false;
                }
                vehicle.Selected = true;
            }

            // If all vehicles are already selected, deselect them
            if (areAllSelected)
            {
                foreach (Vehicle vehicle in vehicles)
                    vehicle.Selected = false;
            }
            errorLabel.Text = "";
        }

        private void buttonSelectAllRented_Click(object sender, EventArgs e)
        {
            /*
            if (rentals.Count < 1)
            {
                errorLabel.Text = "There are no rented vehicles to select";
                timerClearErrors.Start();
                return;
            }

            bool areAllSelected = true;
            foreach (Vehicle vehicle in rentals)
            {
                if (!vehicle.Selected)
                {
                    areAllSelected = false;
                }
                vehicle.Selected = true;
            }

            // If all rented vehicles are already selected, deselect them
            if (areAllSelected)
            {
                foreach (Vehicle vehicle in rentals)
                    vehicle.Selected = false;
            }
            errorLabel.Text = "";
            */
        }

        #endregion

        #endregion


        #region Vehicle removal

        private void RemoveLastVehicle(object sender, EventArgs e)
        {
            string action = "remove the last vehicle";
            FormConfirmation formConfirmation = new FormConfirmation(action);

            var result = formConfirmation.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (vehicles.Count < 1)
            {
                errorLabel.Text = "There's nothing" + Environment.NewLine + " to remove";
                timerClearErrors.Stop();
                timerClearErrors.Start();

                return;
            }

            Vehicle lastVehicle = vehicles[vehicles.Count - 1];
            IDManagement.MarkIDAsAvailable(lastVehicle.ID);

            lastVehicle.Selected = false;
            vehicles.Remove(lastVehicle);

            availableCarsElementsPanel.VerticalScroll.Value = 0;
            availableCarsElementsPanel.Controls.Clear();

            foreach (Vehicle vehicle in vehicles)
                availableCarsElementsPanel.Controls.Add(vehicle);

            errorLabel.Text = "";
        }

        private void RemoveSelectedVehicles(object sender, EventArgs e)
        {
            if (indexesOfSelectedVehicles.Count > 0)
            {
                string action = "remove the selected vehicles";
                FormConfirmation formConfirmation = new FormConfirmation(action);

                var result = formConfirmation.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                errorLabel.Text = "";

                // Store the vehicles to be removed in a temporary List
                List<Vehicle> vehiclesToBeRemoved = new List<Vehicle>();
                foreach (int index in indexesOfSelectedVehicles)
                {
                    short idToBeMarkedAsAvailable = vehicles[index].ID;
                    IDManagement.MarkIDAsAvailable(idToBeMarkedAsAvailable);
                    vehiclesToBeRemoved.Add(vehicles.ElementAt(index));
                }

                // Remove the stored vehicles from the vehicles List
                foreach (Vehicle vehicle in vehiclesToBeRemoved)
                {
                    vehicles.Remove(vehicle);
                }

                PopulateVehiclesPanel();
                indexesOfSelectedVehicles.Clear();
            }

            else
            {
                errorLabel.Text = "You didn't select any vehicle to remove";
                timerClearErrors.Stop();
                timerClearErrors.Start();
            }
        }

        private void RemoveLastRentedVehicle(object sender, EventArgs e)
        {

        }

        private void RemoveSelectedRentedVehicles(object sender, EventArgs e)
        {

        }

        #endregion


        #region Transition between available and rented cars zones

        public void RemoveVehicleFromList(Vehicle vehicle)
        {
            vehicles.Remove(vehicle);
            PopulateVehiclesPanel();
        }

        public void ReturnVehicleFromRent(Vehicle vehicle)
        {
            vehicles.Add(vehicle);
            PopulateVehiclesPanel();
        }

        #endregion


        #region Available and rented vehicles list update


        public void PopulateVehiclesPanel()
        {
            availableCarsElementsPanel.Controls.Clear();
            short counter = 0;

            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.LinkToMainWindow(this);
                availableCarsElementsPanel.Controls.Add(vehicle);
                vehicle.Location = new Point(0, counter++ * 100);
            }
        }

        public void PopulateRentalsPanel()
        {
            rentedCarsElementsPanel.Controls.Clear();
            short counter = 0;

            foreach (Rental rental in rentals)
            {
                rental.LinkToMainWindow(this);
                rentedCarsElementsPanel.Controls.Add(rental);
                rental.Location = new Point(0, counter++ * 150);
            }
        }

        #endregion


        #region Sorting
        
        private void SortAvailableVehicles(object sender, EventArgs e)
        {
            
            int sortSelection = ((SortSelectionItem)sortAvailableSelectionComboBox.SelectedItem).Value;

            if (sortSelection == Constants.SORT_BY_VEHICLE_ID)
            {
                vehicles = availableCarsSorter.SortListByID(vehicles);
            }

            if (sortSelection == Constants.SORT_BY_VEHICLE_NAME)
            {
                vehicles = availableCarsSorter.SortListByName(vehicles);
            }

            if (sortSelection == Constants.SORT_BY_VEHICLE_TYPE)
            {
                vehicles = availableCarsSorter.SortListByType(vehicles);
            }

            if (sortSelection == Constants.SORT_BY_VEHICLE_FUEL_PERCENTAGE)
            {
                vehicles = availableCarsSorter.SortListByFuelPercent(vehicles);
            }

            if (sortSelection == Constants.SORT_BY_VEHICLE_DAMAGE_PERCENTAGE)
            {
                vehicles = availableCarsSorter.SortListByDamagePercent(vehicles);
            }

            PopulateVehiclesPanel();
            
        }
        
        


        private void SortRentedVehicles(object sender, EventArgs e)
        {
            int sortSelection = ((SortSelectionItem)sortRentedSelectionComboBox.SelectedItem).Value;

            if (sortSelection == Constants.SORT_BY_VEHICLE_ID)
            {
                //rentedVehicles = rentedCarsSorter.SortListByID(rentedVehicles);
            } 

            if (sortSelection == Constants.SORT_BY_VEHICLE_NAME)
            {
               //rentedVehicles = rentedCarsSorter.SortListByName(rentedVehicles);
            }
            
            if (sortSelection == Constants.SORT_BY_VEHICLE_TYPE)
            {
                //rentedVehicles = rentedCarsSorter.SortListByType(rentedVehicles);
            }
            
            if (sortSelection == Constants.SORT_BY_VEHICLE_OWNER_NAME)
            {
                //rentedVehicles = rentedCarsSorter.SortListByOwnerName(rentedVehicles);
            }
            
            if (sortSelection == Constants.SORT_BY_VEHICLE_OWNER_PHONE_NUMBER)
            {
                //rentedVehicles = rentedCarsSorter.SortListByOwnerPhoneNumber(rentedVehicles);
            }
            
            if (sortSelection == Constants.SORT_BY_VEHICLE_RETURN_DATE)
            {
                //rentedVehicles = rentedCarsSorter.SortListByReturnDate(rentedVehicles);
            }
            
            PopulateRentalsPanel();
        }

        #endregion


        #region Timers

        private void ClearErrorsTick(object sender, EventArgs e)
        {
            errorLabel.Text = "";
            timerClearErrors.Stop();
        }

        private void ProgramDateTick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            programTime = currentTime;

            string currentTimeString = "";
            currentTimeString += currentTime.Day.ToString() + "/";
            currentTimeString += currentTime.Month.ToString() + "/";
            currentTimeString += currentTime.Year.ToString() + " ";
            currentTimeString += currentTime.ToShortTimeString();

            labelProgramDate.Text = currentTimeString;
        }

        #endregion
    }
}
