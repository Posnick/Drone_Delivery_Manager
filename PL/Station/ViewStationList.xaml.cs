using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for ViewStationList.xaml
    /// </summary>
    public partial class ViewStationList : Window
    {
        private BlApi.IBL BLObject;

        #region Counstructor
        public ViewStationList()
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
            StationListView.ItemsSource = BLObject.GetAllBaseStationsToList();
            DataContext = false;
        }
        #endregion


        #region Station List View
        /// <summary>
        /// showing the details of the ststion that was choesen at station action window.
        /// </summary>
        private void StationListView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (StationListView.SelectedIndex >= 0)
            {
                BO.StationToList selectedStation = BLObject.GetAllBaseStationsToList().ToList()[StationListView.SelectedIndex];
                if (new StationActions(selectedStation).ShowDialog() == false)
                    StationListView.ItemsSource = BLObject.GetAllBaseStationsToList();
            }
        }

        /// <summary>
        /// group by avaible charge slots.
        /// </summary>
        private void GroupByStationListWithAvailableChargingSlots_Click(object sender, RoutedEventArgs e)
        { 
            IEnumerable<IGrouping<int, BO.StationToList>> stationGroup = from station in BLObject.GetStationsWithAvailableChargingSlotstBL()
                               group station by station.AvailableChargeSlots;

            List<BO.StationToList> stationList = new();
            foreach (var group in stationGroup)
            {
                foreach (var station in group)
                {
                    stationList.Add(station);
                }
            }
            StationListView.ItemsSource = stationList;
        }

        #endregion


        #region Add
        /// <summary>
        /// if station action window closed - get the update base station to list.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (new StationActions().ShowDialog() == false)
                StationListView.ItemsSource = BLObject.GetAllBaseStationsToList();
        }
        #endregion


        #region Close Window
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }
        #endregion
    }
}