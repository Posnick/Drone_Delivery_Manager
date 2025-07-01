using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    /// <summary>
    /// Parcel priorety categiries.
    /// </summary>
    public enum Priorities
    {
        Regular,
        Fast,
        Emergency
    }

    /// <summary>
    /// Parcel weight categiries.
    /// </summary>
    public enum WeightCategories
    {
        Heavy,
        Intermediate,
        Light
    }

    /// <summary>
    /// Drone statuses.
    /// </summary>
    public enum DroneStatuses
    {
        Available,
        Maintenance,
        Shipment
    }
    /// <summary>
    /// Parcel statuses.
    /// </summary>
    public enum ParcelStatuses
    {
        Requested,
        Scheduled,
        PickedUp,
        Delivered
    }
}
