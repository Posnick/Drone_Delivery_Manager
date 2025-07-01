using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationActions.xaml
    /// </summary>
    public partial class StationActions : Window
    {
        private BlApi.IBL BLObject;
        private StationToList selcetedStationToList;

        #region Actions Constructor
        public StationActions(StationToList selectedStationToList)
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

            this.selcetedStationToList = selectedStationToList;
            DataContext = false;


            Station station = BLObject.GetStationByIdBL(selcetedStationToList.Id);
            grid1.DataContext = station;

            StationIdTB.IsEnabled = false;
            StationLatitudeTB.IsEnabled = false;
            StationLongitudeTB.IsEnabled = false;
            StationId.IsEnabled = false;
            StationName.IsEnabled = false;
            StationLatitude.IsEnabled = false;
            StationLongitude.IsEnabled = false;
            AvailableChargeSlots.IsEnabled = false;
            DroneChargesList.IsEnabled = false;

            AddStation.Visibility = Visibility.Hidden;

            DroneChargeListView.ItemsSource = from droneCharge in station.DroneChargesList select droneCharge;
        }
        #endregion


        #region Add Constructor
        /// <summary>
        /// constructor.
        /// </summary>
        public StationActions()
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
            DroneChargesList.Visibility = Visibility.Hidden;
            UpdateStationButton.Visibility = Visibility.Hidden;
            DroneChargeListView.Visibility = Visibility.Hidden;
            DeleteStationButton.Visibility = Visibility.Hidden;
        }
        #endregion


        #region Statioc Constructor
        /// <summary>
        /// update sation's name and number of avaible charge slots.
        /// </summary>
        private void UpdateStationButton_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(AvailableChargeSlotsTB.Text, out int chargeSlots);
            string newName = StationNameTB.Text;
            try
            {
                BLObject.UpdateBaseStationDetailsBL(selcetedStationToList.Id, newName, chargeSlots);
                MessageBox.Show("Station's details has been update sucssesfuly",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// enabeling to add a station.
        /// </summary>
        private void AddStation_Click(object sender, RoutedEventArgs e)
        {
            Station station = new();
            int.TryParse(StationIdTB.Text, out int Id);
            station.Id = Id;

            station.Name = StationNameTB.Text;

            double.TryParse(StationLatitudeTB.Text, out double Latitude);
            station.Location.Latitude = Latitude;

            double.TryParse(StationLatitudeTB.Text, out double Longitude);
            station.Location.Longitude = Longitude;

            int.TryParse(AvailableChargeSlotsTB.Text, out int AvailableChargeSlot);
            station.AvailableChargeSlots = AvailableChargeSlot;

            try
            {
                BLObject.AddNewStationBL(station);
                MessageBox.Show("Station has been added sucssesfuly",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }
            catch (InvalidInputException)
            {
                MessageBox.Show("Invalid input", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// enable to delete the station that was choesen.
        /// </summary>
        private void DeleteStationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.DeleteStation(selcetedStationToList.Id);
                MessageBox.Show("Station has been removed",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseButton_Click(sender, e);
            }
            catch (ObjectNotFoundException exception)
            {
                MessageBox.Show(exception.Message,
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


        #region Drone Charge List View
        /// <summary>
        /// showing the details of the drone that is in maintance status at that station.
        /// </summary>
        private void DroneChargeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DroneToList selectedDrone = BLObject.GetAllDroneToList().ToList()[DroneChargeListView.SelectedIndex];
            if (new DroneActions(selectedDrone).ShowDialog() == false)
            {
                DroneChargeListView.Items.Refresh();
            }
        }


        /// <summary>
        /// if there is an avaible charge slots at the station - enable the abillity to update that through update station button.
        /// </summary>
        private void AvailableChargeSlotsTB_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateStationButton.IsEnabled = true;
        }

        /// <summary>
        /// if there is a change at station's name - enable the abillity to update that through update station button.
        /// </summary>
        private void StationNameTB_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateStationButton.IsEnabled = true;
        }
        #endregion


        #region Station TextBox
        /// <summary>
        /// if the station's id isn't currect - mark it in red.
        /// </summary>
        private void StationIdTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(StationIdTB.Text, out int Id);
            if (Id > 10000 || Id < 1000)
            {
                StationIdTB.BorderBrush = Brushes.Red;
                StationIdTB.Foreground = Brushes.Red;
            }
            else
            {
                StationIdTB.BorderBrush = Brushes.Gray;
                StationIdTB.Foreground = Brushes.Black;
            }
        }



        /// <summary>
        /// limit the input of the station's id, to be only numbers in 0-9 .
        /// </summary>
        private void StationIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        /// <summary>
        /// limit the input of the station's name name, to be only numbers in 0-9 and char of a-z and A-Z.
        /// </summary>
        private void StationNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-z,A-Z,0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion


        #region Close Window
        /// <summary>
        /// enable to close the window.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            this.Close();
        }


        /// <summary>
        /// enable the closing of the window by only the cancle button.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }
        #endregion
    }
}
