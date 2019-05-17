using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Windows;
using HidroacousticSygnals.Core.WaveDetails;

namespace HidroacousticSygnals.Core
{
    public class WaveGenerator
    {
        // Header, Format, Data chunks
        public const string filePath = @"C:\Users\sofy9\Desktop\audio.dat";

        public WaveStructure wave { get; set; }
        public CoreHelper core { get; set; }
       
        public WaveGenerator(CoreHelper _core)
        {
            this.core = _core;
        }
        public bool GenerateAndSave()
        {
            var header = new WaveHeader();
            var format = new WaveFormatChunk(this.core);
            var data = this.GenerateData(format);

            var waveHelper = new WaveStructure(header, format, data);
            return waveHelper.Save(filePath);
        }
        public void ConvertCoordinates(CoreHelper.SourceShip s ,CoreHelper.HydroacousticSystem h)
        {

        }
        private WaveDataChunk GenerateData(WaveFormatChunk format)
        {
            var core = this.core;
            uint numSamples = (uint) ((uint) (core.Frequency) * 60);
            var dataChunk = new WaveDataChunk(numSamples);
            var k = core.Frequency / 1450;
            var a = Math.Sqrt(Math.Pow(core.Ship.j - core.HSystem.j, 2));
            //var R0 = CoreHelper.GetRayLength(core.Ship, core.HSystem);

            core.getBorders(out int top,out int bottom);
            core.HSystem.r = (int) Math.Sqrt(Math.Pow(core.R0, 2) - Math.Pow(a, 2));
            try
            {
                var vectors = new List<Vector2>();
                var resVector = new Vector2(0,0);
                for (int i = 0; i < numSamples; i++)
                {
                    var t = i / core.Frequency;
                    double p = 0;

                    for (var n = bottom; n <= top; n++)
                    {
                        var waveLength = core.GetWaveLength(n);
                       
                        var r2 = Math.Pow(core.HSystem.r, 2);

                        var Rd = Math.Round(Math.Sqrt(r2 + waveLength),4);//Длинна луча, отбитого от дна или от поверхности

                        var T = 1 / this.core.Frequency;
                        var partFi = Rd / (this.core.Frequency * T);

                        var divisionPart = partFi - Math.Truncate(partFi);//остаток от деления
                        var fi = Math.Round(divisionPart * (2 * Math.PI),4);

                        var x = (float)(1 * Math.Cos(fi));
                        var y = (float)(1 * Math.Sin(fi));
                        resVector += new Vector2(x, y);

                        //vectors.Add(new Vector2(x, y));
                        #region P
                      
                        var positiveWaveL = Math.Pow(r2 + core.GetWaveLength(n, 1), 1/2);
                        var negativeWaveL = Math.Pow(r2 + core.GetWaveLength(n, -1), 1 / 2);
                        var firstPart = (1 / 4 * Math.PI) * Math.Pow(-1, n);

                        p += firstPart  *
                             (Math.Cos(k * negativeWaveL * t) / negativeWaveL) 
                             - (Math.Cos(k * positiveWaveL * t)/positiveWaveL);

                        #endregion

                    }


                    //var sumVector = new Vector2(vectors.Average(x => x.X), vectors.Average(x => x.Y));
                    var r = resVector;
                   
                    var Vx = core.GetOscillatorySpeed(i, resVector, core.HSystem.r);
                    var Vy = 0;
                    var Vz = core.GetOscillatorySpeed(i, resVector, core.HSystem.j);

                    //var Vy = Math.Round(Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.y / firstRayLength),4);
                    //var Vz = Math.Round(Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.z / firstRayLength),4);


                    dataChunk.shortArray[i] = (short)Convert.ToInt16(Math.Round(Vx));
                    dataChunk.shortArray[i+1] = (short)Convert.ToInt16(Vy);
                    dataChunk.shortArray[i+2] = (short)Convert.ToInt16(Math.Round(Vz));
                    dataChunk.shortArray[i+3] = (short)Convert.ToInt16(Math.Round(p));

                    i += 4;
                }

            }
            catch (Exception ex) { }

            // Calculate data chunk size in bytes
            dataChunk.dwChunkSize = (uint)(dataChunk.shortArray.Length * (format.wBitsPerSample / 8));
            return dataChunk;
        }
        
    }
}