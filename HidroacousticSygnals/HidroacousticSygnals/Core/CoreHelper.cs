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
        public int SamplingFrequency => 2048;
        public double IntermediateWaveLenght => 6.72 / this.Frequency;
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double SeaDeep { get; set; }
        public int TimeSec { get; set; }
        public HydroacousticSystem HSystem { get; set; }
        public SourceShip Ship { get; set; }

        public int MaxCountOfRay = 5;

        public double constPartPov => 0.11;

        public double constPartDno => 0.11;
        //private double constPartPov
        //{
        //    get
        //    {
        //        return Math.Tan(Math.Asin(1/(1450/335)));
        //    }
        //}
        //private double constPartDno
        //{
        //    get
        //    {
        //        return Math.Tan(Math.Asin(1 / (1450 / 1000)));
        //    }
        //}
        public double R0 => Math.Sqrt(Math.Pow(this.HSystem.r, 2) + Math.Pow(this.HSystem.z - this.Ship.z, 2));

        //public void getBorders(out int topBorder, out int bottomBorder)
        //{
            
        //    var b0P = this.Ship.j * this.constPartPov;
        //    var b1p = (Math.Abs(-2 * this.SeaDeep + this.Ship.j) * this.constPartPov - b0P) / 2;
        //    var n0p = (int) ((R0 - b0P) / b1p) + 1;
        //    var n1 = (int)n0p / 2;
        //    var n2 = n0p - n1;

        //    var b0d = (this.SeaDeep - Math.Abs(this.Ship.j)) * constPartDno;
        //    var b1d = Math.Abs(-2 * this.SeaDeep - this.Ship.j) * constPartDno - b0d;
        //    var n0d = (int) ((this.R0 - b0d) / b1d) + 1;
        //    var n3 = n0d / 2;
        //    var n4 = n0d - n3;
        //    topBorder = n4 + n1;
        //    bottomBorder = n3 + n2;
        //}
        public CoreHelper(HydroacousticSystem system, SourceShip ship, double freq, double amplitude, double deep, int time)
        {
            this.HSystem = system;
            this.Ship = ship;
            this.Amplitude = amplitude;
            this.Frequency = freq;
            this.SeaDeep = deep;
            this.TimeSec = time * 60 ;
        }

        public double GetWaveLength(int n, int? hardSign = null)
        {
            double insidePow;

            if (hardSign != null)
            {
                insidePow = Math.Round((this.HSystem.z + (hardSign.Value) * this.Ship.z) - 2 * n * this.SeaDeep, 4);
            }
            else
            {
                insidePow = Math.Round((this.HSystem.z + (n <= 0 ? 1 : -1) * this.Ship.z) - 2 * (n) * this.SeaDeep, 4);
            }

            return Math.Pow(insidePow, 2);
        }

        public static double GetRayLength(SourceShip ship, HydroacousticSystem gas)
        {
            return Math.Sqrt(Math.Pow(gas.x - ship.x, 2) + Math.Pow(gas.y - ship.y, 2) + Math.Pow(gas.z - ship.z, 2));
        }

        public double GetOscillatorySpeed(int t,Vector2 sumVector, float param)
        {
            //var firstRayLength = CoreHelper.GetRayLength(this.Ship, this.HSystem);
            var angle = sumVector.Y / sumVector.X;
            var acrtgFi = Math.Atan(angle);
            //var resVectorLength = Math.Sqrt(Math.Pow(sumVector.X, 2) + Math.Pow(sumVector.Y,2));
            var anglePart = this.Frequency*(2*Math.PI) * (t) + acrtgFi;


            return Math.Round(Math.Sin(anglePart) * ((param) / R0), 4);
        }
        // public double Pressure => this.Amplitude * Math.Cos(2 * Math.PI * this.Frequency * this.R);
       
       
        public class HydroacousticSystem
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public double r { get; set; }
            //public double a { get; set; }
            //public double b { get; set; }
            public HydroacousticSystem(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.r = Math.Sqrt(Math.Pow(x,2)+ Math.Pow(y,2));
                //this.a = Math.Sqrt(Math.Pow(r, 2));
            }
        }
        public class SourceShip
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }

            //public int j { get; set; }
            public int r { get; set; }

            public SourceShip(int z)
            {
                this.x = 0;
                this.y = 0;
                this.z = z;

                //this.j = (int) Math.Abs(zP - z) * -1;
                this.r = 0;
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