using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using BO;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneActions.xaml
    /// </summary>
    public partial class DroneActions : Window
    {
        private BlApi.IBL BLObject;
        private DroneToList selectedDroneToList;
        BackgroundWorker backgroundWorker = new BackgroundWorker();


        #region Drone Actions Constructor 
        /// <summary>
        /// Drone actions c-tor.
        /// </summary>
        /// <param name="droneToList">drone from drone to list</param>
        public DroneActions(DroneToList droneToList)
        {
            InitializeComponent();
            try
            {
                BLObject = BlApi.BlFactory.GetBl();
            }
            catch (DalApi.DalConfigException e)
            {
                MessageBox.Show(e.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                Drone drone = BLObject.GetDroneByIdBL(droneToList.Id);
                this.selectedDroneToList = droneToList;

                DataContext = false;
                grid1.DataContext = drone;

                IdTextBox.IsEnabled = false;
                ModelTextBox.IsEnabled = false;
                BatteryTB.IsEnabled = false;
                MaxWeightTB.IsEnabled = false;
                StatusTB.IsEnabled = false;
                DeliveryTB.IsEnabled = false;
                LocationTB.IsEnabled = false;

                LocationTB.Text = drone.CurrentLocation.ToString();
                DeliveryTB.Text = drone.ParcelInDelivery.ToString();

                if (droneToList.DroneStatus == DroneStatuses.Maintenance)
                {
                    DroneCharge droneCharge = BLObject.FindDroneChargeByDroneIdBL(droneToList.Id);
                    Pb.Value = BLObject.BatteryCalac(droneToList, droneCharge);
                }
                else
                {
                    Pb.Value = droneToList.BatteryStatus;
                }
                progressBarColor();

                grid3.Visibility = Visibility.Hidden;

                if (selectedDroneToList.DeliveryParcelId <= 0)
                    ViewParcelButton.Visibility = Visibility.Hidden;

                if (selectedDroneToList.DeliveryParcelId == 0)
                {
                    DeliveryTB.Visibility = Visibility.Hidden;
                    ParcelInDelivery.Visibility = Visibility.Hidden;
                }
            }
            catch (ObjectNotFoundException exception)
            {
                MessageBox.Show(exception.Message,
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


        #region Add Constructor
        /// <summary>
        /// Add new Drone c-tor.
        /// </summary>
        public DroneActions()
        {
            InitializeComponent();
            try
            {
                BLObject = BlApi.BlFactory.GetBl();
            }
            catch (DalApi.DalConfigException e)
            {
                MessageBox.Show(e.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DataContext = false;

            DeleteDroneButton.Visibility = Visibility.Hidden;
            SimulatorButton.Visibility = Visibility.Hidden;
            grid2.Visibility = Visibility.Hidden;
            grid4.Visibility = Visibility.Hidden;
            

            AddButton.IsEnabled = false;

            var stationsIdList = from stationToList in BLObject.GetStationsWithAvailableChargingSlotstBL() select stationToList.Id;
            BaseStationCB.ItemsSource = stationsIdList;
            BaseStationCB.SelectedItem = stationsIdList.First();

            MaxWeightCB.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            MaxWeightCB.SelectedItem = (WeightCategories)0;
        }
        #endregion


        #region Drone TextBox

        /// <summary>
        /// Allows cleaning of the text box while clicking on it.
        /// </summary>
        private void DroneIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IdTextBox.Text == "Id")
            {
                IdTextBox.Clear();
            }
        }

        /// <summary>
        /// Allows several cases of the text box while unclicking on it.
        /// </summary>
        private void DroneIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdTextBox.Text == String.Empty)
                IdTextBox.Text = "Id";

            else if (ModelTextBox.Text != "Model")
                AddButton.IsEnabled = true;

            //***Bouns.***

            if (IdTextBox.Text != "Id")
            {
                int.TryParse(IdTextBox.Text, out int Id);
                if (Id > 10000 || Id < 1000)
                {
                    IdTextBox.BorderBrush = Brushes.Red;
                    IdTextBox.Foreground = Brushes.Red;
                }
            }

        }

        /// <summary>
        /// limit the chars at the text box of drone's id to be only numbers
        /// </summary>
        private void DroneIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        ///  limit the chars at the text box of drone's model to be only a-z,A-Z,0-9
        /// </summary>
        private void ModelIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-z,A-Z,0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Allows cleaning of the text box of drone's model while clicking on it.
        /// </summary>
        private void ModelTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ModelTextBox.Text == "Model")
                ModelTextBox.Clear();
        }

        /// <summary>
        /// Allows several cases of the text box while unclicking on it.
        /// </summary>
        private void ModelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ModelTextBox.Text == String.Empty)
                ModelTextBox.Text = "Model";
            else if (IdTextBox.Text != "Id")
                AddButton.IsEnabled = true;
        }

        /// <summary>
        /// Bouns if the drone's id isn't correct - mart it in red.
        /// </summary>
        private void IdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IdTextBox.Text != "Id")
            {
                int.TryParse(IdTextBox.Text, out int Id);
                if (Id > 10000 || Id < 1000)
                {
                    IdTextBox.BorderBrush = Brushes.Red;
                    IdTextBox.Foreground = Brushes.Red;
                }
                else
                {
                    IdTextBox.BorderBrush = Brushes.Gray;
                    IdTextBox.Foreground = Brushes.Black;
                }
            }
        }
        #endregion


        #region Drone Actions
        /// <summary>
        /// getting all the details of the drone that was choesen, from drone to list at BL.
        /// </summary>
        /// <param name="droneId">property of drone - ID</param>
        private void GetDroneFields(int droneId)
        {

            Drone drone = BLObject.GetDroneByIdBL(droneId);

            IdTextBox.Text = drone.Id.ToString();
            ModelTextBox.Text = drone.Model;
            BatteryTB.Text = String.Format("{0:0.000}%", drone.BatteryStatus);
            Pb.Value = drone.BatteryStatus;
            progressBarColor();
            MaxWeightTB.Text = drone.MaxWeight.ToString();
            StatusTB.Text = drone.DroneStatus.ToString();
            DeliveryTB.Text = drone.ParcelInDelivery.ToString();
            LocationTB.Text = drone.CurrentLocation.ToString();
        }
        /// <summary>
        /// opening update drone's model winndow.
        /// </summary>
        private void UpdateDroneModel_Click(object sender, RoutedEventArgs e)
        {
            if (new UpdateDroneModel(selectedDroneToList.Id).ShowDialog() == false)
            {
                ModelTextBox.Text = selectedDroneToList.Model;
            }
        }

        /// <summary>
        /// sending the drone that was selected to charging.
        /// </summary>
        private void UpdateDroneToChargingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.UpdateDroneToChargingBL(selectedDroneToList.Id);
                MessageBox.Show("Drone has been update to charging sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(selectedDroneToList.Id);
            }
            catch (InvalidInputException)
            {
                MessageBox.Show("Invalid input",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectAlreadyExistException)
            {
                MessageBox.Show("Drone is already in charging",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OutOfBatteryException)
            {
                MessageBox.Show("Could not send the drone to charging because there is no enough battery",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectNotFoundException)
            {
                MessageBox.Show("Could not update drone to charging",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// release  the drone from charging.
        /// </summary>
        private void UpdateDroneFromChargingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.UpdateDroneFromChargingBL(selectedDroneToList.Id);
                MessageBox.Show("Drone has been updated sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(selectedDroneToList.Id);
            }
            catch (ObjectNotFoundException)
            {
                MessageBox.Show("Could not update drone from charging",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidInputException)
            {
                MessageBox.Show("Invalid input",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotValidRequestException)
            {
                MessageBox.Show("Could not update drone status because the drone is not in maintenance status",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Associate drone to parcel.
        /// </summary>
        private void SendDroneToDelivery_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.AssociateDroneTofParcelBL(selectedDroneToList.Id);

                MessageBox.Show("Drone was sent sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(selectedDroneToList.Id);
                DeliveryTB.Visibility = Visibility.Visible;
                ParcelInDelivery.Visibility = Visibility.Visible;
            }
            catch (InvalidInputException)
            {
                MessageBox.Show("Invalid input",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException exeption)
            {
                MessageBox.Show(exeption.Message,
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectNotFoundException)
            {
                MessageBox.Show("There is no parcels to delivered",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OutOfBatteryException)
            {
                MessageBox.Show("Can not send drone to delivery, beacuse there is not enough battery",
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NoParcelsMatchToDroneException)
            {
                MessageBox.Show("There is no parcels this drone can delivered",
                                 "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// updating that he drone took the parcel, and the parcel's status is deliverd.
        /// </summary>
        private void UpdateParcelStatusToDelivered_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.UpdateDeliveredParcelByDroneIdBL(selectedDroneToList.Id);
                MessageBox.Show("Parcel status has been updated to delivered sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(selectedDroneToList.Id);
            }
            catch (InvalidInputException exeption)
            {
                MessageBox.Show(exeption.Message,
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotValidRequestException exeption)
            {
                MessageBox.Show(exeption.Message,
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// updating that he drone took the parcel, and the parcel's status is picked up.
        /// </summary>
        private void UpdateParcelToPickedUp_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.UpdatePickedUpParcelByDroneIdBL(selectedDroneToList.Id);
                MessageBox.Show("Parcel status updated sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(selectedDroneToList.Id);
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message,
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotValidRequestException exception)
            {
                MessageBox.Show(exception.Message,
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectNotFoundException)
            {
                MessageBox.Show("No parcel was found in this drone",
                            "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// finding the parcel that was choesen, ansd showing all it's details at parcel action window.
        /// </summary>
        private void ViewParcel_Click(object sender, RoutedEventArgs e)
        {
            ParcelToList parcelToList = BLObject.GetParcelToList(selectedDroneToList.DeliveryParcelId);
            new ParcelActions(parcelToList).Show();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int Id;
            int.TryParse(IdTextBox.Text, out Id);
            String Model = ModelTextBox.Text;
            Drone newDrone = new();
            newDrone.Id = Id;
            newDrone.Model = Model;
            newDrone.MaxWeight = (WeightCategories)MaxWeightCB.SelectedItem;

            try
            {
                BLObject.AddNewDroneBL(newDrone, (int)BaseStationCB.SelectedItem);
                MessageBox.Show("Drone has been added sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                GetDroneFields(Id);
                this.Close_Click(sender, e);
            }
            catch (InvalidInputException)
            {
                MessageBox.Show("Invalid input",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectAlreadyExistException)
            {
                MessageBox.Show("Drone is already exist",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// delete the drone and closing drone action windoe Automatically.
        /// </summary>
        private void DeleteDroneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.DeleteDrone(selectedDroneToList.Id);
                MessageBox.Show("Drone has been removed",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);

                Close_Click(sender, e);
            }
            catch (ObjectNotFoundException exception)
            {
                MessageBox.Show(exception.Message,
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Drone could not be deleted because the drone is in shipment",
                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Simulator

        private void SimulatorButton_Click(object sender, RoutedEventArgs e)
        {
            SimulatorButton.Visibility = Visibility.Hidden;
            ManualButton.Visibility = Visibility.Visible;
            grid4.Visibility=Visibility.Hidden;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;


            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BLObject.StartSimulator(selectedDroneToList.Id,
                                        UpdateAction,
                                        () => { return backgroundWorker.CancellationPending; });
            }
            catch (ObjectNotFoundException)
            {
                MessageBox.Show("There is no parcels to delivered",
                    "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(NoParcelsMatchToDroneException)
            {
                MessageBox.Show("There is no parcels this drone can delivered",
                                 "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Drone drone = BLObject.GetDroneByIdBL(selectedDroneToList.Id);
            StatusTB.Text = drone.DroneStatus.ToString();
            LocationTB.Text = drone.CurrentLocation.ToString();

            DeliveryTB.Text = drone.ParcelInDelivery.ToString();
            if (selectedDroneToList.DeliveryParcelId != 0)
            {
                DeliveryTB.Visibility = Visibility.Visible;
                ParcelInDelivery.Visibility = Visibility.Visible;
            }

            if (selectedDroneToList.DroneStatus == DroneStatuses.Maintenance)
            {
                DeliveryTB.Visibility = Visibility.Hidden;
                ParcelInDelivery.Visibility = Visibility.Hidden;

                double batteryStatus;
                DroneCharge droneCharge = BLObject.FindDroneChargeByDroneIdBL(drone.Id);
                batteryStatus = BLObject.BatteryCalac(selectedDroneToList, droneCharge);
                BatteryTB.Text = String.Format("{0:0.000}%", batteryStatus);
                Pb.Value = batteryStatus;
                progressBarColor();
            }
            else
            {
                BatteryTB.Text = String.Format("{0:0.000}%", drone.BatteryStatus);
                Pb.Value = drone.BatteryStatus;
                progressBarColor();
            }
        }

        private void UpdateAction()
        {
            backgroundWorker.ReportProgress(0);
        }
        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
            ManualButton.Visibility =Visibility.Hidden;
            SimulatorButton.Visibility = Visibility.Visible;
            grid4.Visibility = Visibility.Visible;
        }
        private void progressBarColor()
        {
            if (Pb.Value <= 33) Pb.Foreground = Brushes.Red;
            else if (Pb.Value <= 66) Pb.Foreground = Brushes.Yellow;
            else Pb.Foreground = Brushes.LimeGreen;
        }
        #endregion


        #region Close Window
        /// <summary>
        /// Bouns enable to closing the window with onle the cancle button.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }



        /// <summary>
        /// closing of the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
            this.Close();
        }

        #endregion
    }
}