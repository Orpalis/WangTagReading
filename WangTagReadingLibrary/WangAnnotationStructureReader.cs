using System;
using System.Text;
#if DEBUG
using System.Diagnostics;

#endif // DEBUG

namespace WangTagReadingLibrary
{
    /*
     * TODO - David Ometto - 2016-11-24 - We have to support all available annotation marks types.
     * 
     *  The following are existing annotation mark types not supported at the moment by the software.
     *  
     *  Form
     *  Definition: A black and white image included by indirect reference. Only one form mark is allowed per image. It is placed on top of the base image before all other marks. If the image is not found at display time, an error is returned.
     *  Attributes:  bTransparent (must be set)
     *  Named Blocks and Associated Structures:
     *  - OiAnoDat      AN_NEW_ROTATE_STRUCT
     *  - OiFilNam      AN_NAME_STRUCT
     *  - OiDIB         Not used
     *  - OiGroup       STR
     *  - OiIndex       STR
     *  - OiAnText      Not used
     *  
     * Image Reference
     * Definition: An image included by reference. If the image is not found at display time, an error is returned.
     * Attributes: bTransparent
     * Named Blocks and Associated Structures
     * - OiAnoDat       AN_NEW_ROTATE_STRUCT
     * - OiFilNam       AN_NAME_STRUCT
     * - OiDIB          Not used
     * - OiGroup        STR
     * - OiIndex        STR
     * - OiAnText       Not used
     * Note:  To reference images at different locations on different PCs, you can use the following technique to specify the image filename. For example, to reference c:\oi\bw.tif, first define a section in the win.ini file called [O/i ImagePath], with an entry such as x=c:\oi\, where x is an integer from 1 to 9. After this setting is defined, use the filename x:bw.tif to access c:\oi\bw.tif. The path specified for x (c:\oi\) concatenates with what follows after the colon in the filename (bw.tif) to produce c:\oi\bw.tif.
     * 
     */

    /// <summary>
    /// The WangAnnotationStructureReader class offers a couple of method for reading Wang annotation related structures.
    /// </summary>
    internal static class WangAnnotationStructureReader
    {
        /// <summary>
        /// ReadNamedBlockHeader reads a named block header.
        /// </summary>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="count">The number of bytes to character to read in the buffer.</param>
        /// <returns>The data read, null if an error occurred.</returns>
        public static WangNamedBlockHeader ReadNamedBlockHeader(IWangStream stream, int count)
        {
            // The expected data size is fixed.
            if (count == 12)
            {
                /*
                 * 8 bytes  =  name of named block
                 * 4 bytes  =  size (n) of named block
                 * 4 bytes  =  reserved. Only present and necessary with  Intel 16-bit format. Skip on read and write as zeros.
                 */
                WangNamedBlockHeader header =
                    new WangNamedBlockHeader(WangAnnotationStructureReader.ReadCharString(stream, 8), stream.ReadInt32());

                // TODO - David Ometto - 2016-11-21 - Add support for non Intel, big endian and 16 bit fun to handle here
                // Only present and necessary with  Intel 16-bit format. Skip on read and write as zeros.

                return header;
            }
            return null;
        }

        /// <summary>
        /// ReadCharString reads a character string for a Wang annotation, encoded as a 1 byte character null terminated string.
        /// </summary>
        /// <remarks>
        /// This method applies to 1 byte character strings with a fixed number of characters.
        /// Typically referecend as char xxx[count] within the specificaitons.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="count">The number of bytes to character to read in the buffer.</param>
        /// <returns>The byte read.</returns>
        public static string ReadCharString(IWangStream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.ReadBytes(buffer, count);
            // Sligth optimization compared to Encoding.ASCII.GetString(buffer, 0, count).TrimEnd('\0');
            // to avoid creation of an extra string
            int actualCount = 0;
            while (actualCount < count && buffer[actualCount] != 0)
            {
                actualCount ++;
            }
            return Encoding.ASCII.GetString(buffer, 0, actualCount);
        }

        /// <summary>
        /// ReadPoints reads a structure designed to hold points.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a AN_POINTS structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data to read.</param>
        /// <returns>The data read, null if an error occured.</returns>
        public static int[] ReadPoints(IWangStream stream, int dataSize)
        {
            /*
             * This implementation is based on the following C++ typedef:
             * typedef struct tagAnPoints{
             *   int nMaxPoints;                // The maximum number of points; must 
             *                                  // be equal to the value of nPoints.
             *   int nPoints;                   // The current number of points.
             *   POINT ptPoint[1];              // Points marking the beginning and 
             *                                  // ending of the line segment(s); in 
             *                                  // FULLSIZE (not scaled) coordinates 
             *                                  // relative to the upper left corner 
             *                                  // of lrBounds in 
             *                                  // OIAN_MARK_ATTRIBUTES.
             *   } AN_POINTS;
             */

            // We need at least two integers.
            if (dataSize < 8)
            {
                return null;
            }

            int max = stream.ReadInt32();
            int count = stream.ReadInt32();

            // Although the available spec explains the opposite, max and count may be different
            if (max < count)
            {
                return null;
            }

            // The size of the data is exactly the size of two integers
            // plus the size for the maximum number of points.
            if ((8 + max*2*4) != dataSize)
            {
                return null;
            }

            int coordinatesCount = 2*count;
            int[] readData = new int[coordinatesCount];
            stream.ReadInts32(readData, coordinatesCount);

            // Although the available spec explains the opposite, max and count may be different
            // and we have to skip the unused data.
            // 2 coordinates, 4 bytes per coordinate
            stream.SkipBytes((max - count)*2*4);

            return readData;
        }

        /// <summary>
        /// ReadRotation reads a scaling and resolution information for image marks.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a AN_NEW_ROTATE_STRUCT structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data to read.</param>
        /// <returns>The data read, null if an error occured.</returns>
        public static WangRotation ReadRotation(IWangStream stream, int dataSize)
        {
            // The expected data size is fixed.
            if (dataSize == 56)
            {
                /*
                 * This implementation is based on the following C++ typedef:
                 * typedef struct tagAnNewRotateStruct{
                 *   int rotation;            // 1=Original
                 *                            // 2=Rotate right (90 degrees clockwise)
                 *                            // 3=Flip (180 degrees clockwise)
                 *                            // 4=Rotate left (270 degrees clockwise)
                 *                            // 5=Vertical mirror (reflected around a 
                 *                            // vertical line)
                 *                            // 6=Vertical mirror + Rotate right
                 *                            // 7=Vertical mirror + Flip
                 *                            // 8=Vertical mirror + Rotate left
                 *  int scale;                // Set to 1000.
                 *  int nHRes;                // Set to value of nOrigHRes.
                 *  int nVRes;                // Set to value of nOrigVRes.
                 *  int nOrigHRes;            // Resolution of image mark in DPI.
                 *  int nOrigVRes;            // Resolution of image mark in DPI.
                 *  BOOL bReserved1;          // Set to 0.
                 *  BOOL bReserved2;          // Set to 0.
                 *  int nReserved[6];
                 *  }AN_NEW_ROTATE_STRUCT;
                 */

                int rotation = stream.ReadInt32();
                int scale = stream.ReadInt32();
#if DEBUG
                Debug.Assert(scale == 1000);
#endif //DEBUG
                int nHRes = stream.ReadInt32();
                int nVRes = stream.ReadInt32();
                int horizontalResolution = stream.ReadInt32();
                int verticalResolution = stream.ReadInt32();
#if DEBUG
                // This equality is part of the specifications but does not occur every time.
                // Debug.Assert(nHRes == readData.OrigHRes);
                // This equality is part of the specifications but does not occur every time.
                // Debug.Assert(nVRes == readData.OrigVRes);
#endif //DEBUG
                stream.SkipBytes(4);
                stream.SkipBytes(4);
                stream.SkipBytes(6*4);
                return new WangRotation(rotation, horizontalResolution, verticalResolution);
            }
            return null;
        }

        /// <summary>
        /// ReadMarkAttributes reads a structure designed to hold mark attributes.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a OIAN_MARK_ATTRIBUTES structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data to read.</param>
        /// <returns>The data read.</returns>
        public static WangMarkAttributes ReadMarkAttributes(IWangStream stream, int dataSize)
        {
            // The expected data size is fixed.
            if (dataSize == 164)
            {
                /* 
                 * Type         Name                Description
                 * UINT         uType               The type of the mark.
                 * LRECT        lrBounds            Rectangle in FULLSIZE units; equivalent to type RECT. 
                 *                                  Can be a rectangle or two points.
                 * RGBQUAD      rgbColor1           The main color; for example, the color of all lines, all rectangles, and standalone text.
                 * RGBQUAD      rgbColor2           The secondary color; for example, the color of the text of an Attach-a-Note.
                 * BOOL         bHighlighting       TRUE - The mark is drawn highlighted. Highlighting 
                 *                                  performs the same function as a highlighting marker on a 
                 *                                  piece of paper. Valid only for lines, rectangles, and 
                 *                                  freehand.
                 * BOOL         bTransparent        TRUE - The mark is drawn transparent. A transparent 
                 *                                  mark does not draw white pixels. That is, transparent 
                 *                                  replaces white pixels with whatever is behind those pixels. 
                 *                                  Available only for images.
                 * UINT         uLineSize           The width of the line in pixels.
                 * UINT         uReserved1          Reserved; must be set to 0.
                 * UINT         uReserved2          Reserved; must be set to 0.
                 * LOGFONT      lfFont              The font information for the text, consisting of standard 
                                                    font attributes of font size, name, style, effects, and 
                                                    background color.
                 * DWORD        bReserved3          Reserved; must be set to 0.
                 * time_t       Time                The time that the mark was first saved, in seconds, from 
                                                    00:00:00 1-1-1970 GMT. Every annotation mark has 
                                                    time as one of its attributes. If you do not set the time before 
                                                    the file is saved, the time is set to the date and time that the 
                                                    save was initiated. This time is in the form returned by the 
                                                    "time" C call, which is the number of seconds since 
                                                    midnight 00:00:00 on 1-1-1970 GMT. If necessary, refer 
                                                    to your C documentation for a more detailed description.
                 * BOOL         bVisible            TRUE - The mark is currently set to be visible. 
                 *                                  Annotation marks can be visible or hidden.
                 * DWORD        dwReserved4         Reserved; must be set to 0x0FF83F.
                 * long         lReserved[10]       Must be set to 0.                                   
                 */

                uint uintMarkType = stream.ReadUint32();
                WangMarkType type = uintMarkType < ConversionWangMarkTypes.Length
                    ? ConversionWangMarkTypes[uintMarkType]
                    : WangMarkType.Invalid;
                WangMarkAttributes readData = new WangMarkAttributes(type);

                if (!WangAnnotationStructureReader.ReadRectangle(readData.Bounds, stream))
                {
                    return null;
                }
                if (!WangAnnotationStructureReader.ReadRgbQuad(readData.Color1, stream))
                {
                    return null;
                }
                if (!WangAnnotationStructureReader.ReadRgbQuad(readData.Color2, stream))
                {
                    return null;
                }

                readData.Highlighting = WangAnnotationStructureReader.ReadBool(stream);
                readData.Transparent = WangAnnotationStructureReader.ReadBool(stream);
                readData.LineSize = stream.ReadUint32();
                uint uReserved1 = stream.ReadUint32();
#if DEBUG
                // Reserved; must be set to 0.
                Debug.Assert(uReserved1 == 0);
#endif // DEBUG
                uint uReserved2 = stream.ReadUint32();
#if DEBUG
                // Reserved; must be set to 0.
                Debug.Assert(uReserved2 == 0);
#endif // DEBUG
                readData.LogFont = ReadLogfont(stream);

                uint bReserved3 = stream.ReadUint32();
#if DEBUG
                // Reserved; must be set to 0.
                // For some reason several file have a non 0 value there.
                // Debug.Assert(bReserved3 == 0);
#endif // DEBUG
                // Skip time
                // TODO - David Ometto - 2016-11-21 - Add support for reading time_t structure.
                stream.SkipBytes(8);

                readData.Visible = WangAnnotationStructureReader.ReadBool(stream);

                // Reserved; must be set to 0x0FF83F... but for some reason several file have a non 0 value there.
                uint dwReserved4 = stream.ReadUint32();

                // Skip 10 reserved long must be set to 0 (which is not true all of the time).
                stream.SkipBytes(40);

                return readData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ReadDisplayText reads a structure designed to hold display text.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a OIAN_TEXTPRIVDATA structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data to read.</param>
        /// <returns>The data read.</returns>
        public static WangDisplayText ReadDisplayText(IWangStream stream, int dataSize)
        {
            // At least 4 values with 4 bytes are required
            if (dataSize >= 16)
            {
                /* 
                 * Type         Name                Description
                 * int          nCurrentOrientation Angle of text baseline to image in tenths of a degree; valid values are 0, 900, 1800, 2700.
                 * 
                 * UINT         uReserved1          Always 1000 when writing ignore when reading.
                 * 
                 * UINT         uCreationScale      Always 72000 divided by the vertical resolution of the  base image when writing.
                 *                                  Used to modify the Attributes.lfFont.lfHeight variable for display.
                 * 
                 * UINT         uAnoTextLength      64K byte limit (32K for multi-byte data) for Attach-a-Note, typed text, text from file; 
                 *                                  255 byte limit for text stamp.
                 * 
                 * char         szAnoText[*]        Text string for text mark types.
                 */


                int orientation = stream.ReadInt32();
#if DEBUG
                Debug.Assert(orientation == 0 || orientation == 900 || orientation == 1800 || orientation == 2700);
#endif // DEBUG
                UInt32 uReserved1 = stream.ReadUint32();
#if DEBUG
                // 1000 is the value described within the specs but we have some cases with something else.
                // Debug.Assert(uReserved1 == 1000 );
#endif // DEBUG
                uint creationScale = stream.ReadUint32();
                int textSize = (int) stream.ReadUint32();
                // It is necessary to have enough data for the characters.
                if (textSize <= (dataSize - 16))
                {
                    string text = WangAnnotationStructureReader.ReadCharString(stream, textSize);
                    stream.SkipBytes(dataSize - 16 - textSize);
                    return new WangDisplayText(orientation, creationScale, text);
                }
            }
            return null;
        }

        /// <summary>
        /// ReadDib reads a structure designed to hold a DIB.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a DIB structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data to read.</param>
        /// <returns>The data read.</returns>
        public static byte[] ReadDib(IWangStream stream, int dataSize)
        {
            // The data are read as a raw buffer. The raw buffer containing the
            // DIB will later be provided to methods with access to GdPicture.NET
            // features so they can make use of existing features to read these
            // bytes.
            byte[] dibInfo = new byte[dataSize];
            stream.ReadBytes(dibInfo, dataSize);
            return dibInfo;
        }

        /// <summary>
        /// ReadRectangle reads a structure designed to hold a rectangle for a Wang Annotation.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a RECT structure.
        /// </remarks>
        /// <param name="coordinates">An array to retrieve the coordinates of the rectangle.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>true if success otherwise false.</returns>
        public static bool ReadRectangle(int[] coordinates, IWangStream stream)
        {
            if (stream.AvailableBytes() >= 4*4)
            {
                // The order is left, top, right, bottom.
                stream.ReadInts32(coordinates, 4);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ReadRgbQuad reads a structure designed to hold a color for a Wang annotation.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of an RGBQUAD structure.
        /// </remarks>
        /// <param name="colors">An array to retrieve the components for the color.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>true if success otherwise false.</returns>
        public static bool ReadRgbQuad(byte[] colors, IWangStream stream)
        {
            if (stream.AvailableBytes() >= 4)
            {
                // The order is blue, green, red.
                stream.ReadBytes(colors, 3);
                // Skip the reserved data field.
                stream.SkipBytes(1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ReadHyperlink reads a structure designed to hold an hyperlink for a Wang annotation.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of an HYPERLINK_NB  structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The data size.</param>
        /// <returns>the hyperlink.</returns>
        public static WangHyperlink ReadHyperlink(IWangStream stream, int dataSize)
        {
            /* 
             * HYPERLINK_NB
             * Type         Name                Description
             * int          nVersion            The version number of this data.
             * int          nLinkSize           The size of the link string in bytes.
             * char         szLinkString        The variable length multi-byte name string.
             * int          nLocationSize       The size of the location string.
             * char         szLocationString    The variable length multi-byte location string.
             * int          nWorkDirSize        The size of the working directory string.
             * char         szWorkDirString     The variable length multi-byte working directory string.
             * int          nFlags              One or more of the following flags OR’ed together:
             *                                  1 = Can remove hyperlink from mark.
             *                                  2 = Hyperlink refers to this document.
             */

            if (stream.AvailableBytes() < 4)
            {
                return null;
            }

            int version = stream.ReadInt32();

            string[] stringInformation = new string[3];
            for (int index = 0; index < stringInformation.Length; index ++)
            {
                if (stream.AvailableBytes() < 4)
                {
                    return null;
                }

                int size = stream.ReadInt32();

                if (stream.AvailableBytes() < size)
                {
                    return null;
                }

                stringInformation[index] = ReadCharString(stream, size);
            }

            if (stream.AvailableBytes() < 4)
            {
                return null;
            }

            int flag = stream.ReadInt32();

            return new WangHyperlink(stringInformation[0], stringInformation[1], stringInformation[2],
                ((int) flag & 1) != 0, ((int) flag & 2) != 0);
        }

        /// <summary>
        /// ReadLogFont reads a structure designed to hold font attributes.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a OIAN_MARK_ATTRIBUTES structure.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>The data read.</returns>
        public static WangLogFont ReadLogfont(IWangStream stream)
        {
            if (stream.AvailableBytes() >= 56)
            {
                /*
                 * This implementation is based on the following C++ typedef:
                 * typedef struct tagLOGFONT {
                 *  LONG  lfHeight;
                 *  LONG  lfWidth;
                 *  LONG  lfEscapement;
                 *  LONG  lfOrientation;
                 *  LONG  lfWeight;
                 *  BYTE  lfItalic;
                 *  BYTE  lfUnderline;
                 *  BYTE  lfStrikeOut;
                 *  BYTE  lfCharSet;
                 *  BYTE  lfOutPrecision;
                 *  BYTE  lfClipPrecision;
                 *  BYTE  lfQuality;
                 *  BYTE  lfPitchAndFamily;
                 *  TCHAR lfFaceName[LF_FACESIZE];
                 *  } LOGFONT, *PLOGFONT;
                 */
                WangLogFont readData = new WangLogFont();
                readData.Height = stream.ReadInt32();
                readData.Width = stream.ReadInt32();
                readData.Escapement = stream.ReadInt32();
                readData.Orientation = stream.ReadInt32();
                readData.Weight = stream.ReadInt32();
                readData.Italic = stream.ReadByte() != 0;
                readData.Underline = stream.ReadByte() != 0;
                readData.StrikeOut = stream.ReadByte() != 0;
                readData.CharSet = stream.ReadByte();
                readData.OutPrecision = stream.ReadByte();
                readData.ClipPrecision = stream.ReadByte();
                readData.Quality = stream.ReadByte();
                readData.PitchAndFamily = stream.ReadByte();
                readData.FaceName = ReadCharString(stream, 28);
                return readData;
            }
            return null;
        }

        /// <summary>
        /// ReadBool reads a 4 bytes boolean value for a Wang annotation.
        /// </summary>
        /// <remarks>
        /// This methods aplies when the specifications are mentionning the availability of a BOOL data.
        /// </remarks>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>The boolean value read.</returns>
        private static bool ReadBool(IWangStream stream)
        {
            return stream.ReadUint32() != 0;
        }

        /// <summary>
        /// The matrix for fast conversion from integer value to enum.
        /// </summary>
        private static readonly WangMarkType[] ConversionWangMarkTypes = new WangMarkType[16]
        {
            // 0 is invalid
            WangMarkType.Invalid,
            // 1 = Image embedded
            WangMarkType.ImageEmbedded,
            // 2 = Image reference
            WangMarkType.ImageReference,
            // 3 = Straight line
            WangMarkType.StraightLine,
            // 4 = Freehand line
            WangMarkType.FreehandLine,
            // 5 = Hollow rectangle,
            WangMarkType.HollowRectangle,
            // 6 = Filled rectangle
            WangMarkType.FilledRectangle,
            // 7 = Typed text
            WangMarkType.TypedText,
            // 8 = Text from file
            WangMarkType.TextFromFile,
            // 9 = Text stamp
            WangMarkType.TextStamp,
            // 10 = Attach-a-Note
            WangMarkType.AttachANote,
            // 11 is invalid
            WangMarkType.Invalid,
            // 12 = Form
            WangMarkType.Form,
            // 13 = OCR region
            WangMarkType.OcrRegion,
            // 14 = Hollow polygon
            WangMarkType.HollowPolygon,
            // 15 = Filled polygon
            WangMarkType.FilledPolygon
        };
    }
}
