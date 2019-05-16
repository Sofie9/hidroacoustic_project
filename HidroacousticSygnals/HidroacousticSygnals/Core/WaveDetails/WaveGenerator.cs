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
            var format = new WaveFormatChunk();
            var data = this.GenerateData(format);

            var waveHelper = new WaveStructure(header, format, data);
            return waveHelper.Save(filePath);
        }
        private WaveDataChunk GenerateData(WaveFormatChunk format)
        {
            var core = this.core;
            uint numSamples = 2048 * 4;
            int time = core.TimeSec;
            var dataChunk = new WaveDataChunk(numSamples);
            try
            {
                var vectors = new List<Vector2>();
                var resVector = new Vector2(0,0);
                for (int i = 0; i < dataChunk.numSamples; i++)
                {
                    
                    for (var n = -core.MaxCountOfRay; n <= core.MaxCountOfRay; n++)
                    {
                        var waveLength = core.GetWaveLength(n);
                       
                        var r2 = Math.Pow(core.HSystem.x, 2);

                        var Rd = Math.Round(Math.Sqrt(r2 + waveLength),4);//Длинна луча, отбитого от дна или от поверхности
                        var floatNumber = (Rd / core.WaveLenght);

                        var divisionPart = floatNumber - Math.Truncate(floatNumber);//остаток от деления
                        var fi = Math.Round(divisionPart * (2 * Math.PI),4);
                        var x = (float)(1 * Math.Cos(fi));
                        var y = (float)(1 * Math.Sin(fi));
                        resVector += new Vector2();
                        vectors.Add(new Vector2(x, y));
                    }

                    
                    var sumVector = new Vector2(vectors.Average(x => x.X), vectors.Average(x => x.Y));
                    var r = resVector;


                    var Vx = core.GetOscillatorySpeed(i, sumVector, core.HSystem.x);
                    var Vy = core.GetOscillatorySpeed(i, sumVector, core.HSystem.x);
                    var Vz = core.GetOscillatorySpeed(i, sumVector, core.HSystem.x);

                    //var Vy = Math.Round(Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.y / firstRayLength),4);
                    //var Vz = Math.Round(Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.z / firstRayLength),4);


                    dataChunk.shortArray[i] = (short)Convert.ToInt16(Vx);//+ (short)Convert.ToInt16(Vy) + (short)Convert.ToInt16(Vz))


                }

            }
            catch (Exception ex) { }

            // Calculate data chunk size in bytes
            dataChunk.dwChunkSize = (uint)(dataChunk.shortArray.Length * (format.wBitsPerSample / 8));
            return dataChunk;
        }
        
    }
}