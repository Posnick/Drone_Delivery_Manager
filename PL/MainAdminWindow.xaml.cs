using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainAdminWindow : Window
    {
        private BlApi.IBL BLObject;

        #region Constructor
        public MainAdminWindow()
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
        }
        #endregion


        #region View List
        /// <summary>
        /// showing the window of view drone list.
        /// </summary>
        private void ViewDroneList_Click(object sender, RoutedEventArgs e)
        {
            new ViewDroneList().Show();
        }


        /// <summary>
        /// showing the window of view station list.
        /// </summary>
        private void ViewStationList_Click(object sender, RoutedEventArgs e)
        {
            new ViewStationList().Show();
        }



        /// <summary>
        /// showing the window of view customer list.
        /// </summary>
        private void ViewCustomerList_Click(object sender, RoutedEventArgs e)
        {
            new ViewCustomerList().Show();
        }



        /// <summary>
        /// showing the window of view parcel list.
        /// </summary>
        private void ViewParcelList_Click(object sender, RoutedEventArgs e)
        {
            new ViewParcelList().Show();
        }
        #endregion


        #region Close Window
        /// <summary>
        /// closing the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            Close();
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
