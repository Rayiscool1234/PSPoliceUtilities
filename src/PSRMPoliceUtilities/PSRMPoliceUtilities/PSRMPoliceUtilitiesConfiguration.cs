using System.Collections.Generic;
using PSRMPoliceUtilities.Models;
using Rocket.API;
using UnityEngine;

namespace PSRMPoliceUtilities
{
    public class PSRMPoliceUtilitiesConfiguration : IRocketPluginConfiguration
    {
        public float JailRadius { get; set; }
        public Vector3 ReleaseLocation { get; set; }
        public double CheckInterval { get; set; }
        public decimal ExperiencePerMinute { get; set; }
        public List<Jail> Jails { get; set; }

        public void LoadDefaults()
        {
            JailRadius = 5;
            ReleaseLocation = new Vector3(0, 0 ,0);
            CheckInterval = 15;
            ExperiencePerMinute = 5;
            Jails = new List<Jail>()
            {
                new Jail()
                {
                    Name = "Default",
                    X = 0,
                    Y = 0,
                    Z = 0
                }
            };
        }
    }
}