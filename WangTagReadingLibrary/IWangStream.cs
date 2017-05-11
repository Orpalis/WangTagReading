namespace WangTagReadingLibrary
{
    /// <summary>
    /// The IWangStream interface describes an interface to feed Wang annotation reader with bytes.
    /// </summary>
    internal interface IWangStream
    {
        /// <summary>
        /// ReadBytes reads bytes from the stream and retrieves a buffer with the bytes read.
        /// </summary>
        /// <remarks>
        /// The buffer should have been allocated so it is large enough to store count bytes.
        /// </remarks>
        /// <param name="data">The buffer where to write the read data.</param>
        /// <param name="count">The number of bytes to read.</param>
        void ReadBytes(byte[] data, int count);

        /// <summary>
        /// ReadInts32 reads 4 bytes integers from the stream and retrieves a buffer with the integers read.
        /// </summary>
        /// <remarks>
        /// The buffer should have been allocated so it is large enough to store count integers.
        /// </remarks>
        /// <param name="data">The buffer where to write the read data.</param>
        /// <param name="count">The number of integers to read.</param>
        void ReadInts32(int[] data, int count);

        /// <summary>
        /// SkipBytes skips a couple of bytes, moving forward the reading index.
        /// </summary>
        /// <remarks>
        /// Skipping bytes may be usefull for some data the reader doesn't care about such
        /// as reserved bytes and unsupported data fields.
        /// </remarks>
        /// <param name="count">The number of bytes to skip.</param>
        void SkipBytes(int count);

        /// <summary>
        /// ReadByte reads a byte and moves forward the reading index.
        /// </summary>
        /// <returns>The byte read.</returns>
        byte ReadByte();

        /// <summary>
        /// ReadUint reads a 4 bytes unsigned int and moves forward the reading index.
        /// </summary>
        /// <returns>The unsigned int read.</returns>
        uint ReadUint32();

        /// <summary>
        /// ReadInt32 reads a 4 bytes signed int and moves forward the reading index.
        /// </summary>
        /// <returns>The int read.</returns>
        int ReadInt32();

        /// <summary>
        /// IsEnd tests whether the end of the stream has been reached or not.
        /// </summary>
        /// <returns></returns>
        bool IsEnd();

        /// <summary>
        /// AvailableBytes retrieves the number of available bytes.
        /// </summary>
        /// <returns>The number of available bytes.</returns>
        int AvailableBytes();
    }
}
