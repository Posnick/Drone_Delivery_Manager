using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ParcelActions.xaml
    /// </summary>
    public partial class ParcelActions : Window
    {
        private BlApi.IBL BLObject;
        private ParcelToList selecetedParcelToList;

        #region Actions Constructor
        public ParcelActions(ParcelToList selecetedParcelToList)
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
            this.selecetedParcelToList = selecetedParcelToList;

            
            Parcel parcel = BLObject.GetParcelByIdBL(selecetedParcelToList.Id);
            if(parcel.Drone.Id == 0)
            {
                grid4.Visibility = Visibility.Hidden;
            }

            grid1.DataContext = parcel;

            if (parcel.Scheduled == null || parcel.Delivered != null) 
            {
                ViewDroneInParcel.Visibility = Visibility.Hidden;
                ViewReceiverCustomerInParcel.Visibility = Visibility.Hidden;
                ViewSenderCustomerInParcel.Visibility = Visibility.Hidden;
            }

            if (parcel.Scheduled == null) DeleteParcelButton.Visibility = Visibility.Visible;
            AddParcelButton.Visibility = Visibility.Hidden;

            grid4.Visibility = Visibility.Hidden;
        }
        #endregion


        #region Add Constructor
        /// <summary>
        /// constractor.
        /// </summary>
        public ParcelActions()
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

            grid2.Visibility = Visibility.Hidden;
            grid3.Visibility = Visibility.Hidden;


            var customersList = from customer in BLObject.GetAllCustomerToList() select customer.Id;

            if (customersList.Count() > 0)
            {
                ReceiverCustomerIdSelector.ItemsSource = customersList;
                SenderCustomerIdSelector.ItemsSource = customersList;

                ReceiverCustomerIdSelector.SelectedItem = customersList.First();
                SenderCustomerIdSelector.SelectedItem = customersList.First();
            }

            PrioritySelctor.ItemsSource = Enum.GetValues(typeof(Priorities));
            WeightSelctor.ItemsSource = Enum.GetValues(typeof(WeightCategories));

            PrioritySelctor.SelectedItem = (Priorities)0;
            WeightSelctor.SelectedItem= (WeightCategories)0;

        }
        #endregion


        #region Parcel Actions

        /// <summary>
        /// enable to add a parcel.
        /// </summary>
        private void AddParcelButton_Click(object sender, RoutedEventArgs e)
        {
            Parcel newParcle = new();
            int.TryParse(ReceiverCustomerIdSelector.SelectedItem.ToString(), out int receiverId);
            int.TryParse(SenderCustomerIdSelector.SelectedItem.ToString(), out int senderId);

            newParcle.receiverCustomer.Id = receiverId;
            newParcle.senderCustomer.Id = senderId;
            newParcle.Priority = (Priorities)PrioritySelctor.SelectedItem;
            newParcle.Weight = (WeightCategories)WeightSelctor.SelectedItem;

            try
            {
                BLObject.AddNewParcelBL(newParcle);
                MessageBox.Show("Parcel has been added sucssesfuly",
                "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(InvalidInputException)
            {
                MessageBox.Show("Invalid input",
                  "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// showing the drone details that slurred to the parcel.
        /// </summary>
        private void ViewDroneInParcel_Click(object sender, RoutedEventArgs e)
        {
            Parcel parcel = BLObject.GetParcelByIdBL(selecetedParcelToList.Id);
            var droneToList= (from drone in BLObject.GetAllDroneToList() where drone.Id == parcel.Drone.Id select drone).FirstOrDefault();
            new DroneActions(droneToList).Show();
        }



        /// <summary>
        /// showing the receiver details that slurred to the parcel.
        /// </summary>
        private void ViewReceiverCustomerInParcel_Click(object sender, RoutedEventArgs e)
        {
            Parcel parcel = BLObject.GetParcelByIdBL(selecetedParcelToList.Id);
            CustomerToList customerToList = BLObject.GetCustomerToList(parcel.receiverCustomer.Id);
            new CustomerActions(customerToList).Show();
        }


        /// <summary>
        /// showing the customer details that slurred to the parcel.
        /// </summary>
        private void ViewSenderCustomerInParcel_Click(object sender, RoutedEventArgs e)
        {
            Parcel parcel = BLObject.GetParcelByIdBL(selecetedParcelToList.Id);
            CustomerToList customerToList = BLObject.GetCustomerToList(parcel.senderCustomer.Id);
            new CustomerActions(customerToList).Show();
        }

        /// <summary>
        /// enable to delete the parcel that was choesen.
        /// </summary>
        private void DeleteParcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BLObject.DeleteParcel(selecetedParcelToList.Id);
                MessageBox.Show("Parcel has been removed", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(ObjectNotFoundException exception)
            {
                MessageBox.Show(exception.Message,"Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(InvalidOperationException)
            {
                MessageBox.Show("Could not remove this parcel, because it is in delivery"
                    , "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
