using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace HidroacousticSygnals.Core
{
    public class CoreHelper
    {
        //public static double _alfaBorder = Math.Sin(_alfa);
        //public static int _alfa = 1 / (1450 / 343);

        public double WaveLenght => this.Frequency * (1 / this.Frequency);
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double SeaDeep { get; set; }
        public int TimeSec { get; set; }
        public HydroacousticSystem HSystem { get; set; }
        public SourceShip Ship { get; set; }

        public int MaxCountOfRay = 5;

        public CoreHelper(HydroacousticSystem system, SourceShip ship, double freq, double amplitude, double deep, int time)
        {
            this.HSystem = system;
            this.Ship = ship;
            this.Amplitude = amplitude;
            this.Frequency = freq;
            this.SeaDeep = deep;
            this.TimeSec = time ;
        }

        public double GetWaveLength(int n, int? hardSign = null)
        {
            double insidePow;
            if (hardSign != null)
            {
                insidePow = Math.Round(this.HSystem.y + this.Ship.y - 2 * ((hardSign.Value) * n) * this.SeaDeep, 4);
            }
            else
            {
                insidePow = Math.Round(this.HSystem.y + this.Ship.y - 2 * ((n <= 0 ? -1 : 1) * n) * this.SeaDeep, 4);
            }

            return Math.Pow(insidePow,1/2);
        }

        public static double GetRayLength(SourceShip ship, HydroacousticSystem gas)
        {
            return Math.Sqrt(Math.Pow(gas.x - ship.x, 2) + Math.Pow(gas.y - ship.y, 2));
        }

        public double GetOscillatorySpeed(int counter,Vector2 sumVector, int param)
        {
            var firstRayLength = CoreHelper.GetRayLength(this.Ship, this.HSystem);
            var angle = sumVector.Y / sumVector.X;
            var acrtgFi = Math.Atan(angle);

            var anglePart = this.Frequency * (counter / this.Frequency) + acrtgFi;
            return Math.Round(Math.Sin(anglePart) * ((param) / firstRayLength), 4);
        }
        // public double Pressure => this.Amplitude * Math.Cos(2 * Math.PI * this.Frequency * this.R);
       
       
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



        #region OldData
        
        //protected internal double GetR2(SourceShip ship)
        //{
        //    //((HSystem.x - ship.x) ^ 2 + (HSystem.y - ship.y) ^ 2 + (HSystem.z- ship.z) ^ 2) ^ (1 / 2);
        //    return ((HSystem.x - ship.x) ^ 2 + (HSystem.y - (-ship.y)) ^ 2) ^ (1 / 2);
        //}
        //protected internal double GetR3(SourceShip ship)
        //{
        //    var yShip = -2 * this.SeaDeep - ship.y;
        //    var sumX = (HSystem.x - ship.x) ^ 2;
        //    var sumY = (HSystem.y - (int)yShip) ^ 2;
        //    //(0,-2h-Z0)
        //    return (sumX + sumY) ^ (1 / 2);

        //}
        #endregion
    }
}