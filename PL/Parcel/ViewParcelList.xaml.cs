using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ViewParcelList.xaml
    /// </summary>
    public partial class ViewParcelList : Window
    {
        private BlApi.IBL BLObject;
        public ViewParcelList()
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
            ParcelListView.ItemsSource = BLObject.GetAllParcelToList();
        }



        /// <summary>
        /// group by parcel's receiver name.
        /// </summary>
        private void ViewReceivedParcelsList_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<string , ParcelToList>> parcelGroup = from parcel in BLObject.GetAllParcelToList() group parcel by parcel.ReceiverName;
            List<ParcelToList> parcelList = new();

            foreach (IGrouping<string, ParcelToList> group in parcelGroup)
            {
                foreach (var parcel in group)
                {
                    parcelList.Add(parcel);
                }
            }

            ParcelListView.ItemsSource = parcelList;

        }



        /// <summary>
        ///  group by parcel's sender name.
        /// </summary>
        private void ViewSenderParcelList_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IGrouping<string, ParcelToList>> parcelGroup = from parcel in BLObject.GetAllParcelToList() group parcel by parcel.SenderName;
            List<ParcelToList> parcelList = new();

            foreach (var group in parcelGroup)
            {
                foreach (var parcel in group)
                {
                    parcelList.Add(parcel);
                }
            }
            ParcelListView.ItemsSource = parcelList;
        }



        /// <summary>
        /// getting all the parcels that it's requested date is between yhe dates that wase choesen.
        /// </summary>
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parcelList = from parcel in BLObject.GetAllParcels()
                                 where FirstDate.SelectedDate.Value.Date.CompareTo(parcel.Requested) <= 0 &&
                                        LastDate.SelectedDate.Value.Date.CompareTo(parcel.Requested) >= 0
                                 select parcel;

                List<ParcelToList> parcels = new();
                foreach (var parcel in parcelList)
                {
                    parcels.Add(BLObject.GetParcelToList(parcel.Id));
                }

                ParcelListView.ItemsSource = parcels;
            }
            catch(InvalidOperationException)
            {
                MessageBox.Show("Please select a date",
                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// getting all the details of the parcel that was chosen , at parcel action window.
        /// </summary>
        private void ParcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParcelListView.SelectedIndex >= 0)
            {
                ParcelToList selectedParcel = (ParcelToList)ParcelListView.SelectedItem;
                if (new ParcelActions(selectedParcel).ShowDialog() == false)
                    ParcelListView.ItemsSource = BLObject.GetAllParcelToList();
            }
        }



        /// <summary>
        /// enable to adding a parcel at parcel action window.
        /// </summary>
        private void AddParcel_Click(object sender, RoutedEventArgs e)
        {
            if (new ParcelActions().ShowDialog() == false) 
                ParcelListView.ItemsSource = BLObject.GetAllParcelToList();
        }

        private void ViewParcelStatus_Click(object sender, RoutedEventArgs e)
        {
            var parcelList = from parcel in BLObject.GetAllParcelToList()
                             orderby parcel.ParcelStatus
                             select parcel;
            ParcelListView.ItemsSource = parcelList;
        }
    }
}
