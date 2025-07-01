using System;
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
    /// Interaction logic for MainCustomerWindow.xaml
    /// </summary>
    public partial class MainCustomerWindow : Window
    {
        private BlApi.IBL BLObject;
        string userName;

        #region Constructor
        public MainCustomerWindow(string userName)
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
        }
        #endregion


        #region Add
        private void AddParcelFromCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            new AddParcelFromCustomerWindow(userName).Show();
        }
        #endregion


        #region View Parcel
        private void ViewParcelFromCustomer_Click(object sender, RoutedEventArgs e)
        {
            new ViewParcelFromCustomerWindow(userName).Show();
        }
        #endregion
    }
}
