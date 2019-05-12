using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HidroacousticSygnals.Core
{
    public class CoreHelper
    {
        

        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double SeaDeep { get; set; }

        public HydroacousticSystem HSystem { get; set; }
        public SourceShip Ship { get; set; }

        public CoreHelper(HydroacousticSystem system, SourceShip ship, double freq, double amplitude, double deep)
        {
            this.HSystem = system;
            this.Ship = ship;
            this.Amplitude = amplitude;
            this.Frequency = freq;
            this.SeaDeep = deep;
        }
        
       // public double Pressure => this.Amplitude * Math.Cos(2 * Math.PI * this.Frequency * this.R);
        protected internal double GetR1(SourceShip ship)
        {
            //((HSystem.x - ship.x) ^ 2 + (HSystem.y - ship.y) ^ 2 + (HSystem.z- ship.z) ^ 2) ^ (1 / 2);
            return ((HSystem.x - ship.x) ^ 2 + (HSystem.y - ship.y) ^ 2)  ^ (1 / 2);
            
        }
        protected internal double GetR2(SourceShip ship)
        {
            //((HSystem.x - ship.x) ^ 2 + (HSystem.y - ship.y) ^ 2 + (HSystem.z- ship.z) ^ 2) ^ (1 / 2);
            return ((HSystem.x - ship.x) ^ 2 + (HSystem.y - (-ship.y)) ^ 2) ^ (1 / 2);
        }
        protected internal double GetR3(SourceShip ship)
        {
            var yShip = -2 * this.SeaDeep - ship.y;
            var sumX = (HSystem.x - ship.x) ^ 2;
            var sumY = (HSystem.y - (int)yShip) ^ 2;
            //(0,-2h-Z0)
            return (sumX + sumY) ^ (1 / 2);
            
        }
       
        public class HydroacousticSystem
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public HydroacousticSystem(int x, int y, int z = 0)
            {
                this.x = x;
                this.y = y;
                this.z = 0;
            }
        }
        public class SourceShip
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }

            public SourceShip(int x, int y, int z = 0)
            {
                this.x = x;
                this.y = y;
                this.z = 0;
            }
        }
        public class FrequencySpeed
        {
            public int Vx { get; set; }
            public int Vy { get; set; }
            public int Vz { get; set; }
        }

    }
}