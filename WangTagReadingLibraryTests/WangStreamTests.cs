using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WangTagReadingLibrary.Tests
{
    /// <summary>
    /// The WangStreamTests class implements unit tests for the WangStream class.
    /// </summary>
    [TestClass()]
    public class WangStreamTests
    {
        /// <summary>
        /// ReadByteTest tests the reading of a few bytes, one by one.
        /// </summary>
        /// <remarks>
        /// A buffer is initialized, a stream is constructed with the buffer and we make sure the read behaves properly.
        /// </remarks>
        [TestMethod()]
        public void ReadByteTest()
        {
            byte[] input = new byte[3] {1, 2, 3};

            WangStream wangStream = new WangStream(input);
            for (int index = 0; index < input.Length; index ++)
            {
                Assert.IsFalse(wangStream.IsEnd());
                Assert.AreEqual(input.Length - index, wangStream.AvailableBytes());
                Assert.AreEqual(input[index], wangStream.ReadByte());
            }
            Assert.IsTrue(wangStream.IsEnd());
            Assert.AreEqual(0, wangStream.AvailableBytes());
        }

        /// <summary>
        /// SkipBytesTest tests the software behaves properly when asked to skip a few bytes.
        /// </summary>
        /// <remarks>
        /// A buffer is initialized, a stream is constructed with the buffer.
        /// The test skips the first bytes and read one, make sure we have the expected value.
        /// </remarks>
        [TestMethod()]
        public void SkipBytesTest()
        {
            byte[] input = new byte[4] {1, 2, 3, 4};
            WangStream wangStream = new WangStream(input);
            Assert.IsFalse(wangStream.IsEnd());
            wangStream.SkipBytes(2);
            Assert.IsFalse(wangStream.IsEnd());
            Assert.AreEqual(2, wangStream.AvailableBytes());
            Assert.AreEqual(3, wangStream.ReadByte());
            Assert.IsFalse(wangStream.IsEnd());
            Assert.AreEqual(1, wangStream.AvailableBytes());
        }

        /// <summary>
        /// ReadBytesTest tests the reading of a few bytes, creading a chunk of bytes.
        /// </summary>
        /// <remarks>
        /// A buffer is initialized, a stream is constructed with the buffer and we make sure the read behaves properly.
        /// </remarks>
        [TestMethod()]
        public void ReadBytesTest()
        {
            int bufferLength = 10;
            int chunkSize = 2;
            byte[] input = new byte[bufferLength];
            byte[] output = new byte[bufferLength];
            byte[] chunk = new byte[chunkSize];

            for (int index = 0; index < bufferLength; index ++)
            {
                input[index] = (byte) (index + 10);
                output[index] = 0xff;
            }

            WangStream wangStream = new WangStream(input);
            int read = 0;
            while (wangStream.AvailableBytes() != 0)
            {
                Assert.IsFalse(wangStream.IsEnd());
                wangStream.ReadBytes(chunk, chunkSize);
                Array.Copy(chunk, 0, output, read, chunkSize);
                read += chunkSize;
            }

            Assert.IsTrue(wangStream.IsEnd());
            Assert.AreEqual(0, wangStream.AvailableBytes());
            Assert.AreEqual(bufferLength, output.Length);
            for (int index = 0; index < bufferLength; index++)
            {
                Assert.AreEqual(input[index], output[index]);
            }
        }

        /// <summary>
        /// ReadInts32Test tests the reading of a few integers.
        /// </summary>
        [TestMethod()]
        public void ReadInts32Test()
        {
            // Well formed data
            byte[] streamData =
            {
                // Left
                100, 0, 0, 0,
                // Top
                1, 0, 0, 0,
                // Right
                200, 0, 0, 0,
                // Bottom
                55, 0, 0, 0,
            };
            WangStream wangStream = new WangStream(streamData);
            int[] data = new int[4];
            wangStream.ReadInts32(data, 4);
            Assert.AreEqual(100, data[0]);
            Assert.AreEqual(1, data[1]);
            Assert.AreEqual(200, data[2]);
            Assert.AreEqual(55, data[3]);
        }

        /// <summary>
        /// ReadUint32Test tests the reading of a 32 bytes unsigned integers, one by one.
        /// </summary>
        /// <remarks>
        /// A buffer is initialized, a stream is constructed with the buffer and we make sure the read behaves properly.
        /// </remarks>
        [TestMethod()]
        public void ReadUint32Test()
        {
            byte[] input = new byte[4] {0xff, 0xFF, 0xff, 0xFF};

            WangStream wangStream = new WangStream(input);
            Assert.IsFalse(wangStream.IsEnd());
            Assert.AreEqual(0xffffffff, wangStream.ReadUint32());
            Assert.AreEqual(0, wangStream.AvailableBytes());
            Assert.IsTrue(wangStream.IsEnd());
            Assert.AreEqual(0, wangStream.AvailableBytes());
        }

        /// <summary>
        /// ReadInt32Test tests the reading of a 32 bytes integers, one by one.
        /// </summary>
        /// <remarks>
        /// A buffer is initialized, a stream is constructed with the buffer and we make sure the read behaves properly.
        /// </remarks>
        [TestMethod()]
        public void ReadInt32Test()
        {
            byte[] input = new byte[4] {0xff, 0xFF, 0xff, 0xFF};

            WangStream wangStream = new WangStream(input);
            Assert.IsFalse(wangStream.IsEnd());
            Assert.AreEqual(-1, wangStream.ReadInt32());
            Assert.AreEqual(0, wangStream.AvailableBytes());
            Assert.IsTrue(wangStream.IsEnd());
            Assert.AreEqual(0, wangStream.AvailableBytes());
        }
    }
}