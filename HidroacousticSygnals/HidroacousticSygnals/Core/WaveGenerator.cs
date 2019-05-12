using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

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
            uint numSamples = format.dwSamplesPerSec * format.wChannels;
            data.shortArray = new short[numSamples];

            var rnd = new Random();
            for (int i = 0; i < numSamples; i++)
            {
                var t = i / 2000;
                core.Ship = new CoreHelper.SourceShip(core.Ship.x+i,core.Ship.y);
                var r1 = core.GetR1(core.Ship);
                var r2 = core.GetR2(core.Ship);
                var r3 = core.GetR3(core.Ship);

                var partValue = 2 * Math.PI * core.Frequency * r1;
                var cos = Math.Cos(partValue);
                var p1 = core.Amplitude * cos;

                var p2 = core.Amplitude * Math.Cos(2 * Math.PI * core.Frequency * r2);
                var p3 = core.Amplitude * Math.Cos(2 * Math.PI * core.Frequency * r3);
                var length = (r2 + r3) / r1;
                //var pSum = p1 + p2 + p3;

                var Vx = Math.Sin(core.Ship.x) * length;
                var Vy = Math.Sin(core.Ship.y) * length;
                var Vz = Math.Sin(core.Ship.z) * length;
                data.shortArray[i] = Convert.ToInt16(Vx);


                //data.shortArray[i+1] = Convert.ToInt16(Vy);
                //data.shortArray[i+2] = Convert.ToInt16(Vz);
            }
            double t2 = (Math.PI * 2 * 440.0f) / (format.dwSamplesPerSec * format.wChannels);
           
            // Calculate data chunk size in bytes
            return data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));

        }

        private void initChunks()
        {
            this.header = new SoundHelper.WaveHeader();
            this.format = new SoundHelper.WaveFormatChunk();
            this.data = new SoundHelper.WaveDataChunk();
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
    }
}