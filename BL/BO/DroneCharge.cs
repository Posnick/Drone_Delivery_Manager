﻿using System;

namespace BO
{
    public class DroneCharge
    {
        public int DroneId { get; set; }
        public double BatteryStatus { get; set; }

        public DateTime ChargeTime { get; set; }

        /// <summary>
        /// Return describe of DroneCharge class string.
        /// </summary>
        /// <returns> Describe of DroneCharge class string </returns>
        public override string ToString()
        {
            return $"DroneCharge:\n " +
                    $"Id: {DroneId}\n" +
                    $"Battery status: {BatteryStatus}\n"+
                    $"Charge time: {ChargeTime}";
        }
    }
}

