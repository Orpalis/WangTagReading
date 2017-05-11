using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WangTagReadingLibrary;

namespace WangTagReadingLibraryTests
{
    /// <summary>
    /// The WangAnnotationReaderTest class implements unit tests for the WangAnnotationReaderTest class.
    /// </summary>
    [TestClass]
    public class WangAnnotationReaderTest
    {
        /// <summary>
        /// ReadHeaderStd verifies the behavior of the software when a valid header type is encountered.
        /// </summary>
        [TestMethod]
        public void ReadHeaderStd()
        {
            Random random = new Random(10);

            for (int dataSize = 8; dataSize < 16; dataSize++)
            {
                byte[] streamData = new byte[dataSize];
                for (int index = 0; index < dataSize; index++)
                {
                    streamData[index] = (byte) random.Next();
                }

                WangStream stream = new WangStream(streamData);
                Assert.IsTrue(WangAnnotationsReader.ReadHeader(stream));
                Assert.IsTrue(stream.AvailableBytes() == dataSize - 8);
            }
            ;
        }

        /// <summary>
        /// ReadHeaderInvalid verifies the behavior of the software when an invalid header type is encountered.
        /// </summary>
        [TestMethod]
        public void ReadHeaderInvalid()
        {
            for (int dataSize = 0; dataSize < 8; dataSize ++)
            {
                byte[] streamData = new byte[dataSize];
                byte b = 0;
                for (int index = 0; index < dataSize; index ++)
                {
                    streamData[index] = b++;
                }

                WangStream stream = new WangStream(streamData);
                Assert.IsFalse(WangAnnotationsReader.ReadHeader(stream));
            }
            ;
        }

        /// <summary>
        /// ReadDataTypeStd verifies the behavior of the software when a valid data type is encountered.
        /// </summary>
        [TestMethod]
        public void ReadDataTypeStd()
        {
            // The int values for the data types which will be tested.
            int[] intTypes =
            {
                2, 5, 6
            };
            // The data types which will be tested, aligned with the corresponding entry
            // in the intTypes array.
            WangAnnotationsReader.WangDataType[] dataTypes =
            {
                WangAnnotationsReader.WangDataType.DefaultNamedBlock, WangAnnotationsReader.WangDataType.Attributes,
                WangAnnotationsReader.WangDataType.NamedBlock
            };

            // The test runs with different data size. At least 8 bytes
            // are required for the stream to be valid.
            for (int expectedDataSize = 8; expectedDataSize < 40; expectedDataSize ++)
            {
                for (int dataTypeIndex = 0; dataTypeIndex < intTypes.Length; dataTypeIndex ++)
                {
                    // Valid stream data
                    byte[] streamData = new byte[expectedDataSize];
                    Array.Copy(BitConverter.GetBytes(intTypes[dataTypeIndex]), streamData, 4);
                    Array.Copy(BitConverter.GetBytes((int) expectedDataSize), 0, streamData, 4, 4);

                    WangStream stream = new WangStream(streamData);
                    WangAnnotationsReader.WangDataType dataType;
                    int dataSize;
                    Assert.IsTrue(WangAnnotationsReader.ReadDataType(out dataType, out dataSize, stream));
                    Assert.AreEqual(expectedDataSize, dataSize);
                    Assert.AreEqual(dataTypes[dataTypeIndex], dataType);
                    Assert.AreEqual(expectedDataSize - 8, stream.AvailableBytes());
                }
            }
        }

        /// <summary>
        /// ReadInvalidDataType verifies the behavior of the software when an invalid data type is encountered.
        /// </summary>
        [TestMethod]
        public void ReadDataTypeInvalidSize()
        {
            Random random = new Random(10);

            for (int dataSize = 0; dataSize < 8; dataSize++)
            {
                // Random stream data
                byte[] streamData = new byte[dataSize];
                for (int index = 0; index < dataSize; index++)
                {
                    streamData[index] = (byte) random.Next();
                }

                WangStream stream = new WangStream(streamData);
                WangAnnotationsReader.WangDataType dataType;
                int outputDataSize;
                Assert.IsFalse(WangAnnotationsReader.ReadDataType(out dataType, out outputDataSize, stream));
                Assert.AreEqual(WangAnnotationsReader.WangDataType.Invalid, dataType);
                Assert.AreEqual(0, outputDataSize);
            }
        }

        /// <summary>
        /// ReadInvalidDataType verifies the behavior of the software when an invalid data type is encountered.
        /// </summary>
        [TestMethod]
        public void ReadDataTypeInvalid()
        {
            int[] invalidDataTypes =
            {
                0, 1, 3, 4, 7, 8, 9, 10
            };

            foreach (var expectedDataType in invalidDataTypes)
            {
                // Valid stream data but invalid data type
                byte[] streamData = new byte[8];
                Array.Copy(BitConverter.GetBytes((int) expectedDataType), streamData, 4);
                Array.Copy(BitConverter.GetBytes((int) 8), 0, streamData, 4, 4);

                WangStream stream = new WangStream(streamData);
                WangAnnotationsReader.WangDataType dataType;
                int dataSize;
                Assert.IsTrue(WangAnnotationsReader.ReadDataType(out dataType, out dataSize, stream));
                Assert.AreEqual(WangAnnotationsReader.WangDataType.Invalid, dataType);
                Assert.AreEqual(8, dataSize);
                Assert.AreEqual(0, stream.AvailableBytes());
            }
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadBlockReadFailure()
        {
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112,
                0, 11, 0, 0, 0
            };

            // The streamData is well formed. 
            // Just using a part of the data should cause an error to rise.
            // Using the full data should cause an error as trailing information are expected.
            for (int dataSize = 0; dataSize <= streamData.Length; dataSize++)
            {
                byte[] streamDataTooShort = new byte[dataSize];
                Array.Copy(streamData, 0, streamDataTooShort, 0, dataSize);

                WangAnnotationProperties properties = new WangAnnotationProperties();
                WangStream stream = new WangStream(streamDataTooShort);
                Assert.IsFalse(WangAnnotationsReader.ReadBlock(properties, stream, streamDataTooShort.Length));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiFilNam" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiFilNam()
        {
            const int headerSize = 12;
            // Stream with a named block header and a file name
            byte[] streamData =
            {
                // Named block header
                79, 105, 70, 105, 108, 78, 97, 109,
                61, 0, 0, 0,
                // Character string for filename
                67, 58, 92, 68, 111, 99, 117,
                109, 101, 110, 116, 115, 32, 97,
                110, 100, 32, 83, 101, 116, 116,
                105, 110, 103, 115, 92, 65, 100,
                109, 105, 110, 92, 68, 101, 115,
                107, 116, 111, 112, 92, 87, 65,
                78, 71, 92, 115, 111, 117, 114,
                99, 101, 92, 48, 48, 49, 46,
                98, 109, 112, 0, 0,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.AreEqual("C:\\Documents and Settings\\Admin\\Desktop\\WANG\\source\\001.bmp", properties.Filename);
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiDIB" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiDib()
        {
            const int headerSize = 12;
            // Stream with a named block header and some bytes for the dib
            byte[] streamData =
            {
                // Named block header
                79, 105, 68, 73, 66, 0, 0, 0, 4, 0, 0, 0,
                // Dummy dib                
                0, 1, 2, 3,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.IsTrue(properties.HasDib);
            Assert.AreEqual(4, properties.DibInfo.Length);
            for (int index = 0; index < 4; index ++)
            {
                Assert.AreEqual((byte) index, properties.DibInfo[index]);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiGroup" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiGroup()
        {
            const int headerSize = 12;
            // Stream with a named block header and a group name
            byte[] streamData =
            {
                // Named block header
                79, 105, 71, 114, 111, 117, 112, 0,
                11, 0, 0, 0,
                // Character string for filename
                67, 58, 92, 68, 111, 99, 117,
                109, 101, 110, 116,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.AreEqual("C:\\Document", properties.OiGroup);
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiIndex" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiIndex()
        {
            const int headerSize = 12;
            // Stream with a named block header and a string for the index
            byte[] streamData =
            {
                // Named block header
                79, 105, 73, 110, 100, 101, 120, 0,
                11, 0, 0, 0,
                // Character string for filename
                67, 58, 92, 68, 111, 99, 117,
                109, 101, 110, 0,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.AreEqual("C:\\Documen", properties.OiIndex);
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnText" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnText()
        {
            const int headerSize = 12;
            // Stream with a named block header and a display text
            byte[] streamData =
            {
                // Named block header
                79, 105, 65, 110, 84, 101, 120, 116,
                24, 0, 0, 0,
                // Display text
                0, 0, 0, 0,
                242, 2, 0, 0,
                104, 1, 0, 0,
                5, 0, 0, 0,
                72, 101, 108, 108,
                111, 0, 0, 0,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.AreEqual((uint) 360, properties.DisplayText.CreationScale);
            Assert.AreEqual(0, properties.DisplayText.Orientation);
            Assert.AreEqual("Hello", properties.DisplayText.Text);
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnText" block in case of read failure
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnTextReadFailure()
        {
            const int headerSize = 12;
            // Stream with a named block header and a display text too short
            byte[] streamData =
            {
                // Named block header
                79, 105, 65, 110, 84, 101, 120, 116,
                12, 0, 0, 0,
                // Too short display text
                0, 0, 0, 0,
                242, 2, 0, 0,
                104, 1, 0, 0
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsFalse(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            // Only the header should have been read as the rest is not there
            Assert.AreEqual(streamData.Length - 12, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiHypLnk" block.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiHypLnk()
        {
            const int headerSize = 12;
            // Stream with a named block header and a display text
            byte[] streamData =
            {
                // Named block header
                79, 105, 72, 121, 112, 76, 110, 107, 35, 0, 0, 0, 
                // Hyperlink
                1, 0, 0, 0,
                14, 0, 0, 0,
                115, 111, 117, 114,
                99, 101, 92, 48,
                48, 49, 46, 106,
                112, 103, 1, 0,
                0, 0, 49, 0,
                0, 0, 0, 0,
                0, 0, 0,
                // Extra bytes
                1, 2, 3, 4
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsTrue(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
            Assert.AreEqual(4, stream.AvailableBytes());
            Assert.AreEqual(false, properties.Hyperlink.CanRemoveHyperlink);
            Assert.AreEqual(false, properties.Hyperlink.InternalLink);
            Assert.AreEqual("source\\001.jpg", properties.Hyperlink.Link);
            Assert.AreEqual("1", properties.Hyperlink.Location);
            Assert.AreEqual("", properties.Hyperlink.WorkingDirectory);
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiHypLnk" block in case of read failure
        /// </summary>
        [TestMethod]
        public void ReadBlockOiHypLnkReadFailure()
        {
            const int headerSize = 12;
            // Stream with a named block header and a display text too short
            byte[] streamData =
            {
                // Named block header
                79, 105, 72, 121, 112, 76, 110, 107, 
                12, 0, 0, 0, 
                // Too short display text
                0, 0, 0, 0,
                242, 2, 0, 0,
                104, 1, 0, 0
            };

            WangAnnotationProperties properties = new WangAnnotationProperties();
            WangStream stream = new WangStream(streamData);
            Assert.IsFalse(WangAnnotationsReader.ReadBlock(properties, stream, headerSize));
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnoDat" block when the properties have a mark attribute
        /// corresponding to a line type.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnoDatLineType()
        {
            Assert.Fail("to be developped");
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnoDat" block when the properties have a mark attribute
        /// corresponding to a line type and the read fails.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnoDatLineTypeReadFailure()
        {
            Assert.Fail("to be developped");
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnoDat" block when the properties have a mark attribute
        /// corresponding to a rotation type..
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnoDatRotationType()
        {
            Assert.Fail("to be developped");
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnoDat" block when the properties have a mark attribute
        /// corresponding to a rotation type and the read fails.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnoDatRotationTypeReadFailure()
        {
            Assert.Fail("to be developped");
        }

        /// <summary>
        /// Test the behavior of the ReadBlock method when reading an "OiAnoDat" block and no mark attribute or an unknow line type
        /// state is encountered.
        /// </summary>
        [TestMethod]
        public void ReadBlockOiAnoDatRotationNoMarkAttributes()
        {
            Assert.Fail("to be developped");
        }

        /// <summary>
        /// ReadEmpty verifies the behavior of the software when an reading an empty buffer.
        /// </summary>
        [TestMethod]
        public void ReadEmpty()
        {
            byte[] streamData = new byte[0];
            WangAnnotationHandlerTest handler = new WangAnnotationHandlerTest();
            Assert.IsFalse(WangAnnotationsReader.Read(handler, streamData));
        }

        /// <summary>
        /// Test the behavior of the Read method when the first data type is invalid.
        /// </summary>
        [TestMethod]
        public void ReadInvalidDataType()
        {
            byte[] streamData =
            {
                // Header
                0, 0, 0, 0,
                0, 0, 0, 0,
                // Data type
                9, 0, 0, 0,
                // Data Size,
                2, 0, 0, 0,
                // Trailing stuff
                1, 2, 3, 4
            };

            WangAnnotationHandlerTest handler = new WangAnnotationHandlerTest();
            Assert.IsFalse(WangAnnotationsReader.Read(handler, streamData));
        }

        /// <summary>
        /// Test the behavior of the Read method when the expected data size for the first call to ReadBlock is not available within the stream.
        /// </summary>
        [TestMethod]
        public void ReadInvalidDataSizeForBlock()
        {
            byte[] streamData =
            {
                // Header
                0, 0, 0, 0,
                0, 0, 0, 0,
                // Data type
                2, 0, 0, 0,
                // Data Size,
                200, 0, 0, 0,
                // Trailing stuff
                1, 2, 3, 4
            };

            WangAnnotationHandlerTest handler = new WangAnnotationHandlerTest();
            Assert.IsFalse(WangAnnotationsReader.Read(handler, streamData));
        }
    }
}
