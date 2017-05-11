using System;
#if DEBUG
using System.Diagnostics;
#endif // DEBUG

namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangStream class offers the default implementation for the IWangStream interface.
    /// </summary>
    /// <remarks>
    /// The default implementation reads data within a memory buffer.
    /// </remarks>
    internal sealed class WangStream : IWangStream
    {
        /// <summary>
        /// The constructor initializes members with the provided parameters.
        /// </summary>
        /// <param name="tagData">The data within the TIFF tag.</param>
        public WangStream(byte[] tagData)
        {
            _tagData = tagData;
            _offset = 0;
        }

        /// see @IWangStream.ReadBytes
        public void ReadBytes(byte[] data, int count)
        {
#if DEBUG
            Debug.Assert(count <= data.Length);
#endif
            Array.Copy(_tagData, _offset, data, 0, count);
            _offset += count;
        }

        /// see @IWangStream.ReadInts32
        public void ReadInts32(int[] data, int count)
        {
#if DEBUG
            Debug.Assert(count <= data.Length);
#endif
            Buffer.BlockCopy(_tagData, _offset, data, 0, count*4);
            _offset += count*4;
        }

        /// see @IWangStream.SkipBytes
        public void SkipBytes(int count)
        {
            _offset += count;
        }

        /// see @IWangStream.ReadByte
        public byte ReadByte()
        {
            byte ret = _tagData[_offset];
            _offset++;
            return ret;
        }

        /// see @IWangStream.ReadUint32
        public uint ReadUint32()
        {
            UInt32 ret = BitConverter.ToUInt32(_tagData, _offset);
            _offset += 4;
            return ret;
        }

        /// see @IWangStream.ReadInt32
        public int ReadInt32()
        {
            Int32 ret = BitConverter.ToInt32(_tagData, _offset);
            _offset += 4;
            return ret;
        }

        /// see @IWangStream.IsEnd
        public bool IsEnd()
        {
            // We may reach the end of the buffer but we do not expected to go to unallocated
            // data
#if DEBUG
            Debug.Assert(_offset <= _tagData.Length);
#endif // DEBUG
            return _offset >= _tagData.Length;
        }

        /// see @IWangStream.AvailableBytes
        public int AvailableBytes()
        {
            return _tagData.Length - _offset;
        }

        /// <summary>
        /// The data within the TIFF tag.
        /// </summary>
        private readonly byte[] _tagData;

        /// <summary>
        /// The reading offset.
        /// </summary>
        private int _offset;
    }
}
