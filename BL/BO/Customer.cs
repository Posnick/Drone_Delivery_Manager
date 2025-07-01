using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BO
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }

        public Location Location { get; set; } = new();

        public List<ParcelInCustomer> ParcelFromCustomerList { get; set; } = new();
        public List<ParcelInCustomer> ParcelToCustomerList { get; set; } = new();
        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Return describe of Customer class string.
        /// </summary>
        /// <returns> Describe of Customer class string </returns>
        public override string ToString()
        {
            string stringParcelFromCustomerList = "";

            foreach (var parcelFromCustomer in ParcelFromCustomerList)
            {
                stringParcelFromCustomerList += parcelFromCustomer.ToString();
            }

            string stringParcelToCustomerList = "";

            foreach (var parcelToCustomer in ParcelToCustomerList)
            {
                stringParcelToCustomerList += parcelToCustomer.ToString();
            }

            return $"Customer:\n" +
                $"Id: {Id}\n" +
                $"Name: {Name}\n" +
                $"Phone: {Phone}\n" +
                Location.ToString() +
                $"Parcel From Customer:\n{stringParcelFromCustomerList}\n" +
                $"Parcel To Customer:\n{stringParcelFromCustomerList}\n";
        }
    }
}