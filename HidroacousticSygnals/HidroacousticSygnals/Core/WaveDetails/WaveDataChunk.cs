namespace HidroacousticSygnals.Core.WaveDetails
{
    public class WaveDataChunk
    {
        public string sChunkID;     // "data"
        public uint dwChunkSize { get; set; } // Length of header in bytes
        public short[] shortArray; // 8-bit audio
        public uint numSamples { get; set; }
        /// <summary>
        /// Initializes a new data chunk with default values.
        /// </summary>
        public WaveDataChunk(uint numSamples)
        {
            this.numSamples = numSamples;
            shortArray = new short[this.numSamples];
            sChunkID = "data";
        }
    }
}