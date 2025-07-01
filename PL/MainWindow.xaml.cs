using BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlApi.IBL BLObject;

        #region Constructor
        public MainWindow()
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


            //#region Script
            //string dir = Directory.GetCurrentDirectory();

            //List<DO.Drone> list2 = new(Dal.DalObject.DalObj.GetDroneList());
            //FileStream file2 = new FileStream(dir + @"\Data\Drones.xml", FileMode.Create);
            //XmlSerializer x2 = new XmlSerializer(list2.GetType());
            //x2.Serialize(file2, list2);
            //file2.Close();

            //List<DO.Admin> list3 = new(Dal.DalObject.DalObj.GetAdminsList());
            //FileStream file3 = new FileStream(dir + @"\Data\Admins.xml", FileMode.Create);
            //XmlSerializer x3 = new XmlSerializer(list3.GetType());
            //x3.Serialize(file3, list3);
            //file3.Close();

            //List<DO.Customer> list4 = new(Dal.DalObject.DalObj.GetCustomerList());
            //FileStream file4 = new FileStream(dir + @"\Data\Customers.xml", FileMode.Create);
            //XmlSerializer x4 = new XmlSerializer(list4.GetType());
            //x4.Serialize(file4, list4);
            //file4.Close();

            //List<DO.DroneCharge> list5 = new(Dal.DalObject.DalObj.GetDroneChargeList(x => x.DroneId == x.DroneId));
            //FileStream file5 = new FileStream(dir + @"\Data\DroneCharges.xml", FileMode.Create);
            //XmlSerializer x5 = new XmlSerializer(list5.GetType());
            //x5.Serialize(file5, list5);
            //file5.Close();

            //List<DO.Parcel> list6 = new(Dal.DalObject.DalObj.GetParcelList());
            //FileStream file6 = new FileStream(dir + @"\Data\Parcels.xml", FileMode.Create);
            //XmlSerializer x6 = new XmlSerializer(list6.GetType());
            //x6.Serialize(file6, list6);
            //file6.Close();

            //List<DO.Station> list7 = new(Dal.DalObject.DalObj.GetStations(x => x.Id == x.Id));
            //FileStream file7 = new FileStream(dir + @"\Data\Stations.xml", FileMode.Create);
            //XmlSerializer x7 = new XmlSerializer(list7.GetType());
            //x7.Serialize(file7, list7);
            //file7.Close();
            //#endregion

        }
        #endregion


        #region Login

        /// <summary>
        /// check if admin is register , if not - check if the customer is register, if not - message pop - the user isn't exist.
        /// </summary>
        private void Login()
        {
            if (!BLObject.IsAdminRegistered(UserNameTB.Text, PasswordPB.Password))
            {
                if (!BLObject.IsCustomerRegistered(UserNameTB.Text, PasswordPB.Password))
                {
                    MessageBox.Show("The user is not exsist", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    new MainCustomerWindow(UserNameTB.Text).Show();
                }

            }
            else
            {
                new MainAdminWindow().Show();
            }
        }

        /// <summary>
        /// limit the input of the user name, to be only numbers in 0-9 and char of a-z and A-Z.
        /// </summary>
        private void UserNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-z,A-Z,0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        /// <summary>
        /// match between user choices and userName text box.
        /// </summary>
        private void UserNameTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserNameTB.Text == "User Name") //if the user wants to write somthing in the text box - clear that text box.
            {
                UserNameTB.Clear();
            }
            if (!BLObject.IsAdminExsist(UserNameTB.Text))
            {
                UserNameMessage.Visibility = Visibility.Hidden;
            }
            else if (!BLObject.IsAdminExsist(UserNameTB.Text))
            {
                UserNameMessage.Visibility = Visibility.Hidden;
            }
        }



        /// <summary>
        /// tell what will be in the userName text box in each case that note bellow.
        /// </summary>
        private void UserNameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserNameTB.Text == String.Empty)
            {
                UserNameTB.Text = "User Name";
            }
            if (!BLObject.IsAdminExsist(UserNameTB.Text))
            {
                if (!BLObject.IsCustomerExsist(UserNameTB.Text))
                {
                    UserNameMessage.Visibility = Visibility.Visible; //if the user name isn't an admin or customer - show userName massege.
                }
                else if (BLObject.IsCustomerExsist(UserNameTB.Text))
                {
                    UserNameMessage.Visibility = Visibility.Hidden;
                }
            }
            else if (BLObject.IsAdminExsist(UserNameTB.Text))
            {
                UserNameMessage.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// call for Login function for connection to the main window.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }



        /// <summary>
        /// show the characters of the password.
        /// </summary>
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTB.Text = PasswordPB.Password;
            PasswordPB.Visibility = Visibility.Hidden;
            PasswordTB.Visibility = Visibility.Visible;
        }



        /// <summary>
        /// unshow the characters of the password.
        /// </summary>
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordPB.Password = PasswordTB.Text;
            PasswordTB.Visibility = Visibility.Collapsed;
            PasswordPB.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// enable entering with pressing the enter key, and show password also.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
            if (PasswordPB.Password != String.Empty)
                revealModeCheckBox.Visibility = Visibility.Visible;
        }
        #endregion


        #region Sing Up
        /// <summary>
        /// enable customer window.
        /// </summary>
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            new SignUpCustomerWindow().Show();
        }
        #endregion


        #region Close Window
        /// <summary>
        /// 
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;

            this.Close();
        }

        /// <summary>
        /// when this window closed - realese all drones from charging at droneCharges list.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            DalApi.DalFactory.GetDal().ReleseDroneCharges();
        }
        #endregion

    }
}