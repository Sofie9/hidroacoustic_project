using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Windows;

namespace HidroacousticSygnals.Core
{
    public class WaveGenerator
    {
        // Header, Format, Data chunks
        SoundHelper.WaveHeader header;
        SoundHelper.WaveFormatChunk format;
        SoundHelper.WaveDataChunk data;

        public WaveGenerator()
        {
            this.initChunks();
        }
        public WaveGenerator(CoreHelper core)
        {
            this.initChunks();
            this.GenerateData(core);
        }

        private uint GenerateData(CoreHelper core)
        {

            //Random rnd = new Random();
            //short randomValue = 0;

            //for (int i = 0; i < numSamples; i++)
            //{
            //    randomValue = Convert.ToInt16(rnd.Next(-amplitude, amplitude));
            //    data.shortArray[i] = randomValue;
            //}
            uint numSamples = 2048 * 4;
            int time = core.TimeSec;

            data.shortArray = new short[numSamples];
            try
            {
                var vectors = new List<Vector2>();
                for (int i = 0; i < numSamples; i++)
                {
                    var rFirst = core.GetRFirst(core.Ship);
                    for (var n = -core.MaxCountOfRay; n <= core.MaxCountOfRay; n++)
                    {
                        double rightPArt = 0;
                        if (n < 0)
                        {
                            rightPArt = Math.Round(core.HSystem.y + core.Ship.y - 2 * (-n) * core.SeaDeep,4);
                        }

                        if (n > 0)
                        {
                            rightPArt = Math.Round(core.HSystem.y + core.Ship.y - 2 * (n) * core.SeaDeep,4);
                        }

                        var inSqrtD = Math.Round(Math.Pow(core.HSystem.x, 2) + Math.Pow(rightPArt, 2),4);
                        var Rd = Math.Round(Math.Sqrt(inSqrtD),4);//Длинна луча, отбитого от дна или от поверхности
                        var floatNumber = (Rd / core.WaveLenght);

                        var divisionPart = floatNumber - Math.Truncate(floatNumber);
                        var _phaze = Math.Round(divisionPart * (2 * Math.PI),4);//получить дробную часть от (Rd / core.WaveLenght)
                        float x = (float)(1 * Math.Cos(_phaze));
                        float y = (float)(1 * Math.Sin(_phaze));

                        vectors.Add(new Vector2(x, y));
                    }
                    var sum = new Vector2(vectors.Average(x => x.X), vectors.Average(x => x.Y));
                    var res = sum.Y / sum.X;
                    var acrtgFi = Math.Atan(res);

                    var Vx = Math.Round(Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.x / rFirst),4);
                    //var Vy = Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.y / rFirst);
                    //var Vz = Math.Sin(core.Frequency * (i / core.Frequency) + acrtgFi) * (core.HSystem.z / rFirst);

                    data.shortArray[i] = (short)Convert.ToInt16(Vx);//+ (short)Convert.ToInt16(Vy) + (short)Convert.ToInt16(Vz))


                }

            }
            catch (Exception ex) { }

            //var t = i / 2000;
            //core.Ship = new CoreHelper.SourceShip(core.Ship.x+i,core.Ship.y);
            //var r1 = core.GetR1(core.Ship);
            //var r2 = core.GetR2(core.Ship);
            //var r3 = core.GetR3(core.Ship);

            //var partValue = 2 * Math.PI * core.Frequency * r1;
            //var cos = Math.Cos(partValue);
            //var p1 = core.Amplitude * cos;

            //var p2 = core.Amplitude * Math.Cos(2 * Math.PI * core.Frequency * r2);
            //var p3 = core.Amplitude * Math.Cos(2 * Math.PI * core.Frequency * r3);
            //var length = (r2 + r3) / r1;
            ////var pSum = p1 + p2 + p3;

            //var Vx = Math.Sin(core.Ship.x) * length;
            //var Vy = Math.Sin(core.Ship.y) * length;
            //var Vz = Math.Sin(core.Ship.z) * length;



            //data.shortArray[i+1] = Convert.ToInt16(Vy);
            //data.shortArray[i+2] = Convert.ToInt16(Vz);

            // Calculate data chunk size in bytes
            return data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));

        }

        private void initChunks()
        {
            this.header = new SoundHelper.WaveHeader();
            this.format = new SoundHelper.WaveFormatChunk();
            this.data = new SoundHelper.WaveDataChunk();
        }

        public void AnyOneEx(string filePath)
        {
            var duration = 10;
            var bitsPerSample = 8;
            var samplesPerSec = 8000;
            var f1 = 400;
            var f2 = 450;
            var pattern = new[] {
                TimeSpan.FromMilliseconds(400),
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(400),
                TimeSpan.FromMilliseconds(2000)
            };

            var wavdata = new short[duration * samplesPerSec]; // 10 seconds of wav data @ 8000 samples per sec, 8 bits per sample, 1 channel 

            // Loop through each sample
            for (var i = 0; i < wavdata.Length; i = i + (bitsPerSample / 8))
            {

                // Get time in seconds of the current sample
                var time = Convert.ToDouble(i) / (Convert.ToDouble(bitsPerSample) / 8) / samplesPerSec;

                // Calculate the on off pattern
                var onoff = 0;
                var timeMilliseconds = time * 1000;
                var p = 0;
                while (timeMilliseconds >= 0)
                {
                    timeMilliseconds = timeMilliseconds - pattern[p].TotalMilliseconds;
                    onoff = onoff == 1 ? 0 : 1;
                    if (++p >= pattern.Length) p = 0;
                }

                // Calculate the sample: (sin(time * 400) * 128 + sin(time * 450) * 128)) / 2
                var sample = onoff * (((Math.Sin(2 * Math.PI * time * f1) * 127) + (Math.Sin(2 * Math.PI * time * f2) * 127)) / 2);

                // Store sample
                wavdata[i] = Convert.ToInt16(sample + 128);

            }

            data.shortArray = wavdata;
            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
            //return wavdata;
        }
        public void GenerateSimpleData()
        {
            try
            {
                uint numSamples = format.dwSamplesPerSec * format.wChannels;

                // Initialize the 16-bit array
                data.shortArray = new short[numSamples];

                int amplitude = 32760; // Max amplitude for 16-bit audio
                double freq = 440.0f; // Concert A: 440Hz

                // The "angle" used in the function, adjusted for the number of channels and sample rate.
                // This value is like the period of the wave.
                double t = (Math.PI * 2 * freq) / (format.dwSamplesPerSec * format.wChannels);

                for (uint i = 0; i < numSamples - 1; i++)
                {
                    // Fill with a simple sine wave at max amplitude
                    for (int channel = 0; channel < format.wChannels; channel++)
                    {
                        data.shortArray[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                    }
                }

                data.dwChunkSize = (uint) (data.shortArray.Length * (format.wBitsPerSample / 8));

            }
            catch (Exception ex)
            {

            }
           
        }
        public enum WaveExampleType
        {
            ExampleSineWave = 0,
            HidroacousticWave = 1
        
        }
        public void Save(string filePath)
        {
            // Create a file (it always overwrites)
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            // Use BinaryWriter to write the bytes to the file
            BinaryWriter writer = new BinaryWriter(fileStream);

            // Write the header
            writer.Write(header.sGroupID.ToCharArray());
            writer.Write(header.dwFileLength);
            writer.Write(header.sRiffType.ToCharArray());

            // Write the format chunk
            writer.Write(format.sChunkID.ToCharArray());
            writer.Write(format.dwChunkSize);
            writer.Write(format.wFormatTag);
            writer.Write(format.wChannels);
            writer.Write(format.dwSamplesPerSec);
            writer.Write(format.dwAvgBytesPerSec);
            writer.Write(format.wBlockAlign);
            writer.Write(format.wBitsPerSample);

            // Write the data chunk
            writer.Write(data.sChunkID.ToCharArray());
            writer.Write(data.dwChunkSize);
            foreach (short dataPoint in data.shortArray)
            {
                writer.Write(dataPoint);
            }
            writer.Seek(4, SeekOrigin.Begin);
            uint filesize = (uint)writer.BaseStream.Length;
            writer.Write(filesize - 8);

            // Clean up
            writer.Close();
            fileStream.Close();
        }

        public class Point
        {
            public int x;
            public int y;

            public Point(int d, int d1)
            {
            }
        }
    }
}