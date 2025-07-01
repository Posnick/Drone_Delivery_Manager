using System.Linq;
using System.Windows;
using System.Windows.Input;
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerActions.xaml
    /// </summary>
    public partial class CustomerActions : Window
    {
        private BlApi.IBL BLObject;
        private CustomerToList selecetedCustomerToList;


        #region Actions Constructor
        /// <summary>
        /// constractor
        /// </summary>
        /// <param name="selcetedCustomerToList">the customer that was chosen from the list view at view customer list window </param>
        public CustomerActions(CustomerToList selcetedCustomerToList)
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
            this.selecetedCustomerToList = selcetedCustomerToList;

            Customer customer = BLObject.GetCustomerByIdBL(selcetedCustomerToList.Id);

            DataContext = false;
            grid1.DataContext = customer;

            CustomerId.IsEnabled = false;
            CustomerIdTB.IsEnabled = false;

            CustomerLatitude.IsEnabled = false;
            CustomerLatitudeTB.IsEnabled = false;

            CustomerLongitude.IsEnabled = false;
            CustomerLongitudeTB.IsEnabled = false;
            UserName.IsEnabled = false;
            UserNameTB.IsEnabled = false;

            Password.IsEnabled = false;
            PasswordTB.IsEnabled = false;

            AddCustomerBustton.Visibility = Visibility.Hidden;

            ParcelFromCustomerList.ItemsSource = customer.ParcelFromCustomerList;
            ParcelToCustomerList.ItemsSource = customer.ParcelToCustomerList;
        }

        #endregion


        #region Add Consturctor
        /// <summary>
        /// constructor
        /// </summary>
        public CustomerActions()
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

            grid3.Visibility = Visibility.Hidden;
            grid4.Visibility = Visibility.Hidden;
        }
        #endregion


        #region Add Customer
        /// <summary>
        /// adding a customer.
        /// </summary>
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Customer has been added sucssesfuly",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseButton_Click(sender, e);
            }
            catch(InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(ObjectAlreadyExistException)
            {
                MessageBox.Show("Customer already exist", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


        #region Update
        /// <summary>
        /// update customer details.
        /// </summary>
        private void UpdateCustomerDetailes_Click(object sender, RoutedEventArgs e)
        {
            string newName = CustomerNameTB.Text;
            string newPhone = CustomerPhoneTB.Text;
            try
            {
                BLObject.UpdateCustomerDetailesBL(selecetedCustomerToList.Id, newName, newPhone);
                MessageBox.Show("Customer has been added sucssesfuly",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidInputException exception)
            {
                MessageBox.Show(exception.Message, "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


        #region Close Window
        /// <summary>
        /// function that enable closing of the window.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true; //enable closing window.
            this.Close();
        }



        /// <summary>
        ///  enable the closing of the window by only the cancle button.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }

        #endregion


        #region Show Parcel From Customer
        /// <summary>
        /// finding the parcel that the customer send, and sending it to the parcel action window.
        /// </summary>
        private void ParcelFromCustomerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Customer customer = BLObject.GetCustomerByIdBL(selecetedCustomerToList.Id);
            ParcelToList selectedParcel = BLObject.GetParcelToList(customer.ParcelFromCustomerList[ParcelFromCustomerList.SelectedIndex].Id);
            new ParcelActions(selectedParcel).Show();
        }

        #endregion


        #region Show Parcel To Customer
        /// <summary>
        /// finding the parcel that the customer needs to get, and sending it to the parcel action window.
        /// </summary>
        private void ParcelToCustomerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Customer customer = BLObject.GetCustomerByIdBL(selecetedCustomerToList.Id);
            ParcelToList selectedParcel = BLObject.GetParcelToList(customer.ParcelToCustomerList[ParcelToCustomerList.SelectedIndex].Id);
            new ParcelActions(selectedParcel).Show();
        }
        #endregion


        #region Delete
        /// <summary>
        /// deleting the customer that was choesen , from customer to list at BL.
        /// </summary>
        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.DeleteCustomer(selecetedCustomerToList.Id);
                MessageBox.Show("Customer has been removed",
                                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                this.CloseButton_Click(sender, e);
            }
            catch (ObjectNotFoundException exception)
            {
                MessageBox.Show(exception.Message,
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message,
                                "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Customer TextBox
        /// <summary>
        /// enabling writing in customer name text box.
        /// </summary>
        private void CustomerNameTB_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateCustomerDetailes.IsEnabled = true;
        }

        /// <summary>
        /// enabling writing in customer phone text box.
        /// </summary>
        private void CustomerPhoneTB_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateCustomerDetailes.IsEnabled = true;
        }
        #endregion
    }
}
