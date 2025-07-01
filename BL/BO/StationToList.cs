using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class StationToList
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AvailableChargeSlots { get; set; }
        public int NotAvailableChargeSlots { get; set; }

        /// <summary>
        /// Return Describe of StationToList class string.
        /// </summary>
        /// <returns> Describe of StationToList class string </returns>
        public override string ToString()
        {
            return $"Station To List:\n"  +
                    $"Id: {Id}\n" +
                    $"Name: {Name}\n"  +
                    $"Available charge slots: {AvailableChargeSlots}\n"+
                    $"Not available charge slots: {NotAvailableChargeSlots}\n";
        }
    }
}
