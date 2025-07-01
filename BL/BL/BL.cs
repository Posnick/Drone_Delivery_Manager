﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using BO;
using DalApi;

namespace BL
{
    /// <summary>
    /// The Bissnes Layer wich responsible for the logic in the project refer to the Layer Model.
    /// </summary>
    sealed public partial class BL : BlApi.IBL
    {

        #region Singelton
        //Singelton implementation.
        private static class LoadBlObj
        {
            internal static readonly BL blObject = new BL();
        }

        public static BL BlObj { get { return LoadBlObj.blObject; } }
        #endregion


        #region Constuctor
        List<DroneToList> droneToLists = new();

        //Create instance of dalObject for reference to DAL.
        internal static IDal dalObject;

        /// <summary>
        /// C-tor of BL.
        /// Initiate DroneToList list.
        /// </summary>
        /// <exception cref="NoBaseStationToAssociateDroneToException"> Thrown if there are no stations 
        ///                                                             with available charge-slots </exception>
        private BL()
        {
            try
            {
                dalObject = DalFactory.GetDal();
            }
            catch (DalConfigException)
            {
                throw;
            }
            Random r = new();
            //Import from DAL the 4 weight categories and the charging rate in seperate varibales.
            double[] tempArray;
            lock (dalObject)
            {
                tempArray = dalObject.ElectricityUseRequest();
            }

            double dorneChargingRate = tempArray[4];
            double[] electricityUse = new double[4];

            for (int i = 0; i < tempArray.Length - 1; i++)
            {
                electricityUse[i] = tempArray[i];
            }

            //Initialize all the drones (if any).
            IEnumerable<DO.Drone> dalDronesList = dalObject.GetDroneList();
            if (dalDronesList.Count() > 0)
            {
                foreach (var drone in dalDronesList)
                {
                    DroneToList newDrone = new();
                    //Take all the information from the drone entity in DAL and set it in the DroneToList object (newDrone).
                    newDrone.Id = drone.Id;
                    newDrone.Model = drone.Model;
                    newDrone.MaxWeight = (WeightCategories)drone.MaxWeight;

                    //Get the rest of the information and fill the other fileds in the new DroneToList object.
                    //Set DroneStatus, DeliveryParcelId and CurrentLocation (and BatteryStatus - if needed) fileds.
                    bool isInShipment = false;
                    foreach (var parcel in dalObject.GetParcelList())
                    {
                        if (parcel.DroneId == newDrone.Id && parcel.Delivered == null)
                        {
                            newDrone.DroneStatus = DroneStatuses.Shipment;
                            newDrone.DeliveryParcelId = parcel.Id;

                            if (parcel.PickedUp == null)
                            {
                                try
                                {
                                    int baseStationId = FindNearestBaseStationByCustomerId(parcel.SenderId);

                                    double baseStationLatitude = dalObject.GetStationById(baseStationId).Latitude;
                                    double baseStationLongitude = dalObject.GetStationById(baseStationId).Longitude;

                                    //Update the location coordinates of the droneToList same as the closest base station to the sender location.
                                    newDrone.CurrentLocation.Latitude = baseStationLatitude;
                                    newDrone.CurrentLocation.Longitude = baseStationLongitude;
                                }
                                catch (ObjectNotFoundException)
                                {
                                    throw new NoBaseStationToAssociateDroneToException();
                                }
                            }
                            else
                            {
                                double senderLatitude = dalObject.GetCustomerById(parcel.SenderId).Latitude;
                                double senderLongitude = dalObject.GetCustomerById(parcel.SenderId).Longitude;

                                //Update the location coordinates of the droneToList same as the sender location.
                                newDrone.CurrentLocation.Latitude = senderLatitude;
                                newDrone.CurrentLocation.Longitude = senderLongitude;

                            }

                            double minimumOfBattery = FindMinPowerSuply(newDrone, parcel.TargetId);
                            newDrone.BatteryStatus = r.Next((int)minimumOfBattery, 101);
                            isInShipment = true;
                        }
                    }
                    if (!isInShipment)
                    {
                        newDrone.DroneStatus = (DroneStatuses)r.Next(2);
                    }

                    if (newDrone.DroneStatus == DroneStatuses.Maintenance)
                    {
                        List<DO.Station> stationsList = new(dalObject.GetBaseStationList());
                        int size = stationsList.Count();

                        int index = r.Next(0, size);
                        DO.Station station = stationsList[index];

                        newDrone.CurrentLocation.Latitude = station.Latitude;
                        newDrone.CurrentLocation.Longitude = station.Longitude;
                        newDrone.BatteryStatus = r.Next(0, 21);
                        try
                        {
                            dalObject.AddDroneCharge(newDrone.Id, station.Id);
                        }
                        catch (DO.XMLFileLoadCreateException e)
                        {
                            throw new XMLFileLoadCreateException(e.Message);
                        }
                    }
                    else if (newDrone.DroneStatus == DroneStatuses.Available)
                    {
                        List<int> CustomerIdList = new();
                        CustomerIdList.AddRange(from parcel in dalObject.GetParcelList()
                                                where parcel.Delivered != null
                                                select parcel.TargetId);
                        int size = CustomerIdList.Count();
                        if (size > 0)
                        {
                            int index = r.Next(0, size);
                            int targetId = CustomerIdList[index];

                            DO.Customer target = dalObject.GetCustomerById(targetId);
                            newDrone.CurrentLocation.Latitude = target.Latitude;
                            newDrone.CurrentLocation.Longitude = target.Longitude;

                            newDrone.BatteryStatus = r.Next((int)FindMinPowerSuplyForCharging(newDrone), 101);
                        }
                        else
                        {
                            try
                            {
                                StationToList baseStation = GetStationsWithAvailableChargingSlotstBL().First();
                                DO.Station newStation = dalObject.GetStationById(baseStation.Id);

                                newDrone.CurrentLocation.Longitude = newStation.Longitude;
                                newDrone.CurrentLocation.Latitude = newStation.Latitude;
                                newDrone.BatteryStatus = 100;

                                baseStation.AvailableChargeSlots--;
                                baseStation.NotAvailableChargeSlots++;
                                try
                                {
                                    lock (dalObject)
                                    {
                                        dalObject.UpdateDroneToCharging(newDrone.Id, baseStation.Id);
                                    }
                                }
                                catch (DO.ObjectNotFoundException e)
                                {
                                    throw new ObjectNotFoundException(e.Message);
                                }
                                catch (DO.XMLFileLoadCreateException e)
                                {
                                    throw new XMLFileLoadCreateException(e.Message);
                                }

                            }
                            catch (InvalidOperationException)
                            {
                                throw new NoBaseStationToAssociateDroneToException();
                            }
                        }
                    }
                    droneToLists.Add(newDrone);
                }
            }

        }
        #endregion


        #region Find

        /// <summary>
        /// Find the nearest base-station to the customer by customer id.
        /// </summary>
        /// <param name="customerId">Customer Id </param>
        /// <returns> Id of the nearest base-station to the recieven coustomer </returns>
        /// <exception cref="InvalidInputException">Thrown if the receiven id is invalid</exception>
        int FindNearestBaseStationByCustomerId(int customerId)
        {
            if (customerId < 100000000 || customerId >= 1000000000) throw new InvalidInputException("Id");

            double minDistance = double.MaxValue;
            int nearestBaseStationId = 0;

            //Get the Sender location coordinates.

            double customerLatitude = dalObject.GetCustomerById(customerId).Latitude;
            double customerLongitude = dalObject.GetCustomerById(customerId).Longitude;
            if (dalObject.GetBaseStationList().Count() > 0)
            {
                foreach (var baseStation in dalObject.GetBaseStationList())
                {
                    //Calculate the distance between the sender and the current base station.
                    double distance = dalObject.Distance(baseStation.Latitude, customerLatitude, baseStation.Longitude, customerLongitude);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestBaseStationId = baseStation.Id;
                    }
                }
            }
            else
            {
                throw new ObjectNotFoundException();
            }
            return nearestBaseStationId;
        }

        /// <summary>
        /// Find the nearest base-station with at least one available charging-slot by id.
        /// </summary>
        /// <param name="location"> Location information to calculate the distance </param>
        /// <returns> Id of the nearest base-station with at least one available charge-slot if found </returns>
        /// <excption cref="ObjectNotFoundException"> Thrown if there are no stations with available charging slots </excption>
        internal int FindNearestBaseStationWithAvailableChargingSlots(Location location)
        {
            double minDistance = double.MaxValue;
            int nearestBaseStationId = 0;
            IEnumerable<DO.Station> stations = dalObject.GetStations(x => x.ChargeSlots > 0);
            if (stations.Count() == 0) throw new ObjectNotFoundException("Stations with available charging slots");

            foreach (var myBaseStation in stations)
            {
                double distance = dalObject.Distance(myBaseStation.Latitude, location.Latitude, myBaseStation.Longitude, location.Longitude);

                if (distance < minDistance)
                {
                    nearestBaseStationId = myBaseStation.Id;
                    minDistance = distance;
                }
            }
            return nearestBaseStationId;
        }

        /// <summary>
        /// Find the minimal needed power suply for a given drone to deliver the parcel
        /// and go to the nearest base-station for charging.
        /// </summary>
        /// <param name="drone"> Drone object to calculate his needed minimum power suply</param>
        /// <param name="customerId"> Target id to calculate the needed power suply for the drone to get there</param>
        /// <returns>The minimum needed power suply to go to the target and to base station for charging if found,
        ///              otherwise retun 0 </returns>
        /// <exception cref="InvalidInputException"> Thrown if drone id or customer id is invalid </exception>
        double FindMinPowerSuply(DroneToList drone, int customerId)
        {
            if (drone.Id < 1000 || drone.Id > 10000) throw new InvalidInputException("drone");
            if (customerId < 100000000 || customerId >= 1000000000) throw new InvalidInputException("customer Id");

            //Step 1: Find the distance between the drone current location and the destination location.
            Location location = new();
            lock (dalObject)
            {
                location.Latitude = dalObject.GetCustomerById(customerId).Latitude;
                location.Longitude = dalObject.GetCustomerById(customerId).Longitude;
                double distance1 = dalObject.Distance(drone.CurrentLocation.Latitude, location.Latitude, drone.CurrentLocation.Longitude, location.Longitude);

                //Step 2: Find the minimal needed power suply to go to the destination.
                double suply1 = 0;
                switch (drone.MaxWeight)
                {
                    case WeightCategories.Heavy:
                        //Available-0, Light-1, Intermediate-2, Heavy-3, DroneChargingRate-4.
                        suply1 = distance1 / dalObject.ElectricityUseRequest()[3];
                        break;
                    case WeightCategories.Intermediate:
                        suply1 = distance1 / dalObject.ElectricityUseRequest()[2];
                        break;
                    case WeightCategories.Light:
                        suply1 = distance1 / dalObject.ElectricityUseRequest()[1];
                        break;
                }

                //Step 3: Find the nearest base-station with available charge-slot and calcuolate the needed power suply 
                //        and calculate the distance between the cutomer and the base-station.
                int closestBaseStationId = FindNearestBaseStationWithAvailableChargingSlots(location);
                if (closestBaseStationId != 0)
                {
                    double nearestBaseStationLatitude = dalObject.GetStationById(closestBaseStationId).Latitude;
                    double nearestBaseStationLongitude = dalObject.GetStationById(closestBaseStationId).Longitude;
                    double distance2 = dalObject.Distance(location.Latitude, nearestBaseStationLatitude, location.Longitude, nearestBaseStationLongitude);
                    //Step 4: Find the minimal needed power suply to go to the destination.
                    double suply2 = distance2 / dalObject.ElectricityUseRequest()[0];

                    //Step 5: Calculate the final needed power suply.
                    double minBatteryValue = suply1 + suply2;

                    return minBatteryValue;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Find the minimum needed power suply to go to the nearest base-station 
        ///          (with at least one available charging slot) for charging.
        /// </summary>
        /// <param name="drone"> Drone object to caclculate the minimum needed power suply </param>
        /// <returns> Minimum needed power suply </returns>
        /// <exception cref="InvalidInputException">Thrown if drone id is invalid </exception>
        /// <exception cref="ObjectNotFoundException"> Thrown if there are no station with such id </exception>
        double FindMinPowerSuplyForCharging(DroneToList drone)
        {
            if (drone.Id < 1000 || drone.Id > 10000) throw new InvalidInputException("drone");

            //Step 1: Find the nearest base-station with available charge-slot and calcuolate the needed power suply.
            int closestBaseStationId = FindNearestBaseStationWithAvailableChargingSlots(drone.CurrentLocation);
            if (closestBaseStationId == 0) throw new ObjectNotFoundException("base-Station");
            lock (dalObject)
            {
                double nearestBaseStationLatitude = dalObject.GetStationById(closestBaseStationId).Latitude;
                double nearestBaseStationLongitude = dalObject.GetStationById(closestBaseStationId).Longitude;
                double distance = dalObject.Distance(drone.CurrentLocation.Latitude, nearestBaseStationLatitude, drone.CurrentLocation.Longitude, nearestBaseStationLongitude);

                //Step 2: Find the minimal needed power suply to go to the destination.
                double minBatteryValue = distance / dalObject.ElectricityUseRequest()[0];
                return minBatteryValue;
            }
        }

        /// <summary>
        /// Get the minimun power of battery for distance between the drone and the destination.
        /// </summary>
        /// <param name="droneId"> Drone Id </param>
        /// <param name="customerId"> Customer Id </param>
        /// <returns> Minimun power of battery for distance between the drone and the dastination </returns>
        /// /// <exception cref="InvalidInputException"> Thrown if drone id or customer id is invalid </exception>
        /// <exception cref="ObjectNotFoundException"> Thrown if there are no drone with such id </exception>
        double FindMinPowerSuplyForDistanceBetweenDroneToTarget(int droneId, int customerId)
        {
            if (droneId < 1000 || droneId > 10000) throw new InvalidInputException("drone id");
            if (customerId < 100000000 || customerId >= 1000000000) throw new InvalidInputException("customer Id");


            DroneToList blDrone = droneToLists.Find(x => x.Id == droneId);
            if (blDrone.Id != droneId) throw new ObjectNotFoundException("drone");

            Customer myCustomer = GetCustomerByIdBL(customerId);

            lock (dalObject)
            {
                double myDistantce = dalObject.Distance(blDrone.CurrentLocation.Latitude, myCustomer.Location.Latitude,
                    blDrone.CurrentLocation.Longitude, myCustomer.Location.Longitude);

                double mySuply = 0;

                switch (blDrone.MaxWeight)
                {
                    case WeightCategories.Heavy:
                        //Available-0, Light-1, Intermediate-2, Heavy-3, DroneChargingRate-4.
                        mySuply = myDistantce / dalObject.ElectricityUseRequest()[3];
                        break;
                    case WeightCategories.Intermediate:
                        mySuply = myDistantce / dalObject.ElectricityUseRequest()[2];
                        break;
                    case WeightCategories.Light:
                        mySuply = myDistantce / dalObject.ElectricityUseRequest()[1];
                        break;
                }
                return mySuply;
            }
        }

        /// <summary>
        /// Get the minimun power of battery for all the jurney of the drone.
        /// </summary>
        /// <param name="droneId"> Drone Id </param>
        /// <param name="senderId"> Customer Id </param>
        /// <returns> Minimun power of battery for all the jurney of the drone </returns>
        /// /// <exception cref="InvalidInputException"> Thrown if drone id or customer id is invalid </exception>
        /// <exception cref="ObjectNotFoundException"> Thrown if there are no drone with such id </exception>
        double FindMinSuplyForAllPath(int droneId, int senderId, int targetId)
        {
            if (droneId < 1000 || droneId > 10000) throw new InvalidInputException("drone id");
            if (senderId < 100000000 || senderId >= 1000000000) throw new InvalidInputException("customer Id");
            if (targetId < 100000000 || targetId >= 1000000000) throw new InvalidInputException("customer Id");

            //DroneToList blDrone = droneToLists.Find(x => x.Id == droneId);
            Drone blDrone = GetDroneByIdBL(droneId);
            if (blDrone.Id != droneId) throw new ObjectNotFoundException("drone");

            double minSuply1 = FindMinPowerSuplyForDistanceBetweenDroneToTarget(blDrone.Id, senderId);

            Customer sender = GetCustomerByIdBL(senderId);
            blDrone.CurrentLocation = sender.Location;

            DroneToList drone = droneToLists.Find(x => x.Id == blDrone.Id);
            double minSuply2 = FindMinPowerSuply(drone, targetId);
            return minSuply1 + minSuply2;
        }
        public DroneCharge FindDroneChargeByDroneIdBL(int droneId)
        {
            try
            {

                DO.DroneCharge droneCharge = dalObject.GetDroneChargeByDroneId(droneId);
                DroneCharge newDroneCharge = new();
                newDroneCharge.DroneId = droneCharge.DroneId;
                newDroneCharge.ChargeTime = droneCharge.ChargeTime;
                return newDroneCharge;
            }
            catch (XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message);
            }
        }
        #endregion


        #region Sexagesimal

        /// <summary>
        /// Return string of sexagesimal presentation.
        /// </summary>
        /// <param name="decimalNumber"></param>
        /// <returns> String of sexagesimal presentation </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string SexagesimalPresentation(double decimalNumber)
        {
            int degrees, minutes1, seconds;
            double minutes2;

            degrees = (int)decimalNumber;

            minutes2 = (decimalNumber - degrees) * 60;
            minutes1 = (int)minutes2;

            seconds = (int)((minutes2 - minutes1) * 60);

            return $"{degrees}°{minutes1}'{seconds}\"";
        }
        #endregion
    }
}