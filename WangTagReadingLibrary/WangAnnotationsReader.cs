using System;
#if DEBUG
using System.Diagnostics;
#endif // DEBUG

namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangAnnotationsReader class offers Wang annotations reading.
    /// </summary>
    internal sealed class WangAnnotationsReader
    {
        /// <summary>
        /// The WangDataType defines the different types for the block of data used to encode the Wang annotations
        /// </summary>
        /// <remarks>
        /// The data associated with an annotation tag in a TIFF file consists of zero or more blocks of default data, followed by the data necessary to describe each annotation mark. 
        /// Default data consists of default named blocks, designated Type 2 data.
        /// Annotation mark data consists of an attribute structure, designated Type 5 data, followed by one or more named blocks, designated Type 6 data.
        /// </remarks>
        public enum WangDataType
        {
            // Type 2 data
            DefaultNamedBlock,
            // Type 5 data
            Attributes,
            // Type 6
            NamedBlock,
            // Invalid
            Invalid
        }

        /// <summary>
        /// Read reads the Wang annotations within the provided tag data and calls the handler accordingly.
        /// </summary>        
        /// <param name="handler">The Wang annotation handler.</param>
        /// <param name="tagData">The tag data.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        public static bool Read(IWangAnnotationHandler handler, byte[] tagData)
        {
            WangStream wangStream = new WangStream(tagData);
            if (!WangAnnotationsReader.ReadHeader(wangStream))
            {
                return false;
            }
            WangAnnotationProperties propertiesDefault = new WangAnnotationProperties();
            WangAnnotationProperties propertiesCurrent = new WangAnnotationProperties();

            while (!wangStream.IsEnd())
            {
                WangDataType dataType;
                Int32 dataSize;
                bool blockRead = false;
                if (ReadDataType(out dataType, out dataSize, wangStream) && wangStream.AvailableBytes() >= dataSize)
                {
                    switch (dataType)
                    {
                        case WangDataType.DefaultNamedBlock:
                            // Data in this block will be part of each newly-created mark.
                            blockRead = OnUpdateDefaultProperties(propertiesDefault, wangStream, dataSize);
                            break;

                        case WangDataType.Attributes:
                            // The attribute structure of the next annotation mark.
                            // This type also implies the end of the previous mark’s data, erefore the beginning of an annotation mark.
                            blockRead = OnNewMark(handler, propertiesCurrent, wangStream, dataSize, propertiesDefault);
                            break;

                        case WangDataType.NamedBlock:
                            // A named block that is part of the preceding annotation mark.
                            blockRead = OnUpdateProperties(propertiesCurrent, wangStream, dataSize);
                            break;
                    }
                }

                if (!blockRead)
                {
                    return false;
                }
            }

            return OnDone(handler, propertiesCurrent);
        }

        /// <summary>
        /// ReadHeader reads the header for the tag holding the Wang annotations.
        /// </summary>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        public static bool ReadHeader(IWangStream stream)
        {
            if (stream.AvailableBytes() >= 8)
            {
                /*
                 * The annotation data itself consists of a 4-byte header, which is not currently used (reserved for future use). 
                 * The header should be skipped on read and written as zeros. Note, however, that it is included in the TIFF tag's Data Count field.
                 *  The header is followed by four bytes in Intel order that specify size of INTs and UINTs and byte ordering:
                 *  4 bytes:
                 *    0 = Intel 16-bit (least-significant byte to most-significant byte and two-byte INTs and UINTs)
                 *    1 = Intel 32-bit (least-significant byte to most-significant byte and four-byte INTs and UINTs)
                 */
                stream.SkipBytes(4);
                if (stream.ReadInt32() == 0)
                {
                    // 0 = Intel 16-bit (least-significant byte to most-significant byte and two-byte INTs and UINTs)
                    // TODO - David Ometto - 2016-11-21 - Add support for 16-bit annotations.
                    // TODO - David Ometto - 2016-11-21 - Find a good way to send the error.
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// ReadDataType reads the datatype.
        /// </summary>
        /// <param name="dataType">To retrieve the data type.</param>
        /// <param name="dataSize">To retrieve the data size.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        public static bool ReadDataType(out WangDataType dataType, out int dataSize, IWangStream stream)
        {
            if (stream.AvailableBytes() >= 8)
            {
                int intDataType = stream.ReadInt32();
                dataSize = stream.ReadInt32();
                dataType = intDataType < ConversionWangDataTypes.Length
                    ? ConversionWangDataTypes[intDataType]
                    : WangDataType.Invalid;
                return true;
            }
            dataSize = 0;
            dataType = WangDataType.Invalid;
            return false;
        }

        /// <summary>
        /// ReadBlock reads the block and updates the properties accordingly.
        /// </summary>
        /// <param name="properties">The properties to update.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the block.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        public static bool ReadBlock(WangAnnotationProperties properties, IWangStream stream, int dataSize)
        {
#if DEBUG
            Debug.Assert(stream.AvailableBytes() >= dataSize);
#endif // DEBUG
            WangNamedBlockHeader header = WangAnnotationStructureReader.ReadNamedBlockHeader(stream, dataSize);
            if (header == null || header.Size > stream.AvailableBytes())
            {
                return false;
            }

            /*
             * Named Block          Associated Structure            Usage
             * OiAnoDat             For lines: AN_POINTS            List coordinates for lines and freehand marks.
             *                      For images:AN_NEW_ROTATE_STRUCT Hold scaling and resolution information for image marks.
             * OiFilNam             AN_NAME_STRUCT                  Hold file name for image marks.
             * OiDIB                AN_IMAGE_STRUCT                 Store DIB data.
             * OiGroup (required)   STR                             Create sets of marks (required).
             * OiIndex (required)   STR                             Assign unique number, originating at 0, for each mark. 
             *                                                      To facilitate easy application control, the next available number is generated by incrementing by 1 the number just assigned and storing it in the default OiIndex mark.
             * OiAnText             OIAN_TEXTPRIVDATA               Display text annotation marks.
             * OiHypLnk             HYPERLINK_NB                    Turn mark into a hyperlink.
             */
            if (header.Name == "OiAnoDat")
            {
                // TODO - David Ometto - 2016-11-24 - Unit test this method
                return ReadAnoDatBlock(properties, stream, header.Size);
            }
            else if (header.Name == "OiFilNam")
            {
                properties.SetFilename(WangAnnotationStructureReader.ReadCharString(stream, header.Size));
                return true;
            }
            else if (header.Name == "OiDIB")
            {
                properties.SetDibInfo(WangAnnotationStructureReader.ReadDib(stream, header.Size));
                return true;
            }
            else if (header.Name == "OiGroup")
            {
                properties.OiGroup = WangAnnotationStructureReader.ReadCharString(stream, header.Size);
                return true;
            }
            else if (header.Name == "OiIndex")
            {
                properties.OiIndex = WangAnnotationStructureReader.ReadCharString(stream, header.Size);
                return true;
            }
            else if (header.Name == "OiAnText")
            {
                WangDisplayText displayText = WangAnnotationStructureReader.ReadDisplayText(stream, header.Size);
                if (displayText == null)
                {
                    return false;
                }
                properties.SetDisplayText(displayText);
                return true;
            }
            else if (header.Name == "OiHypLnk")
            {
                WangHyperlink hyperlink = WangAnnotationStructureReader.ReadHyperlink(stream, header.Size);
                if (hyperlink == null)
                {
                    return false;
                }
                properties.SetHyperlink(hyperlink);
                return true;
            }
            else
            {
                // We just skip unknown data
                stream.SkipBytes(header.Size);
                return true;
            }
        }

        /// <summary>
        /// OnUpdateDefaultProperties updates the default properties with the upcoming block.
        /// </summary>
        /// <param name="properties">The properties to update.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the block.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool OnUpdateDefaultProperties(WangAnnotationProperties properties, IWangStream stream,
            int dataSize)
        {
            return ReadBlock(properties, stream, dataSize);
        }

        /// <summary>
        /// OnNewMark has to be called when a new mark starts. 
        /// </summary>
        /// <remarks>
        /// The previous mark is sent to the handler and the properties are initialized with default properties and the mark attributes.
        /// </remarks>
        /// <param name="handler">The Wang annotation handler.</param>
        /// <param name="propertiesCurrent">The current properties.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the block.</param>
        /// <param name="propertiesDefault">The default properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool OnNewMark(IWangAnnotationHandler handler, WangAnnotationProperties propertiesCurrent,
            IWangStream stream, int dataSize, WangAnnotationProperties propertiesDefault)
        {
            if (propertiesCurrent.HasMarkAttributes && !WangAnnotationTranslator.Send(handler, propertiesCurrent))
            {
                return false;
            }

            propertiesCurrent.CopyFrom(propertiesDefault);

            WangMarkAttributes markAttributes = WangAnnotationStructureReader.ReadMarkAttributes(stream, dataSize);
            if (markAttributes == null)
            {
                return false;
            }
            propertiesCurrent.SetMarkAttributes(markAttributes);
            return true;
        }

        /// <summary>
        /// OnUpdateProperties updates the properties with the upcoming block.
        /// </summary>
        /// <param name="properties">The properties to update.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the block.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool OnUpdateProperties(WangAnnotationProperties properties, IWangStream stream, int dataSize)
        {
            return ReadBlock(properties, stream, dataSize);
        }

        /// <summary>
        /// OnDone has to be called when the whole data have been read.
        /// </summary>
        /// <remarks>
        /// The previous mark is sent to the handler if necessary.
        /// </remarks>
        /// <param name="handler">The Wang annotation handler.</param>
        /// <param name="propertiesCurrent">The current properties.</param>
        private static bool OnDone(IWangAnnotationHandler handler, WangAnnotationProperties propertiesCurrent)
        {
            if (propertiesCurrent.HasMarkAttributes)
            {
                return WangAnnotationTranslator.Send(handler, propertiesCurrent);
            }
            return true;
        }


        /// <summary>
        /// ReadAnoDatBlock reads an "OiAnoDat" block.
        /// </summary>
        /// <param name="properties">The properties to be updated with the data.</param>
        /// <param name="stream">The stream to read the data in.</param>
        /// <param name="dataSize">The size of the data</param>
        private static bool ReadAnoDatBlock(WangAnnotationProperties properties, IWangStream stream, int dataSize)
        {
#if DEBUG
            Debug.Assert(stream.AvailableBytes() >= dataSize);
#endif // DEBUG
            if (properties.HasMarkAttributes)
            {
                switch (properties.MarkAttributes.Type)
                {
                    case WangMarkType.StraightLine:
                    case WangMarkType.FreehandLine:
                    case WangMarkType.HollowPolygon:
                    case WangMarkType.FilledPolygon:
                        int [] points = WangAnnotationStructureReader.ReadPoints(stream, dataSize);
                        properties.SetPoints(points);
                        return true;

                    case WangMarkType.ImageEmbedded:
                        WangRotation rotation = WangAnnotationStructureReader.ReadRotation(stream, dataSize);
                        properties.SetRotation(rotation);
                        return true;

                    default:
                        // TODO - David Ometto - 2016-11-22 - Add support for all types                    
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// The matrix for fast conversion from integer value to enum.
        /// </summary>
        private static readonly WangDataType[] ConversionWangDataTypes = new WangDataType[7]
        {
            WangDataType.Invalid, WangDataType.Invalid, WangDataType.DefaultNamedBlock, WangDataType.Invalid,
            WangDataType.Invalid, WangDataType.Attributes, WangDataType.NamedBlock
        };
    }
}