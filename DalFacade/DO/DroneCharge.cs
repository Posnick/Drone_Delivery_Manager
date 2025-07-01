﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    /// <summary>
    /// Represenr droneCharge card which connect between the station and the charging drone in it
    /// </summary>
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }
        public DateTime ChargeTime { get; set; }
        /// <summary>
        /// Return describe of DroneCharge class string.
        /// </summary>
        /// <returns> Describe of DroneCharge class string </returns>
        public override string ToString()
        {
            return $"DroneCharge:\n" +
                $"Id: {DroneId}\n" +
                $"StationId: {StationId}" +
                $"Charge time: {ChargeTime}";
        }
    }
}