using System;
using System.Drawing;
using System.IO;
using GdPicture14;
using GdPicture14.Annotations;
using WangTagReadingLibrary;
using FontStyle = System.Drawing.FontStyle;

namespace WangTiffToTiff
{
    /// <summary>
    /// The WangTiffToGdPictureTiffWangAnnotationHandler class implements the IWangAnnotationHandler interface for debug purpose.
    /// </summary>
    /// <remarks>
    /// This class converts the annotation information received through the interface into GdPicture.NET annotations for a TIFF.
    /// More than just using the GdPicture.NET API, it is also required to convert a few things:
    /// - coordinates (most of the time pixels to inches)
    /// - concept (for instance hollow rectangle does not apply as is in GdPicture.NET)
    /// </remarks>
    internal class WangTiffToGdPictureTiffWangAnnotationHandler : IWangAnnotationHandler
    {
        /// <summary>
        /// The constructor initializes members with the provided parameters.
        /// </summary>
        /// <param name="widthPixels">The width of the image (pixels).</param>
        /// <param name="heightPixels">The height of the image (pixels).</param>
        /// <param name="widthInches">The width of the image (inches).</param>
        /// <param name="heightInches">The height of the image (inches).</param>
        /// <param name="annotationManager">The annotation manager to udpdate.</param>
        /// <param name="textWriter">The stream to output the annotations as text (optional).</param>
        public WangTiffToGdPictureTiffWangAnnotationHandler(int widthPixels, int heightPixels, double widthInches, double heightInches, AnnotationManager annotationManager, StreamWriter textWriter)
        {
            _widthPixels = (double)widthPixels;
            _heightPixels = (double)heightPixels;
            _widthInches = widthInches;
            _heightInches = heightInches;
            _annotationManager = annotationManager;
            _textWriter = textWriter;
            LastError = GdPictureStatus.OK;
        }

        /// <summary>
        /// AddLineAnnot adds a line annotation to the annotation manager.
        /// </summary>
        /// <param name="srcXPixels">The X coordinate of the source (pixels).</param>
        /// <param name="srcYPixels">The Y coordinate of the source (pixels).</param>
        /// <param name="dstXPixels">The X coordinate of the target (pixels).</param>
        /// <param name="dstYPixels">The Y coordinate of the target (pixels).</param>
        /// <param name="colorComponents">The components for the color of the line.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        public void AddLineAnnot(int srcXPixels, int srcYPixels, int dstXPixels, int dstYPixels, byte[] colorComponents, int borderWidthPixels)
        {
            _textWriter?.WriteLine("AddLineAnnot");
            _textWriter?.WriteLine("srcYPixels: " + srcXPixels + "srcYPixels: " + srcYPixels + " dstXPixels: " + dstXPixels + " dstYPixels: " + dstYPixels);
            _textWriter?.WriteLine("colorComponents: " + colorComponents[0] + " " + colorComponents[1] + " " + colorComponents[2]);
            _textWriter?.WriteLine("borderWidthPixels: " + borderWidthPixels);

            AnnotationLine annotation = _annotationManager.AddLineAnnot(GdPictureColor(colorComponents), ToInchesHorizontal(srcXPixels), ToInchesVertical(srcYPixels), ToInchesHorizontal(dstXPixels), ToInchesVertical(dstYPixels));
            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotation.BorderWidth = ToInchesHorizontal(borderWidthPixels);
        }

        /// <summary>
        /// AddFreeHandHighligtherAnnot adds a free hand highligther annotation to the annotation manager.
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the serie of lines.</param>
        /// <param name="colorComponents">The components for the color.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        public void AddFreeHandHighligtherAnnot(int[] pointsCoordinates, byte[] colorComponents, int borderWidthPixels)
        {
            _textWriter?.WriteLine("AddFreeHandHighligtherAnnot");
            if (_textWriter != null)
            {
                for (int index = 0; index < pointsCoordinates.Length; index++)
                {
                    _textWriter.Write(pointsCoordinates[index] + " ");
                }
                _textWriter.WriteLine("");
            }
            _textWriter?.WriteLine("colorComponents: " + colorComponents[0] + " " + colorComponents[1] + " " + colorComponents[2]);
            _textWriter?.WriteLine("borderWidthPixels: " + borderWidthPixels);

            AnnotationFreeHandHighlighter annotation = _annotationManager.AddFreeHandHighlighterAnnot(GdPictureColor(colorComponents), ToInches(pointsCoordinates));
            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotation.BorderWidth = ToInchesHorizontal(borderWidthPixels);
        }


        /// <summary>
        /// AddTextAnnot adds a text annotation to the annotation manager. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="italic">The italic flag.</param>
        /// <param name="underline">The underline flag.</param>
        /// <param name="fontName">The font name.</param>
        /// <param name="rotation">The rotation of the annotation in the range [0;360] (degrees, clockwise). </param>
        public void AddTextAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, bool italic, bool underline, string fontName,
            int rotation)
        {
            _textWriter?.WriteLine("AddTextAnnot");
            _textWriter?.WriteLine("leftPixels: " + leftPixels + " topPixels: " + topPixels + " widthPixels: " + widthPixels + " heightPixels: " + heightPixels);
            _textWriter?.WriteLine("text: " + text);
            _textWriter?.WriteLine("italic: " + italic + " underline: " + underline + " fontName: " + fontName + " rotation: " + rotation);

            AnnotationText annotationText = _annotationManager.AddTextAnnot(ToInchesHorizontal(leftPixels),
                ToInchesVertical(topPixels), ToInchesHorizontal(widthPixels), ToInchesVertical(heightPixels), text);
            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotationText.Rotation = rotation;
            annotationText.FontName = fontName;
            annotationText.FontStyle = FontStyle.Regular;
            if (italic)
            {
                annotationText.FontStyle |= FontStyle.Italic;
            }
            if (underline)
            {
                annotationText.FontStyle |= FontStyle.Underline;
            }
        }

        /// <summary>
        /// AddTextAnnot adds a text annotation to the annotation manager. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="link">The link. </param>
        public void AddLinkAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text,
            string link)
        {
            AnnotationLink annotationLink = _annotationManager.AddLinkAnnot(ToInchesHorizontal(leftPixels),
                ToInchesVertical(topPixels), ToInchesHorizontal(widthPixels), ToInchesVertical(heightPixels), text, link);
            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }
        }

        /// <summary>
        /// AddStickyNoteAnnot adds a sticky note annotation to the annotation manager. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="foreColorComponents">The components for the fore color.</param>
        /// <param name="fillColorComponents">the components for the fill color.</param>
        public void AddStickyNoteAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text,
            byte[] foreColorComponents, byte[] fillColorComponents)
        {
            AnnotationStickyNote annotationStickyNote =
                _annotationManager.AddStickyNoteAnnot(ToInchesHorizontal(leftPixels), ToInchesVertical(topPixels),
                    ToInchesHorizontal(widthPixels), ToInchesVertical(heightPixels), text);

            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotationStickyNote.ForeColor = GdPictureColor(foreColorComponents);
            annotationStickyNote.FillColor = GdPictureColor(fillColorComponents);
        }

        /// <summary>
        /// AddRectangleAnnot adds a rectangle annotation to the annotation manager. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="borderColorComponents">The components for the color of the border.</param>
        /// <param name="backColorComponents">The components for the color of the background.</param>        
        /// <param name="fill">The flag indicating whether the rectangle should be filled (filled rectangle) or not (hollow rectangle).</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        public void AddRectangleAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels,
            byte[] borderColorComponents, byte[] backColorComponents, bool fill, int borderWidthPixels)
        {
            AnnotationRectangle annotationRectangle =
                _annotationManager.AddRectangleAnnot(GdPictureColor(borderColorComponents),
                    GdPictureColor(backColorComponents),
                    ToInchesHorizontal(leftPixels),
                    ToInchesVertical(topPixels),
                    ToInchesHorizontal(widthPixels), ToInchesVertical(heightPixels));

            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            if (!fill)
            {
                // We do consider the border width only for hollow rectangles.
                annotationRectangle.BorderWidth = ToInchesHorizontal(borderWidthPixels);
                annotationRectangle.Fill = false;
            }
        }

        /// <summary>
        /// AddPolygonAnnot adds a polygon annotation to the annotation manager. 
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the polygon.</param>
        /// <param name="borderColorComponents">The components for the color of the border.</param>
        /// <param name="backColorComponents">The components for the color of the background.</param>        
        /// <param name="fill">The flag indicating whether the polygon should be filled (filled polygon) or not (hollow polygon).</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        public void AddPolygonAnnot(int[] pointsCoordinates, byte[] borderColorComponents, byte[] backColorComponents,
            bool fill, int borderWidthPixels)
        {
            PointF[] pointsForGdPicture = new PointF[pointsCoordinates.Length / 2];
            for (int index = 0; index < pointsForGdPicture.Length; index++)
            {
                pointsForGdPicture[index].X = ToInchesHorizontal(pointsCoordinates[2 * index]);
                pointsForGdPicture[index].Y = ToInchesVertical(pointsCoordinates[2 * index + 1]);
            }

            AnnotationPolygon annotationPolygon =
                _annotationManager.AddPolygonAnnot(GdPictureColor(borderColorComponents),
                    GdPictureColor(backColorComponents), pointsForGdPicture);

            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            if (!fill)
            {
                // We do consider the border width only for hollow polygons.
                annotationPolygon.BorderWidth = ToInchesHorizontal(borderWidthPixels);
                annotationPolygon.Fill = false;
            }
        }

        /// <summary>
        /// AddFreeHandAnnot adds a free hand annotation to the annotation manager.
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the serie of lines.</param>
        /// <param name="colorComponents">The components for the color.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        public void AddFreeHandAnnot(int[] pointsCoordinates, byte[] colorComponents, int borderWidthPixels)
        {
            PointF[] pointsForGdPicture = new PointF[pointsCoordinates.Length / 2];
            for (int index = 0; index < pointsForGdPicture.Length; index++)
            {
                pointsForGdPicture[index].X = ToInchesHorizontal(pointsCoordinates[2 * index]);
                pointsForGdPicture[index].Y = ToInchesVertical(pointsCoordinates[2 * index + 1]);
            }

            AnnotationFreeHand annotationFreeHand = _annotationManager.AddFreeHandAnnot(
                GdPictureColor(colorComponents), pointsForGdPicture);

            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotationFreeHand.BorderWidth = ToInchesHorizontal(borderWidthPixels);
        }

        /// <summary>
        /// AddEmbeddedImageAnnot adds an embedded image annotation.
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="dib">Pointer to a Microsoft® Windows® Graphics Device Interface (GDI) BITMAPINFO structure, as IntPtr value.</param>
        public void AddEmbeddedImageAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, IntPtr dib)
        {
            GdPictureImaging imaging = new GdPictureImaging();
            int id = imaging.CreateGdPictureImageFromDIB(dib);

            if (id < 0)
            {
                LastError = imaging.GetStat();
                return;
            }

            AnnotationEmbeddedImage annotationEmbeddedImage = _annotationManager.AddEmbeddedImageAnnot(id,
                ToInchesHorizontal(leftPixels), ToInchesVertical(topPixels),
                ToInchesHorizontal(widthPixels), ToInchesVertical(heightPixels));

            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }
        }

        /// <summary>
        /// AddRubberStampAnnot adds a buffer stamp annotation to the annotation manager. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="rotation">The rotation of the annotation in the range [0;360] (degrees, clockwise). </param>
        /// <param name="colorComponents">The components for the color for the stamp.</param>
        public void AddRubberStampAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text,
            int rotation, byte[] colorComponents)
        {
            AnnotationRubberStamp annotationRubberStamp =
                _annotationManager.AddRubberStampAnnot(GdPictureColor(colorComponents),
                    ToInchesHorizontal(leftPixels), ToInchesVertical(topPixels), ToInchesHorizontal(widthPixels),
                    ToInchesVertical(heightPixels), text);
            GdPictureStatus status = _annotationManager.GetStat();
            if (status != GdPictureStatus.OK)
            {
                LastError = status;
            }

            annotationRubberStamp.Rotation = rotation;
        }

        /// <summary>
        /// ToInchesHorizontal converts an horizontal coordinate in points to an horizontal coordinate in inches.
        /// </summary>
        /// <param name="pixels">The coordinates in inches.</param>
        /// <returns>>The coordinates in  points.</returns>
        private float ToInchesHorizontal(int pixels)
        {
            return (float)(((double)pixels / _widthPixels) * _widthInches);
        }

        /// <summary>
        /// ToInchesVertical converts an vertical coordinate in points to an vertical coordinate in inches.
        /// </summary>
        /// <param name="pixels">The coordinates in inches.</param>
        /// <returns>>The coordinates in  points.</returns>
        private float ToInchesVertical(int pixels)
        {
            return (float)(((double)pixels / _heightPixels) * _heightInches);
        }

        /// <summary>
        /// ToInchesVertical converts points coordinates in points to points coordinates in inches.
        /// </summary>
        /// <param name="pointsCoordinates"></param>
        /// <returns></returns>
        private PointF[] ToInches(int[] pointsCoordinates)
        {
            PointF[] pointsForGdPicture = new PointF[pointsCoordinates.Length / 2];
            for (int index = 0; index < pointsForGdPicture.Length; index++)
            {
                pointsForGdPicture[index].X = ToInchesHorizontal(pointsCoordinates[2 * index]);
                pointsForGdPicture[index].Y = ToInchesVertical(pointsCoordinates[2 * index + 1]);
            }
            return pointsForGdPicture;
        }

        /// <summary>
        /// Creates a GdPicture color with the components stored in the array.
        /// </summary>
        /// <param name="colorComponents">The components for the color.</param>
        /// <returns>The color.</returns>
        private static Color GdPictureColor(byte[] colorComponents)
        {
            return Color.FromArgb(colorComponents[WangAnnotationTranslation.RedIndex],
                colorComponents[WangAnnotationTranslation.GreenIndex],
                colorComponents[WangAnnotationTranslation.BlueIndex]);
        }

        /// <summary>
        /// The width of the image (pixels).
        /// </summary>
        private readonly double _widthPixels;

        /// <summary>
        /// The height of the image (pixels).
        /// </summary>
        private readonly double _heightPixels;

        /// <summary>
        /// The width of the image (inches).
        /// </summary>
        private readonly double _widthInches;

        /// <summary>
        /// The height of the image (inches).
        /// </summary>
        private readonly double _heightInches;

        /// <summary>
        /// The annotation manager.
        /// </summary>
        private readonly AnnotationManager _annotationManager;

        /// <summary>
        /// The output for the annotations as text (optional).
        /// </summary>
        private readonly StreamWriter _textWriter;

        /// <summary>
        /// The last error.
        /// </summary>
        public GdPictureStatus LastError { get; set; }
    }
}
