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
            uint numSamples = (uint) (this.core.SamplingFrequency * this.core.TimeSec);
            var dataChunk = new WaveDataChunk(numSamples);
            var k = (core.Frequency * (2*Math.PI) )/ 1450;
            //var a = Math.Sqrt(Math.Pow(core.Ship.j - core.HSystem.j, 2));
            //var R0 = CoreHelper.GetRayLength(core.Ship, core.HSystem);

            //core.getBorders(out int top,out int bottom);
            //core.HSystem.r = (int) Math.Sqrt(Math.Pow(core.R0, 2) - Math.Pow(a, 2));

            try
            {
                
                var resVector = new Vector2(0,0);
                for (int i = 0; i < numSamples; i++)
                {
                    var t = i / this.core.SamplingFrequency;
                    double p = 0;

                    var angleB = this.core.constPartPov+1;
                    var bottomBorder = 1;
                    var topBorder = 2;

                    #region Bottom border of sum
                    while (angleB > this.core.constPartDno)
                    {
                        var b = bottomBorder % 2 != 0
                            ? 2 * bottomBorder * this.core.SeaDeep - this.core.Ship.z - this.core.HSystem.z
                            : 2 * bottomBorder * this.core.SeaDeep + this.core.Ship.z + this.core.HSystem.z;

                        bottomBorder++;
                        angleB = Math.Atan(Math.Sqrt(Math.Pow(this.core.HSystem.r, 2)) / b);
                    }
                    #endregion

                    #region Top border of sum
                    while (angleB > this.core.constPartPov)
                    {
                        var b = bottomBorder % 2 != 0
                            ? 2 * topBorder * this.core.SeaDeep + this.core.Ship.z - this.core.HSystem.z
                            : topBorder * this.core.SeaDeep + this.core.Ship.z + this.core.HSystem.z;

                        topBorder++;
                        angleB = Math.Atan(Math.Sqrt(Math.Pow(this.core.HSystem.r, 2)) / b);
                    }



                    #endregion


                    bottomBorder *= -1;

                    for (var n = bottomBorder; n <= topBorder; n++)
                    {
                        var waveLength = core.GetWaveLength(n);
                       
                        var r2 = Math.Pow(core.HSystem.r, 2);

                        var Rd = Math.Round(Math.Sqrt(r2 + waveLength), 4);//Длинна луча, отбитого от дна или от поверхности

                        var T = 1 / this.core.Frequency;
                        
                        var deltaIntermidiateWave = Rd - ((int)(Rd / this.core.IntermediateWaveLenght));//без остатка,кастим к инту ?

                        var partFi = (2 * Math.PI * deltaIntermidiateWave ) / this.core.IntermediateWaveLenght;
                        var divisionPart = partFi - Math.Truncate(partFi);//остаток от деления
                        var fi = Math.Round(divisionPart * (2 * Math.PI), 4);

                        var normAmplitude = this.core.Amplitude / (Math.Abs(bottomBorder) + topBorder);
                        var x = (float)(normAmplitude * Math.Cos(fi));
                        var y = (float)(normAmplitude * Math.Sin(fi));
                        resVector += new Vector2(x, y);

                        //vectors.Add(new Vector2(x, y));
                        #region P

                        var positiveWaveL = Math.Pow(r2 + core.GetWaveLength(n, 1), 1 / 2);
                        var negativeWaveL = Math.Pow(r2 + core.GetWaveLength(n, -1), 1 / 2);
                        var firstPart = (1 / 4 * Math.PI) * Math.Pow(-1 * normAmplitude, n);

                        p += firstPart *
                             (Math.Cos(k * negativeWaveL * t * (2*Math.PI) * this.core.Frequency) / negativeWaveL)
                             - (Math.Cos(k * positiveWaveL * t * (2 * Math.PI) * this.core.Frequency) / positiveWaveL);

                        #endregion

                    }


                    var Vx = core.GetOscillatorySpeed(t, resVector, resVector.X);
                    var Vy = 0;
                    var Vz = core.GetOscillatorySpeed(t, resVector,resVector.Y);


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