﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HidroacousticSygnals.Core.WaveDetails
{
    public class WaveStructure
    {
        public WaveHeader header { get; set; }
        public WaveFormatChunk format { get; set; }
        public WaveDataChunk data { get; set; }

        public WaveStructure(WaveHeader header, WaveFormatChunk format, WaveDataChunk data)
        {
            this.header = header;
            this.format = format;
            this.data = data;
        }
        //uint numsamples = this.numS;
        //ushort numchannels = 4;
        //ushort samplelength = 1; // in bytes
        //uint samplerate = 22050;

        //FileStream f = new FileStream(filePath, FileMode.Create);
        //BinaryWriter wr = new BinaryWriter(f);

        //wr.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        ////wr.Write("RIFF");
        //wr.Write(36 + numsamples* numchannels * samplelength);
        //wr.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
        //wr.Write(16);
        //wr.Write((ushort)1);
        //wr.Write(numchannels);
        //wr.Write(samplerate);
        //wr.Write(samplerate* samplelength * numchannels);
        //wr.Write(samplelength* numchannels);
        //wr.Write((ushort)(8 * samplelength));
        //wr.Write("data");
        //wr.Write(numsamples* samplelength);

        //// for now, just a square wave

        //foreach (short dataPoint in data.shortArray)
        //{
        //    wr.Write(dataPoint);
        //}
        //// writer.Seek(4, SeekOrigin.Begin);
        ////uint filesize = (uint)writer.BaseStream.Length;
        ////writer.Write(filesize - 8);

        //// Clean up
        //wr.Close();
        //f.Close();
        //return true;
        public bool Save(string filePath)
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
            return filesize > 0;
        }
    }
}