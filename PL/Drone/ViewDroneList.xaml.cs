using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneList.xaml
    /// </summary>
    public partial class ViewDroneList : Window
    {

        private BlApi.IBL BLObject;
        private CollectionView sourceCollectionView;

        #region Constructor
        public ViewDroneList()
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

            sourceCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(BLObject.GetAllDroneToList());
            DroneListView.ItemsSource = sourceCollectionView;
            DroneStatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatuses));
            DroneWeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }
        #endregion


        #region  Add

        /// <summary>
        /// enable entering to drone action window for adding  a new drone.
        /// </summary>
        private void AddNewDrone_Click(object sender, RoutedEventArgs e)
        {
            if (new DroneActions().ShowDialog() == false)
            {
                DroneListView.ItemsSource = BLObject.GetAllDroneToList();
                sourceCollectionView.Refresh();
                DroneListView.Items.Refresh();
            }
        }
        #endregion


        #region Drone List View

        /// <summary>
        /// showing all the details of the drone that was selected.
        /// </summary>
        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DroneListView.SelectedIndex >= 0)
            {
                DroneToList selectedDrone = (DroneToList)sourceCollectionView.GetItemAt(DroneListView.SelectedIndex);
                if (new DroneActions(selectedDrone).ShowDialog() == false)
                {
                    DroneListView.Items.Refresh();
                    sourceCollectionView.Refresh();
                }
            }
        }



        /// <summary>
        /// filter drones by status.
        /// </summary>
        private void DroneStatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneListView.ItemsSource = BLObject.GetDronesToList(x => x.DroneStatus == (DroneStatuses)DroneStatusSelector.SelectedItem);
        }



        /// <summary>
        /// filter drones by weight.
        /// </summary>
        private void DroneWeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DroneListView.ItemsSource = BLObject.GetDronesToList(x => x.MaxWeight == (WeightCategories)DroneWeightSelector.SelectedItem);
        }


        /// <summary>
        /// showing the regular drone list without sorting.
        /// </summary>
        private void RegulrViewButton_Checked(object sender, RoutedEventArgs e)
        {
            DroneListView.ItemsSource = BLObject.GetAllDroneToList();
        }


        /// <summary>
        ///  arranging the drone list by drone status.
        /// </summary>
        private void GroupViewButton_Checked(object sender, RoutedEventArgs e)
        {
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DroneStatus");
            sourceCollectionView.GroupDescriptions.Add(groupDescription);
            DroneListView.ItemsSource = sourceCollectionView;
        }
        #endregion


        #region Close Window
        /// <summary>
        /// closing of the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            this.Close();
        }



        /// <summary>
        /// Bouns closing of the window.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }
        #endregion
    }
}
