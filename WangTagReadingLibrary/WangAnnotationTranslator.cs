using System;
#if DEBUG
using System.Diagnostics;
#endif // DEBUG
using System.Runtime.InteropServices;

namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangAnnotationTranslator class implements a couple of methods to translate a Wang annotation to something
    /// close to the GdPicture.NET world.
    /// </summary>
    internal static class WangAnnotationTranslator
    {
        /// <summary>
        /// Send sends the annotation with the current properties to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        public static bool Send(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            // TODO - David Ometto - 2016-11-23 - Study the best way to deal with the Visible property for each and every mark type.
            // Preconditions
#if DEBUG
            Debug.Assert(properties.HasMarkAttributes);
#endif //DEBUG
            switch (properties.MarkAttributes.Type)
            {
                case WangMarkType.StraightLine:
                    return SendStraightLine(handler, properties);

                case WangMarkType.TypedText:
                    return SendTypedText(handler, properties);
                    
                case WangMarkType.AttachANote:
                    return SendAttachANote(handler, properties);
                    
                case WangMarkType.FilledRectangle:
                    return SendFilledRectangle(handler, properties);
                    
                case WangMarkType.HollowRectangle:
                    return SendHollowRectangle(handler, properties);
                    
                case WangMarkType.FilledPolygon:
                    return SendFilledPolygon(handler, properties);
                    
                case WangMarkType.HollowPolygon:
                    return SendHollowPolygon(handler, properties);
                    
                case WangMarkType.FreehandLine:
                    return SendFreehandLine(handler, properties);
                    
                case WangMarkType.ImageEmbedded:
                    return SendImageEmbeded(handler, properties);
                    
                case WangMarkType.TextStamp:
                    return SendTextStamp(handler, properties);
                    
                case WangMarkType.TextFromFile:
                    return SendTextFromFile(handler, properties);

                case WangMarkType.OcrRegion:
                    return SendOcrRegion(handler, properties);
                    
                case WangMarkType.ImageReference:
                case WangMarkType.Form:                
                case WangMarkType.Invalid:
                default:
                    // TODO - David Ometto - 2016-11-22 - We have to develop the support for this annotation type
                    return false;
                    break;
            }            
        }

        /// <summary>
        /// SendStraightLine sends a straight line annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendStraightLine(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Straight Line
             * Definition: A line with a defined starting and ending point.
             * Attributes: rgbColor1, bHighlighting, uLineSize
             * Named Blocks and Associated Structures:
             * - OiAnoDat       AN_POINTS
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            if (properties.HasPoints && WangAnnotationTranslation.PointsLength(properties.Points) == 2)
            {
                SendStraightLine(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds, properties.Points, properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting, properties.MarkAttributes.LineSize);
                return true;
            }

            return false;            
        }

        /// <summary>
        /// SendStraightLine sends a straight line annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        /// <param name="bounds">The rectangle surrouding the annotation.</param>
        /// <param name="points">The points for the line.</param>
        /// <param name="colorComponents">The components for the color of the line.</param>
        /// <param name="highlighting">A flag indicating whether it's an highlighter line or a regular line.</param>
        /// <param name="lineSize">The size of the line, i.e. width of the line.</param>
        private static void SendStraightLine( IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int[] points, byte[] colorComponents, bool highlighting, uint lineSize)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.

            if (highlighting)
            {
                // The coordinates for the line are relative to the upper left corner of the annotation
                // bounding box. Also convert a straight line to a free hand because of highlither
                
                handler.AddFreeHandHighligtherAnnot(PointsInImage(bounds, points), colorComponents, (int) lineSize );
            }
            else
            {
                // The coordinates for the line are relative to the upper left corner of the annotation
                // bounding box.

                handler.AddLineAnnot(
                    bounds[WangAnnotationTranslation.LeftIndex] + WangAnnotationTranslation.PointX(0, points),
                    WangAnnotationTranslation.PointY(0, points),
                    WangAnnotationTranslation.PointX(1, points),
                    WangAnnotationTranslation.PointY(1, points), colorComponents, (int)lineSize);
            }
        }

        /// <summary>
        /// SendTypedText sends a typed text annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendTypedText(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Typed Text
             * Definition: A series of characters entered from the keyboard.
             * Attributes: rgbColor1, lfFont
             * Named Blocks and Associated Structures:
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       OIAN_TEXTPRIVDATA
             */
            if (properties.HasDisplayText)
            {
                if (properties.HasHyperlink)
                {
                    SendLink(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                        properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                        properties.DisplayText.Text,
                        properties.Hyperlink.InternalLink, properties.Hyperlink.Link,
                        properties.Hyperlink.WorkingDirectory, properties.Hyperlink.Location,
                        properties.MarkAttributes.Color1, properties.MarkAttributes.LogFont);
                }
                else
                {
                    SendTypedText(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                        properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                        properties.DisplayText.Text,
                        properties.MarkAttributes.Color1, properties.MarkAttributes.LogFont);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendTypedText sends a typed text annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        /// <param name="bounds">The rectangle surrouding the annotation.</param>
        private static void SendTypedText( IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int orientation, uint creationScale, string text, byte[] color, WangLogFont lfFont)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            // Take extra care of each and every field of the WangLogFont and WangDisplayText structures.

            handler.AddTextAnnot(bounds[WangAnnotationTranslation.LeftIndex], bounds[WangAnnotationTranslation.TopIndex], bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex], bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                lfFont.Italic, lfFont.Underline, lfFont.FaceName, orientation/10);
        }

        /// <summary>
        /// SendLink sends a typed link annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        /// <param name="bounds">The rectangle surrouding the annotation.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendLink(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int orientation,
            uint creationScale, string text,
            bool internalLink, string link, string workingDirectory, string location,
            byte[] color,
            WangLogFont lfFont)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            // Take extra care of each and every field of the WangLogFont and WangDisplayText structures.

            // TODO - David Ometto - 2016-11-22 - This definitely does not work. Having a try with a pdf
            // output: the text is there, underlined and blue ... but not link. Tested with www.orpalis.com.
            // Also the link to files, etc. needs to be analysed to understand how to combine the different
            // components.
            if (internalLink)
            {
                handler.AddLinkAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.TopIndex],
                    bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                    workingDirectory + link + location);
            }
            else
            {
                handler.AddLinkAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.TopIndex],
                    bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                    link);
            }
            return true;
        }

        /// <summary>
        /// SendAttachANote sends an "attach a note" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendAttachANote(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Attach-a-Note
             * Definition: A colored rectangle that contains text.
             * Attributes: rgbColor1, rgbColor2, lfFont
             * Named Blocks and Associated Structures:
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       OIAN_TEXTPRIVDATA
             */
            if (properties.HasDisplayText)
            {
                SendAttachANote(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                    properties.DisplayText.Text,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.Color2,
                    properties.MarkAttributes.LogFont);
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendAttachANote sends an "attach a note" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendAttachANote(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int orientation,
            uint creationScale, string text, byte[] mainColor, byte[] colorOfText,
            WangLogFont lfFont)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            // Take extra care of each and every field of the WangLogFont and WangDisplayText structures.

            handler.AddStickyNoteAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.TopIndex],
                bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                colorOfText,
                mainColor);
        }

        /// <summary>
        /// SendFilledRectangle sends a "Filled rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendFilledRectangle(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Filled Rectangle
             * Definition: A rectangle with a filled center.
             * Attributes: rgbColor1, bHighlighting
             * Named Blocks and Associated Structures
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            SendFilledRectangle(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting);
            return true;
        }

        /// <summary>
        /// SendFilledRectangle sends a "Filled rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendFilledRectangle(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, byte[] color,
            bool highlighting)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            handler.AddRectangleAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.TopIndex],
                bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], color, color,
                true, 0);
        }

        /// <summary>
        /// SendHollowRectangle sends a "Hollow rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendHollowRectangle(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Hollow Rectangle
             * Definition: A rectangle with a center that is not filled.
             * Attributes: rgbColor1, bHighlighting, uLineSize
             * Named Blocks and Associated Structures
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            SendHollowRectangle(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting,
                properties.MarkAttributes.LineSize);
            return true;
        }

        /// <summary>
        /// SendFilledRectangle sends a "Filled rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendHollowRectangle(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, byte[] color,
            bool highlighting, uint lineSize)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            handler.AddRectangleAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.TopIndex],
                bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], color, color,
                false,
                (int) lineSize);
        }

        /// <summary>
        /// SendFilledPolygon sends a "Filled polygon" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendFilledPolygon(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Filled Polygon
             * Please note this type was not mentionned within the documentation. The implementation
             * of this type is kind of reverse engineering. It's a mix between rectangles and free hand lines.
             * 
             * Definition: A polygon with a filled center.
             * Attributes: rgbColor1, bHighlighting
             * Named Blocks and Associated Structures
             * - OiAnoDat       AN_POINTS
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            if (properties.HasPoints)
            {
                SendFilledPolygon(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.Points,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting);
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendFilledPolygon sends a "Filled polygon" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendFilledPolygon(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int[] points, byte[] color,
            bool highlighting)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            handler.AddPolygonAnnot(PointsInImage(bounds, points), color, color, true, 0);
        }

        /// <summary>
        /// SendHollowPolygon sends a "Hollow rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendHollowPolygon(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Hollow Polygon
             * Please note this type was not mentionned within the documentation. The implementation
             * of this type is kind of reverse engineering. It's a mix between rectangles and free hand lines.
             * 
             * Definition: A polygon with a center that is not filled.
             * Attributes: rgbColor1, bHighlighting, uLineSize
             * Named Blocks and Associated Structures
             * - OiAnoDat       AN_POINTS
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            SendHollowPolygon(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                properties.Points,
                properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting,
                properties.MarkAttributes.LineSize);
            return true;
        }

        /// <summary>
        /// SendFilledPolygon sends a "Filled rectangle" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendHollowPolygon(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int[] points, byte[] color,
            bool highlighting, uint lineSize)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            handler.AddPolygonAnnot(PointsInImage(bounds, points), color, color, false, (int) lineSize);
        }


        /// <summary>
        /// SendFreehandLine sends a free hand line annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendFreehandLine(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Freehand Line
             * Definition: A series of lines such that the starting point of line n+1 is the same as the ending point of line n.
             * Attributes: rgbColor1, bHighlighting, uLineSize
             * Named Blocks and Associated Structures:
             * - OiAnoDat       AN_POINTS
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            if (properties.HasPoints)
            {
                SendFreehandLine(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.Points,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.Highlighting,
                    properties.MarkAttributes.LineSize);
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendFreehandLine sends a free hand line annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendFreehandLine(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int[] points,
            byte[] color,
            bool highlighting, uint lineSize)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            handler.AddFreeHandAnnot(PointsInImage(bounds, points), color, (int) lineSize);
        }

        /// <summary>
        /// SendImageEmbeded sends an "Image Embedded" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendImageEmbeded(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Image Embedded
             * Definition: An embedded image.
             * Attributes: bTransparent
             * Named Blocks and Associated Stuctures
             * - OiAnoDat       AN_NEW_ROTATE_STRUCT
             * - OiFilNam       AN_NAME_STRUCT
             * - OiDIB          AN_IMAGE_STRUCT
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       Not used
             */

            if (properties.HasDib && properties.HasFilename && properties.HasDib)
            {
                SendImageEmbeded(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.MarkAttributes.Transparent, properties.Rotation, properties.Filename, properties.DibInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The AutoPinner class handles unmanaged resources.
        /// </summary>
        private class AutoPinner : IDisposable
        {
            /// <summary>
            /// The constructor initializes the unmanaged memory with a copy of the provided object.
            /// </summary>
            /// <param name="obj">The object to copy.</param>
            public AutoPinner(Object obj)
            {
                _pinned = GCHandle.Alloc(obj, GCHandleType.Pinned);
            }

            /// <summary>
            /// The operator retrieves a pointer to the unmanaged memory.
            /// </summary>
            /// <param name="autoPinner">The object holding the memory.</param>
            public static implicit operator IntPtr(AutoPinner autoPinner)
            {
                return autoPinner._pinned.AddrOfPinnedObject();
            }

            /// <summary>
            /// Dispose releases the memory.
            /// </summary>
            public void Dispose()
            {
                _pinned.Free();
            }

            /// <summary>
            /// The handle.
            /// </summary>
            GCHandle _pinned;
        }

        /// <summary>
        /// SendImageEmbeded sends an "Image Embedded" annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendImageEmbeded(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, bool transparent,
            WangRotation rotation, string filename, byte[] dib)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.

            using (AutoPinner autoPinner = new AutoPinner(dib))
            {
                handler.AddEmbeddedImageAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.TopIndex],
                    bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                    bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex],
                    autoPinner);
            }
        }

        /// <summary>
        /// SendTextStamp sends a text stamp annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendTextStamp(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Text Stamp
             * Definition: Text that contains a pre-defined string which may include, for example, the date and/or time when the mark was applied.
             * Attributes: rgbColor1, lfFont
             * Named Blocks and Associated Structures:
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       OIAN_TEXTPRIVDATA
             * 
             */
            if (properties.HasDisplayText)
            {
                // TODO - David Ometto - 2016-11-24 - We miss the time_t read for the moment
                SendTextStamp(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                    properties.DisplayText.Text,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.LogFont);
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendTextStamp sends a text stamp annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendTextStamp(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int orientation,
            uint creationScale, string text, byte[] color,
            WangLogFont lfFont)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            // Take extra care of each and every field of the WangLogFont and WangDisplayText structures.

            handler.AddRubberStampAnnot(bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.TopIndex],
                bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                orientation/10,
                color);
        }

        /// <summary>
        /// SendTextFromFile sends a text from file annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendTextFromFile(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * Text From File
             * Definition: Text supplied from a file. The file format is Text Only, which is only ASCII with carriage returns, line feeds, and tabs.
             * Attributes: rgbColor1, lfFont
             * Named Blocks and Associated Structures:
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       OIAN_TEXTPRIVDATA
             */
            if (properties.HasDisplayText)
            {
                // TODO - David Ometto - 2016-11-24 - We miss the time_t read for the moment
                SendTextFromFile(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                    properties.DisplayText.Text,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.LogFont);
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendTextFromFile sends a text from file annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="oiGroup">The group.</param>
        /// <param name="oiIndex">The index.</param>
        private static void SendTextFromFile(
            IWangAnnotationHandler handler, string oiGroup, string oiIndex, int[] bounds, int orientation,
            uint creationScale, string text, byte[] color,
            WangLogFont lfFont)
        {
            // TODO - David Ometto - 2016-11-22 - Translate to GdPicture.NET proper annotation type.
            // Please see in detail the parameters that are just ignored for the moment.
            // Take extra care of each and every field of the WangLogFont and WangDisplayText structures.

            handler.AddTextAnnot(bounds[WangAnnotationTranslation.LeftIndex], bounds[WangAnnotationTranslation.TopIndex],
                bounds[WangAnnotationTranslation.RightIndex] - bounds[WangAnnotationTranslation.LeftIndex],
                bounds[WangAnnotationTranslation.BottomIndex] - bounds[WangAnnotationTranslation.TopIndex], text,
                lfFont.Italic, lfFont.Underline, lfFont.FaceName,
                orientation / 10);            
        }

        /// <summary>
        /// SendOcrRegion sends an ocr region annotation to the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>true if the operation succeeded otherwise returns false.</returns>
        private static bool SendOcrRegion(IWangAnnotationHandler handler, WangAnnotationProperties properties)
        {
            /*
             * OCR region
             * Definition: A rectangular region in which to perform OCR.
             * Attributes: rgbColor1, lfFont
             * Named Blocks and Associated Structures:
             * - OiAnoDat       Not used
             * - OiFilNam       Not used
             * - OiDIB          Not used
             * - OiGroup        STR
             * - OiIndex        STR
             * - OiAnText       OIAN_TEXTPRIVDATA (This will contain a sequential number string).
             */
            if (properties.HasDisplayText)
            {
                // TODO - David Ometto - 2016-11-24 - We miss the time_t read for the moment
                SendTextFromFile(handler, properties.OiGroup, properties.OiIndex, properties.MarkAttributes.Bounds,
                    properties.DisplayText.Orientation, properties.DisplayText.CreationScale,
                    properties.DisplayText.Text,
                    properties.MarkAttributes.Color1, properties.MarkAttributes.LogFont);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Several coordinates of the points are relative to upper left corner of the bounds. 
        /// This method translates to upper left corner of the image
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="points">The points.</param>
        /// <returns>The points in the image.</returns>
        private static int[] PointsInImage(int[] bounds, int[] points)
        {
            int[] pointsInImage = new int[points.Length];
            Array.Copy(points, pointsInImage, points.Length);
            for (int index = 0; index < points.Length; index += 2)
            {
                pointsInImage[index] += bounds[WangAnnotationTranslation.LeftIndex];
            }
            for (int index = 1; index < points.Length; index += 2)
            {
                pointsInImage[index] += bounds[WangAnnotationTranslation.TopIndex];
            }
            return pointsInImage;
        }
    }
}
