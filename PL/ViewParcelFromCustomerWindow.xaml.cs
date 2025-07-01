﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for ViewParcelFromCustomerWindow.xaml
    /// </summary>
    public partial class ViewParcelFromCustomerWindow : Window
    {
        private BlApi.IBL BLObject;
        private string userName;

        #region Constructor
        public ViewParcelFromCustomerWindow(string userName)
        {
            InitializeComponent();
            try
            {
                BLObject = BlApi.BlFactory.GetBl();
                this.userName = userName;
            }
            catch (DalApi.DalConfigException e)
            {
                MessageBox.Show(e.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Customer customer = BLObject.GetCustomerByUserName(userName);

            ParcelListView.ItemsSource = from parcelToList in customer.ParcelFromCustomerList select BLObject.GetParcelToList(parcelToList.Id);
        }
        #endregion


        #region Parcel List View
        /// <summary>
        /// getting all the details of the parcel that was chosen , at parcel action window.
        /// </summary>
        private void ParcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParcelListView.SelectedIndex >= 0)
            {
                ParcelToList selectedParcel = (ParcelToList)ParcelListView.SelectedItem;
                if (new ParcelActions(selectedParcel).ShowDialog() == false)
                {
                    Customer customer = BLObject.GetCustomerByUserName(userName);
                    ParcelListView.ItemsSource = from parcelToList in customer.ParcelFromCustomerList select BLObject.GetParcelToList(parcelToList.Id);
                }

            }
        }
        #endregion
    }
}
