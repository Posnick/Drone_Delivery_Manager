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
    public partial class SignUpCustomerWindow : Window
    {
        private BlApi.IBL BLObject;

        #region Constructor
        public SignUpCustomerWindow()
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
           
        }
        #endregion


        #region Close Window
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }
        #endregion


        #region Sign Up
        private void AddCustomerBustton_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new();

            int.TryParse(CustomerIdTB.Text, out int Id);
            customer.Id = Id;
            customer.Name = CustomerNameTB.Text;
            customer.Phone = CustomerPhoneTB.Text;
            double.TryParse(CustomerLatitudeTB.Text, out double Latitude);
            customer.Location.Latitude = Latitude;

            double.TryParse(CustomerLongitudeTB.Text, out double Longitude);
            customer.Location.Longitude = Longitude;
            customer.UserName = UserNameTB.Text;
            customer.Password = PasswordTB.Text;

            try
            {
                BLObject.AddNewCustomerBL(customer);
                MessageBox.Show("You have been registered successfully",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseButton_Click(sender, e);
                new MainCustomerWindow(customer.UserName).Show();
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectAlreadyExistException)
            {
                MessageBox.Show("Customer already exist", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
