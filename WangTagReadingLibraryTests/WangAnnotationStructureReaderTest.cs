using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WangTagReadingLibrary;

namespace WangTagReadingLibraryTests
{
    [TestClass]
    public class WangAnnotationStructureReaderTest
    {
        /// <summary>
        /// Test the behavior of the ReadNamedBlockHeader method when reading a standard well defined 32 bit data.
        /// </summary>
        [TestMethod]
        public void ReadNamedBlockHeaderStd32()
        {
            // Well formed data with an extra last byte
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 0, 11, 0, 0, 0, 91
            };

            WangStream stream = new WangStream(streamData);
            WangNamedBlockHeader read = WangAnnotationStructureReader.ReadNamedBlockHeader(stream, 12);
            Assert.IsTrue(read != null);

            Assert.AreEqual("OiGroup", read.Name);
            Assert.AreEqual(11, read.Size);
            Assert.AreEqual(1, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadNamedBlockHeader method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadNamedBlockHeaderReadFailure()
        {
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 0, 11, 0, 0, 0
            };

            // The streamData is well formed. 
            // Just using a part of the data should cause an error to rise.

            for (int dataSize = 0; dataSize < streamData.Length - 1; dataSize ++)
            {
                byte[] streamDataTooShort = new byte[dataSize];
                Array.Copy(streamData, 0, streamDataTooShort, 0, dataSize);

                WangStream stream = new WangStream(streamDataTooShort);
                WangNamedBlockHeader read = WangAnnotationStructureReader.ReadNamedBlockHeader(stream,
                    streamDataTooShort.Length);
                Assert.IsTrue(read == null);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadNamedBlockHeader method when the name is not a null terminated string.
        /// </summary>
        [TestMethod]
        public void ReadNamedBlockHeaderReadOversize()
        {
            // Well formed data with an extra last byte
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 0, 11, 0, 0, 0, 91
            };

            WangStream stream = new WangStream(streamData);
            // As we ask to read the entire data, including the extra byte, an
            // error should rise.
            WangNamedBlockHeader read = WangAnnotationStructureReader.ReadNamedBlockHeader(stream, streamData.Length);
            Assert.IsTrue(read == null);
        }

        /// <summary>
        /// Test the behavior of the ReadCharString method when reading a string shorter than the available space.
        /// </summary>
        [TestMethod]
        public void ReadCharStringStdShort()
        {
            // The buffer contains the string "OiGroup" and trailing null bytes.
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 0, 0, 0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            string read = WangAnnotationStructureReader.ReadCharString(stream, streamData.Length);

            Assert.AreEqual("OiGroup", read);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadCharString method when reading a string using exactly the available space.
        /// </summary>
        [TestMethod]
        public void ReadCharStringStdExact()
        {
            // The buffer contains the string "OiGroup" and a null bytes.
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 0
            };

            WangStream stream = new WangStream(streamData);
            string read = WangAnnotationStructureReader.ReadCharString(stream, streamData.Length);

            Assert.AreEqual("OiGroup", read);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadCharString method when the string is not null terminated.
        /// </summary>
        [TestMethod]
        public void ReadCharStringReadOversize()
        {
            // The buffer contains the string "OiGroup" and no null bytes.
            byte[] streamData =
            {
                79, 105, 71, 114, 111, 117, 112, 112
            };

            WangStream stream = new WangStream(streamData);
            string read = WangAnnotationStructureReader.ReadCharString(stream, 7);

            Assert.AreEqual("OiGroup", read);
            Assert.AreEqual(1, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadPoints method when reading a set of points shorter than the available space.
        /// </summary>
        [TestMethod]
        public void ReadPointsStdShort()
        {
            // Well formed data with 4 extra last byte
            byte[] streamData =
            {
                // max is 2
                2, 0, 0, 0,
                // count is 2
                2, 0, 0, 0,
                // x0 = 0
                0, 0, 0, 0,
                // y0 = 0
                0, 0, 0, 0,
                // x1 = 729
                217, 2, 0, 0,
                // y1 = 446
                190, 1, 0, 0,
                // extra
                1, 2, 3, 4
            };

            int expectedPointsCount = 2;
            int[] expectedPoints =
            {
                // X0, Y0
                0, 0,
                // X1, Y1
                729, 446
            };

            WangStream stream = new WangStream(streamData);
            int[] read = WangAnnotationStructureReader.ReadPoints(stream, 24);
            Assert.IsTrue(read != null);

            Assert.AreEqual(2, WangAnnotationTranslation.PointsLength(read));
            Assert.AreEqual(4, stream.AvailableBytes());

            for (int index = 0; index < expectedPointsCount; index ++)
            {
                Assert.AreEqual(expectedPoints[index*2], WangAnnotationTranslation.PointX(index, read));
                Assert.AreEqual(expectedPoints[index*2 + 1], WangAnnotationTranslation.PointY(index, read));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadPoints method when reading a set of points using exactly the available space.
        /// </summary>
        [TestMethod]
        public void ReadPointsStdExact()
        {
            // Well formed data
            byte[] streamData =
            {
                // max is 2
                2, 0, 0, 0,
                // count is 2
                2, 0, 0, 0,
                // x0 = 0
                0, 0, 0, 0,
                // y0 = 0
                0, 0, 0, 0,
                // x1 = 729
                217, 2, 0, 0,
                // y1 = 446
                190, 1, 0, 0
            };

            int expectedPointsCount = 2;
            int[] expectedPoints =
            {
                // X0, Y0
                0, 0,
                // X1, Y1
                729, 446
            };

            WangStream stream = new WangStream(streamData);
            int[] read = WangAnnotationStructureReader.ReadPoints(stream, 24);
            Assert.IsTrue(read != null);

            Assert.AreEqual(2, WangAnnotationTranslation.PointsLength(read));
            Assert.AreEqual(0, stream.AvailableBytes());

            for (int index = 0; index < expectedPointsCount; index++)
            {
                Assert.AreEqual(expectedPoints[index*2], WangAnnotationTranslation.PointX(index, read));
                Assert.AreEqual(expectedPoints[index*2 + 1], WangAnnotationTranslation.PointY(index, read));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadPoints method when reading a set of points with max greater than count
        /// </summary>
        [TestMethod]
        public void ReadPointsStdGreaterMax()
        {
            // Well formed data
            byte[] streamData =
            {
                // max is 2
                2, 0, 0, 0,
                // count is 1
                1, 0, 0, 0,
                // x0 = 729
                217, 2, 0, 0,
                // y0 = 446
                190, 1, 0, 0,
                // x1 = 0
                0, 0, 0, 0,
                // y1 = 0
                0, 0, 0, 0,
            };

            int expectedPointsCount = 1;
            int[] expectedPoints =
            {
                // X1, Y1
                729, 446
            };

            WangStream stream = new WangStream(streamData);
            int[] read = WangAnnotationStructureReader.ReadPoints(stream, 24);
            Assert.IsTrue(read != null);

            Assert.AreEqual(1, WangAnnotationTranslation.PointsLength(read));
            Assert.AreEqual(0, stream.AvailableBytes());

            for (int index = 0; index < expectedPointsCount; index++)
            {
                Assert.AreEqual(expectedPoints[index*2], WangAnnotationTranslation.PointX(index, read));
                Assert.AreEqual(expectedPoints[index*2 + 1], WangAnnotationTranslation.PointY(index, read));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadPoints method when not enough data are available to read the main properties
        /// or the set of points does not fit in the available space.
        /// </summary>
        [TestMethod]
        public void ReadPointsReadFailure()
        {
            // Well formed data
            byte[] streamData =
            {
                // max is 2
                2, 0, 0, 0,
                // count is 2
                2, 0, 0, 0,
                // x0 = 729
                217, 2, 0, 0,
                // y0 = 446
                190, 1, 0, 0,
                // x1 = 0
                0, 0, 0, 0,
                // y1 = 0
                0, 0, 0, 0,
            };

            // The stream contains exactly the correct amount of data.
            // With fewer data, an error should rise.
            for (int dataSize = 0; dataSize < streamData.Length - 1; dataSize ++)
            {
                WangStream stream = new WangStream(streamData);
                int[] read = WangAnnotationStructureReader.ReadPoints(stream, dataSize);
                Assert.IsTrue(read == null);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadPoints method when the data are not well formed:
        /// - count is greater than max
        /// </summary>
        [TestMethod]
        public void ReadPointsMalformed()
        {
            // Mall formed data: count is 2 while max is 1
            byte[] streamData =
            {
                // max is 1
                1, 0, 0, 0,
                // count is 2
                2, 0, 0, 0,
                // x0 = 729
                217, 2, 0, 0,
                // y0 = 446
                190, 1, 0, 0,
                // x1 = 0
                0, 0, 0, 0,
                // y1 = 0
                0, 0, 0, 0,
            };

            WangStream stream = new WangStream(streamData);
            int[] read = WangAnnotationStructureReader.ReadPoints(stream, streamData.Length);
            Assert.IsTrue(read == null);
        }

        /// <summary>
        /// Test the behavior of the ReadRotation method when reading a rotation using exactly the available space.
        /// </summary>
        [TestMethod]
        public void ReadRotationStdExact()
        {
            byte[] streamData =
            {
                1, 0, 0, 0,
                232, 3, 0, 0,
                200, 0, 0, 0,
                200, 0, 0, 0,
                145, 0, 0, 0,
                145, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                1, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangRotation read = WangAnnotationStructureReader.ReadRotation(stream, 56);
            Assert.IsTrue(read != null);

            Assert.AreEqual(145, read.OrigHRes);
            Assert.AreEqual(145, read.OrigVRes);
            Assert.AreEqual(1, read.Rotation);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadRotation method when reading a rotation using exactly the available space and contains different vertical and horizontal resolutions.
        /// </summary>
        [TestMethod]
        public void ReadRotationStdExactDifferentResolution()
        {
            byte[] streamData =
            {
                1, 0, 0, 0,
                232, 3, 0, 0,
                200, 0, 0, 0,
                200, 0, 0, 0,
                200, 0, 0, 0,
                145, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                1, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangRotation read = WangAnnotationStructureReader.ReadRotation(stream, 56);
            Assert.IsTrue(read != null);

            Assert.AreEqual(200, read.OrigHRes);
            Assert.AreEqual(145, read.OrigVRes);
            Assert.AreEqual(1, read.Rotation);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadRotation method when the available space does not match exactly the expected space.
        /// </summary>
        [TestMethod]
        public void ReadRotationInvalidSize()
        {
            // Well formed stream data
            byte[] streamData =
            {
                1, 0, 0, 0,
                232, 3, 0, 0,
                200, 0, 0, 0,
                200, 0, 0, 0,
                145, 0, 0, 0,
                145, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                1, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };

            for (int dataSize = 0; dataSize < streamData.Length - 1; dataSize ++)
            {
                WangStream stream = new WangStream(streamData);
                WangRotation read = WangAnnotationStructureReader.ReadRotation(stream, dataSize);
                Assert.IsTrue(read == null);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadDisplayText method when reading a display text using exactly the available space.
        /// </summary>
        [TestMethod]
        public void ReadDisplayTextStdExact()
        {
            // Well formed stream data
            byte[] streamData =
            {
                0, 0, 0, 0,
                123, 1, 0, 0,
                104, 1, 0, 0,
                5, 0, 0, 0,
                72, 101, 108, 108, 111, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangDisplayText read = WangAnnotationStructureReader.ReadDisplayText(stream, 24);
            Assert.IsTrue(read != null);

            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual("Hello", read.Text);
            Assert.AreEqual(360, (int) read.CreationScale);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadDisplayText method when the available space does not match exactly the expected space.
        /// </summary>
        [TestMethod]
        public void ReadDisplayTextDifferentSizes()
        {
            // Well formed stream data
            byte[] streamData =
            {
                0, 0, 0, 0,
                123, 1, 0, 0,
                104, 1, 0, 0,
                5, 0, 0, 0,
                72, 101, 108, 108, 111, 0, 0, 0
            };

            // It is ok to have a little bit more space for the text
            for (int dataSize = 21; dataSize < 24; dataSize++)
            {
                WangStream stream = new WangStream(streamData);
                WangDisplayText read = WangAnnotationStructureReader.ReadDisplayText(stream, dataSize);
                Assert.IsTrue(read != null);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadDisplayText method when the available space does not match the minimum space.
        /// </summary>
        [TestMethod]
        public void ReadDisplayTextInvalidSize()
        {
            // Well formed stream data
            byte[] streamData =
            {
                0, 0, 0, 0,
                123, 1, 0, 0,
                104, 1, 0, 0,
                5, 0, 0, 0,
                72, 101, 108, 108, 111, 0, 0, 0
            };

            // At least 4 values with 4 bytes are required.
            for (int dataSize = 0; dataSize < 21; dataSize++)
            {
                WangStream stream = new WangStream(streamData);
                WangDisplayText read = WangAnnotationStructureReader.ReadDisplayText(stream, dataSize);
                Assert.IsTrue(read == null);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadDib method.
        /// </summary>
        [TestMethod]
        public void ReadDibStd()
        {
            // Raw buffer not interpreted by the method.
            byte[] streamData =
            {
                1, 2, 3, 4, 5, 6
            };

            WangStream stream = new WangStream(streamData);
            byte[] read = WangAnnotationStructureReader.ReadDib(stream, streamData.Length);
            Assert.AreEqual(0, stream.AvailableBytes());
            Assert.AreEqual(streamData.Length, read.Length);
            for (int index = 0; index < streamData.Length; index ++)
            {
                Assert.AreEqual(streamData[index], read[index]);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadRectangle method.
        /// </summary>
        [TestMethod]
        public void ReadRectangleStd()
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

            WangStream stream = new WangStream(streamData);
            int[] coordinates = new int[4];
            Assert.IsTrue(WangAnnotationStructureReader.ReadRectangle(coordinates, stream));

            Assert.AreEqual(100, coordinates[WangAnnotationTranslation.LeftIndex]);
            Assert.AreEqual(1, coordinates[WangAnnotationTranslation.TopIndex]);
            Assert.AreEqual(200, coordinates[WangAnnotationTranslation.RightIndex]);
            Assert.AreEqual(55, coordinates[WangAnnotationTranslation.BottomIndex]);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadRectangle method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadRectangleInvalidSize()
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

            // The streamData is well formed. 
            // Just using a part of the data should cause an error to rise.

            for (int dataSize = 0; dataSize < streamData.Length - 1; dataSize++)
            {
                byte[] streamDataTooShort = new byte[dataSize];
                Array.Copy(streamData, 0, streamDataTooShort, 0, dataSize);

                WangStream stream = new WangStream(streamDataTooShort);
                int[] coordinates = new int[4];
                Assert.IsFalse(WangAnnotationStructureReader.ReadRectangle(coordinates, stream));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadRgbQuad method.
        /// </summary>
        [TestMethod]
        public void ReadRgbQuadStd()
        {
            // Well formed data
            byte[] streamData =
            {
                // Blue
                1,
                // Green
                2,
                // Red
                3,
                // Reserved
                0
            };

            WangStream stream = new WangStream(streamData);
            byte[] components = new byte[3];
            Assert.IsTrue(WangAnnotationStructureReader.ReadRgbQuad(components, stream));

            Assert.AreEqual(1, components[WangAnnotationTranslation.BlueIndex]);
            Assert.AreEqual(2, components[WangAnnotationTranslation.GreenIndex]);
            Assert.AreEqual(3, components[WangAnnotationTranslation.RedIndex]);
            Assert.AreEqual(0, stream.AvailableBytes());
        }

        /// <summary>
        /// Test the behavior of the ReadRgbQuad method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadRgbQuadInvalidSize()
        {
            // Well formed data
            byte[] streamData =
            {
                // Blue
                1,
                // Green
                2,
                // Red
                3,
                // Reserved
                0
            };

            // The streamData is well formed. 
            // Just using a part of the data should cause an error to rise.

            for (int dataSize = 0; dataSize < streamData.Length - 1; dataSize++)
            {
                byte[] streamDataTooShort = new byte[dataSize];
                Array.Copy(streamData, 0, streamDataTooShort, 0, dataSize);

                WangStream stream = new WangStream(streamDataTooShort);
                byte[] components = new byte[3];
                Assert.IsFalse(WangAnnotationStructureReader.ReadRgbQuad(components, stream));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadHyperlink method.
        /// </summary>
        [TestMethod]
        public void ReadHyperlink()
        {
            // Well formed data
            byte[] streamData =
            {
                1, 0, 0, 0,
                14, 0, 0, 0,
                115, 111, 117, 114,
                99, 101, 92, 48,
                48, 49, 46, 106,
                112, 103, 1, 0,
                0, 0, 49, 0,
                0, 0, 0, 0,
                0, 0, 0
            };


            WangStream stream = new WangStream(streamData);
            WangHyperlink hyperlink = WangAnnotationStructureReader.ReadHyperlink(stream, streamData.Length);

            Assert.AreEqual(false, hyperlink.CanRemoveHyperlink);
            Assert.AreEqual(false, hyperlink.InternalLink);
            Assert.AreEqual("source\\001.jpg", hyperlink.Link);
            Assert.AreEqual("1", hyperlink.Location);
            Assert.AreEqual("", hyperlink.WorkingDirectory);
        }

        /// <summary>
        /// Test the behavior of the ReadHyperlink method.
        /// </summary>
        [TestMethod]
        public void ReadHyperlink2()
        {
            // Well formed data
            byte[] streamData =
            {
                1, 0, 0, 0,
                22, 0, 0, 0,
                104, 116, 116, 112,
                58, 47, 47, 119,
                119, 119, 46, 111,
                114, 112, 97, 108,
                105, 115, 46, 99,
                111, 109, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0
            };


            WangStream stream = new WangStream(streamData);
            WangHyperlink hyperlink = WangAnnotationStructureReader.ReadHyperlink(stream, streamData.Length);

            Assert.AreEqual(false, hyperlink.CanRemoveHyperlink);
            Assert.AreEqual(false, hyperlink.InternalLink);
            Assert.AreEqual("http://www.orpalis.com", hyperlink.Link);
            Assert.AreEqual("", hyperlink.Location);
            Assert.AreEqual("", hyperlink.WorkingDirectory);
        }

        /// <summary>
        /// Test the behavior of the ReadHyperlink method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadHyperlinkInvalidSize()
        {
            // Well formed data
            byte[] streamData =
            {
                1, 0, 0, 0,
                1, 0, 0, 0,
                104,
                1, 0, 0, 0,
                104,
                1, 0, 0, 0,
                104,
                0, 0, 0, 0
            };

            int[] dataSizes =
            {
                0, 4, 8, 9, 13, 14, 18, 19
            };

            foreach (var dataSize in dataSizes)
            {
                byte[] streamDataTooShort = new byte[dataSize];
                Array.Copy(streamData, streamDataTooShort, dataSize);
                WangStream stream = new WangStream(streamDataTooShort);
                WangHyperlink hyperlink = WangAnnotationStructureReader.ReadHyperlink(stream, dataSize);
                Assert.AreEqual(null, hyperlink);
            }
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when reading a standard well defined logfont.
        /// </summary>
        [TestMethod]
        public void ReadLogfontArial()
        {
            // Well formed data with
            byte[] streamData =
            {
                12, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 144, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 65, 114,
                105, 97, 108, 0, 145, 124, 247, 49, 145, 124,
                8, 6, 12, 0, 136, 0, 126, 0, 0, 0,
                0, 0, 0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangLogFont read = WangAnnotationStructureReader.ReadLogfont(stream);
            Assert.IsTrue(read != null);

            Assert.AreEqual(12, read.Height);
            Assert.AreEqual(0, read.Width);
            Assert.AreEqual(0, read.Escapement);
            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual(400, read.Weight);
            Assert.AreEqual(false, read.Italic);
            Assert.AreEqual(false, read.Underline);
            Assert.AreEqual(false, read.StrikeOut);
            Assert.AreEqual(0, read.CharSet);
            Assert.AreEqual(0, read.OutPrecision);
            Assert.AreEqual(0, read.ClipPrecision);
            Assert.AreEqual(0, read.Quality);
            Assert.AreEqual(0, read.PitchAndFamily);
            Assert.AreEqual("Arial", read.FaceName);
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when reading a standard well defined logfont.
        /// </summary>
        [TestMethod]
        public void ReadLogfontZero()
        {
            // Well formed data (only 0s)
            byte[] streamData = new byte[56];

            WangStream stream = new WangStream(streamData);
            WangLogFont read = WangAnnotationStructureReader.ReadLogfont(stream);
            Assert.IsTrue(read != null);

            Assert.AreEqual(0, read.Height);
            Assert.AreEqual(0, read.Width);
            Assert.AreEqual(0, read.Escapement);
            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual(0, read.Weight);
            Assert.AreEqual(false, read.Italic);
            Assert.AreEqual(false, read.Underline);
            Assert.AreEqual(false, read.StrikeOut);
            Assert.AreEqual(0, read.CharSet);
            Assert.AreEqual(0, read.OutPrecision);
            Assert.AreEqual(0, read.ClipPrecision);
            Assert.AreEqual(0, read.Quality);
            Assert.AreEqual(0, read.PitchAndFamily);
            Assert.AreEqual("", read.FaceName);
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when reading a standard well defined logfont.
        /// </summary>
        [TestMethod]
        public void ReadLogfontZeroSignificant()
        {
            // Well formed data with. Zero significant data and trailing stuff
            byte[] streamData =
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 223,
                10, 0, 219, 49, 145, 124, 247, 49, 145, 124,
                8, 6, 12, 0, 136, 0, 126, 0, 0, 0,
                0, 0, 0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangLogFont read = WangAnnotationStructureReader.ReadLogfont(stream);
            Assert.IsTrue(read != null);

            Assert.AreEqual(0, read.Height);
            Assert.AreEqual(0, read.Width);
            Assert.AreEqual(0, read.Escapement);
            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual(0, read.Weight);
            Assert.AreEqual(false, read.Italic);
            Assert.AreEqual(false, read.Underline);
            Assert.AreEqual(false, read.StrikeOut);
            Assert.AreEqual(0, read.CharSet);
            Assert.AreEqual(0, read.OutPrecision);
            Assert.AreEqual(0, read.ClipPrecision);
            Assert.AreEqual(0, read.Quality);
            Assert.AreEqual(0, read.PitchAndFamily);
            Assert.AreEqual("", read.FaceName);
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when reading a standard well defined logfont.
        /// </summary>
        [TestMethod]
        public void ReadLogfontArialUnderline()
        {
            // Well formed data with
            byte[] streamData =
            {
                12, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 144, 1, 0, 0,
                0, 1, 0, 0, 0, 0, 0, 0, 65, 114,
                105, 97, 108, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangLogFont read = WangAnnotationStructureReader.ReadLogfont(stream);
            Assert.IsTrue(read != null);

            Assert.AreEqual(12, read.Height);
            Assert.AreEqual(0, read.Width);
            Assert.AreEqual(0, read.Escapement);
            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual(400, read.Weight);
            Assert.AreEqual(false, read.Italic);
            Assert.AreEqual(true, read.Underline);
            Assert.AreEqual(false, read.StrikeOut);
            Assert.AreEqual(0, read.CharSet);
            Assert.AreEqual(0, read.OutPrecision);
            Assert.AreEqual(0, read.ClipPrecision);
            Assert.AreEqual(0, read.Quality);
            Assert.AreEqual(0, read.PitchAndFamily);
            Assert.AreEqual("Arial", read.FaceName);
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when reading a standard well defined logfont.
        /// </summary>
        [TestMethod]
        public void ReadLogfontGeorgia()
        {
            // Well formed data with
            byte[] streamData =
            {
                14, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 188, 2, 0, 0,
                0, 0, 1, 0, 3, 2, 1, 18, 71, 101,
                111, 114, 103, 105, 97, 0, 32, 71, 111, 116,
                104, 105, 99, 32, 77, 101, 100, 105, 117, 109,
                0, 0, 0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangLogFont read = WangAnnotationStructureReader.ReadLogfont(stream);
            Assert.IsTrue(read != null);

            Assert.AreEqual(14, read.Height);
            Assert.AreEqual(0, read.Width);
            Assert.AreEqual(0, read.Escapement);
            Assert.AreEqual(0, read.Orientation);
            Assert.AreEqual(700, read.Weight);
            Assert.AreEqual(false, read.Italic);
            Assert.AreEqual(false, read.Underline);
            Assert.AreEqual(true, read.StrikeOut);
            Assert.AreEqual(0, read.CharSet);
            Assert.AreEqual(3, read.OutPrecision);
            Assert.AreEqual(2, read.ClipPrecision);
            Assert.AreEqual(1, read.Quality);
            Assert.AreEqual(18, read.PitchAndFamily);
            Assert.AreEqual("Georgia", read.FaceName);
        }

        /// <summary>
        /// Test the behavior of the ReadLogfont method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadLogfontReadFailure()
        {
            for (int dataSize = 0; dataSize < 56; dataSize ++)
            {
                byte[] streamData = new byte[dataSize];
                WangStream stream = new WangStream(streamData);
                Assert.AreEqual(null, WangAnnotationStructureReader.ReadLogfont(stream));
            }
        }

        /// <summary>
        /// Test the behavior of the ReadMarkAttributes method when reading a standard well defined mark attributes.
        /// </summary>
        [TestMethod]
        public void ReadMarkAttributes1()
        {
            // Well formed data
            byte[] streamData =
            {
                10, 0, 0, 0, 229, 1, 0, 0, 202, 0,
                0, 0, 233, 3, 0, 0, 155, 2, 0, 0,
                0, 255, 255, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 12, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 144, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 65, 114, 105, 97,
                108, 0, 145, 124, 247, 49, 145, 124, 8, 6,
                12, 0, 136, 0, 126, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 44, 224, 10, 0, 0, 0,
                0, 0, 68, 3, 51, 88, 1, 0, 0, 0,
                63, 248, 15, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangMarkAttributes read = WangAnnotationStructureReader.ReadMarkAttributes(stream, 164);
            Assert.IsTrue(read != null);

            Assert.AreEqual(12, read.LogFont.Height);
            Assert.AreEqual(0, read.LogFont.Width);
            Assert.AreEqual(0, read.LogFont.Escapement);
            Assert.AreEqual(0, read.LogFont.Orientation);
            Assert.AreEqual(400, read.LogFont.Weight);
            Assert.AreEqual(false, read.LogFont.Italic);
            Assert.AreEqual(false, read.LogFont.Underline);
            Assert.AreEqual(false, read.LogFont.StrikeOut);
            Assert.AreEqual(0, read.LogFont.CharSet);
            Assert.AreEqual(0, read.LogFont.OutPrecision);
            Assert.AreEqual(0, read.LogFont.ClipPrecision);
            Assert.AreEqual(0, read.LogFont.Quality);
            Assert.AreEqual(0, read.LogFont.PitchAndFamily);
            Assert.AreEqual("Arial", read.LogFont.FaceName);
            Assert.AreEqual(false, read.Highlighting);
            Assert.AreEqual(false, read.Transparent);
            Assert.AreEqual((uint) 0, read.LineSize);
            Assert.AreEqual(true, read.Visible);
            Assert.AreEqual(485, read.Bounds[0]);
            Assert.AreEqual(202, read.Bounds[1]);
            Assert.AreEqual(1001, read.Bounds[2]);
            Assert.AreEqual(667, read.Bounds[3]);
            Assert.AreEqual(0, read.Color1[0]);
            Assert.AreEqual(255, read.Color1[1]);
            Assert.AreEqual(255, read.Color1[2]);
            Assert.AreEqual(0, read.Color2[0]);
            Assert.AreEqual(0, read.Color2[1]);
            Assert.AreEqual(0, read.Color2[2]);
        }

        /// <summary>
        /// Test the behavior of the ReadMarkAttributes method when reading a standard well defined mark attributes.
        /// </summary>
        [TestMethod]
        public void ReadMarkAttributes2()
        {
            // Well formed data
            byte[] streamData =
            {
                15, 0, 0, 0, 64, 1, 0, 0, 68, 0,
                0, 0, 156, 4, 0, 0, 16, 3, 0, 0,
                192, 192, 192, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 252, 253, 54, 88, 1, 0, 0, 0,
                63, 248, 15, 0, 88, 254, 54, 88, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangMarkAttributes read = WangAnnotationStructureReader.ReadMarkAttributes(stream, 164);
            Assert.IsTrue(read != null);

            Assert.AreEqual(0, read.LogFont.Height);
            Assert.AreEqual(0, read.LogFont.Width);
            Assert.AreEqual(0, read.LogFont.Escapement);
            Assert.AreEqual(0, read.LogFont.Orientation);
            Assert.AreEqual(0, read.LogFont.Weight);
            Assert.AreEqual(false, read.LogFont.Italic);
            Assert.AreEqual(false, read.LogFont.Underline);
            Assert.AreEqual(false, read.LogFont.StrikeOut);
            Assert.AreEqual(0, read.LogFont.CharSet);
            Assert.AreEqual(0, read.LogFont.OutPrecision);
            Assert.AreEqual(0, read.LogFont.ClipPrecision);
            Assert.AreEqual(0, read.LogFont.Quality);
            Assert.AreEqual(0, read.LogFont.PitchAndFamily);
            Assert.AreEqual("", read.LogFont.FaceName);
            Assert.AreEqual(true, read.Highlighting);
            Assert.AreEqual(false, read.Transparent);
            Assert.AreEqual((uint) 0, read.LineSize);
            Assert.AreEqual(true, read.Visible);
            Assert.AreEqual(320, read.Bounds[0]);
            Assert.AreEqual(68, read.Bounds[1]);
            Assert.AreEqual(1180, read.Bounds[2]);
            Assert.AreEqual(784, read.Bounds[3]);
            Assert.AreEqual(192, read.Color1[0]);
            Assert.AreEqual(192, read.Color1[1]);
            Assert.AreEqual(192, read.Color1[2]);
            Assert.AreEqual(0, read.Color2[0]);
            Assert.AreEqual(0, read.Color2[1]);
            Assert.AreEqual(0, read.Color2[2]);
        }

        /// <summary>
        /// Test the behavior of the ReadMarkAttributes method when reading a standard well defined mark attributes.
        /// </summary>
        [TestMethod]
        public void ReadMarkAttributes3()
        {
            // Well formed data
            byte[] streamData =
            {
                7, 0, 0, 0, 81, 0, 0, 0, 50, 0,
                0, 0, 38, 5, 0, 0, 205, 0, 0, 0,
                0, 0, 255, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 24, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 144, 1, 0, 0, 1, 0,
                0, 255, 3, 2, 1, 50, 77, 111, 100, 101,
                114, 110, 0, 124, 247, 49, 145, 124, 8, 6,
                12, 0, 144, 0, 126, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 44, 224, 10, 0, 0, 0,
                0, 0, 36, 3, 51, 88, 1, 0, 0, 0,
                63, 248, 15, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };

            WangStream stream = new WangStream(streamData);
            WangMarkAttributes read = WangAnnotationStructureReader.ReadMarkAttributes(stream, 164);
            Assert.IsTrue(read != null);

            Assert.AreEqual(24, read.LogFont.Height);
            Assert.AreEqual(0, read.LogFont.Width);
            Assert.AreEqual(0, read.LogFont.Escapement);
            Assert.AreEqual(0, read.LogFont.Orientation);
            Assert.AreEqual(400, read.LogFont.Weight);
            Assert.AreEqual(true, read.LogFont.Italic);
            Assert.AreEqual(false, read.LogFont.Underline);
            Assert.AreEqual(false, read.LogFont.StrikeOut);
            Assert.AreEqual(255, read.LogFont.CharSet);
            Assert.AreEqual(3, read.LogFont.OutPrecision);
            Assert.AreEqual(2, read.LogFont.ClipPrecision);
            Assert.AreEqual(1, read.LogFont.Quality);
            Assert.AreEqual(50, read.LogFont.PitchAndFamily);
            Assert.AreEqual("Modern", read.LogFont.FaceName);
            Assert.AreEqual(false, read.Highlighting);
            Assert.AreEqual(false, read.Transparent);
            Assert.AreEqual((uint) 0, read.LineSize);
            Assert.AreEqual(true, read.Visible);
            Assert.AreEqual(81, read.Bounds[0]);
            Assert.AreEqual(50, read.Bounds[1]);
            Assert.AreEqual(1318, read.Bounds[2]);
            Assert.AreEqual(205, read.Bounds[3]);
            Assert.AreEqual(0, read.Color1[0]);
            Assert.AreEqual(0, read.Color1[1]);
            Assert.AreEqual(255, read.Color1[2]);
            Assert.AreEqual(0, read.Color2[0]);
            Assert.AreEqual(0, read.Color2[1]);
            Assert.AreEqual(0, read.Color2[2]);
        }

        /// <summary>
        /// Test the behavior of the ReadMarkAttributes method when not enough data are available.
        /// </summary>
        [TestMethod]
        public void ReadMarkAttributesReadFailure()
        {
            for (int dataSize = 0; dataSize < 164; dataSize++)
            {
                byte[] streamData = new byte[dataSize];
                WangStream stream = new WangStream(streamData);
                Assert.AreEqual(null, WangAnnotationStructureReader.ReadMarkAttributes(stream, dataSize));
            }
        }
    }
}
