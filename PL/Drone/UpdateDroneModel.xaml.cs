using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace PL
{
    /// <summary>
    /// Interaction logic for UpdateDroneWindow.xaml
    /// </summary>
    public partial class UpdateDroneModel : Window
    {
        private BlApi.IBL BLObject;
        private int droneId;

        #region Constructor
        public UpdateDroneModel(int droneId)
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
            this.droneId = droneId;

            UpdateButton.IsEnabled = false;
            DataContext = false;
        }
        #endregion


        #region Model TextBox
        /// <summary>
        /// match between user choices and userName text box.
        /// </summary>
        private void ModelTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ModelTextBox.Text == "Enter Model")
                ModelTextBox.Clear();

        }


        /// <summary>
        /// match between user choices and userName text box.
        /// </summary>
        private void ModelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ModelTextBox.Text == String.Empty)
                ModelTextBox.Text = "Enter Model";

        }


        /// <summary>
        /// limit the input of the user name, to be only numbers in 0-9 and char of a-z and A-Z.
        /// </summary>
        private void ModelIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-z,A-Z,0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// if the user wrote somthing at drone's model text box - enable update button.
        /// </summary>
        private void ModelTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModelTextBox.Text != String.Empty) UpdateButton.IsEnabled = true;
            else UpdateButton.IsEnabled = false;
        }
        #endregion


        #region Update Model
        /// <summary>
        /// update drone's model and closing the window Automatically.
        /// </summary>
        private void UpdateDroneModelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelTextBox.Text !=String.Empty)
            {

                String Model = ModelTextBox.Text;
                try
                {
                    BLObject.UpdateDroneModelBL(droneId, Model);
                    MessageBox.Show("Drone has been update sucssesfuly",
                                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (InvalidInputException)
                {
                    MessageBox.Show("Invalid input", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Enter Model to update", "Operation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        #endregion


        #region CLose Window

        /// <summary>
        /// closing the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataContext = true;
            this.Close();
        }

        /// <summary>
        /// Bouns enable to closing the window with onle the cancle button.
        /// </summary>

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext.Equals(false)) e.Cancel = true;
        }
        #endregion
    }
}
