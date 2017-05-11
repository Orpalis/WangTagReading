using System;

namespace WangTagReadingLibrary
{
    /// <summary>
    /// The IWangAnnotationHandler describes an interface for a class handling Wang annotations.
    /// </summary>
    internal interface IWangAnnotationHandler
    {
        /// <summary>
        /// AddLineAnnot is called by the Wang annotation reader when it is required to add a line annotation. 
        /// </summary>        
        /// <param name="srcXPixels">The X coordinate of the source (pixels).</param>
        /// <param name="srcYPixels">The Y coordinate of the source (pixels).</param>
        /// <param name="dstXPixels">The X coordinate of the target (pixels).</param>
        /// <param name="dstYPixels">The Y coordinate of the target (pixels).</param>
        /// <param name="colorComponents">The components for the color of the line.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        void AddLineAnnot(int srcXPixels, int srcYPixels, int dstXPixels, int dstYPixels, byte[] colorComponents, int borderWidthPixels);

        /// <summary>
        /// AddFreeHandHighligtherAnnot is called by the Wang annotation reader when it is required to add a free hand highligther annotation.
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the serie of lines.</param>
        /// <param name="colorComponents">The components for the color of the line.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        void AddFreeHandHighligtherAnnot( int[] pointsCoordinates, byte[] colorComponents, int borderWidthPixels);

        /// <summary>
        /// AddTextAnnot is called by the Wang annotation reader when it is required to add a text annotation. 
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
        void AddTextAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, bool italic, bool underline, string fontName, int rotation);

        /// <summary>
        /// AddLinkAnnot is called by the Wang annotation reader when it is required to add a link annotation. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="link">The link. </param>
        void AddLinkAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, string link);

        /// <summary>
        /// AddStickyNoteAnnot is called by the Wang annotation reader when it is required to add a sticky note annotation. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="foreColor">The fore color.</param>
        /// <param name="fillColor">The fill color.</param>
        void AddStickyNoteAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text,
            byte[] foreColor, byte[] fillColor);

        /// <summary>
        /// AddRectangleAnnot is called by the Wang annotation reader when it is required to add a rectangle note annotation. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="borderColor">The color of the border.</param>
        /// <param name="backColor">The color of the background.</param>
        /// <param name="fill">The flag indicating whether the rectangle should be filled (filled rectangle) or not (hollow rectangle).</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        void AddRectangleAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, byte[] borderColor,
            byte[] backColor, bool fill, int borderWidthPixels);

        /// <summary>
        /// AddPolygonAnnot is called by the Wang annotation reader when it is required to add a polygon note annotation. 
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the serie of lines.</param>
        /// <param name="borderColor">The color of the border.</param>
        /// <param name="backColor">The color of the background.</param>
        /// <param name="fill">The flag indicating whether the rectangle should be filled (filled rectangle) or not (hollow rectangle).</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        void AddPolygonAnnot(int[] pointsCoordinates, byte[] borderColor, byte[] backColor, bool fill, int borderWidthPixels);

        /// <summary>
        /// AddFreeHandAnnot is called by the Wang annotation reader when it is required to add a free hand annotation.
        /// </summary>
        /// <param name="pointsCoordinates">The coordinates for the points that are defining the serie of lines.</param>
        /// <param name="colorComponents">The color.</param>
        /// <param name="borderWidthPixels">The width of the border (pixels).</param>
        void AddFreeHandAnnot(int[] pointsCoordinates, byte[] colorComponents, int borderWidthPixels);

        /// <summary>
        /// AddEmbeddedImageAnnot is called by the Wang annotation reader when it is required to add an embedded image annotation.
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="dib">Pointer to a Microsoft® Windows® Graphics Device Interface (GDI) BITMAPINFO structure, as IntPtr value.</param>
        void AddEmbeddedImageAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, IntPtr dib);

        /// <summary>
        /// AddRubberStampAnnot is called by the Wang annotation reader when it is required to add a rubber stamp annotation. 
        /// </summary>
        /// <param name="leftPixels">The left coordinate of the annotation (pixels).</param>
        /// <param name="topPixels">The top coordinate of the annotation (pixels).</param>
        /// <param name="widthPixels">The width of the annotation (pixels).</param>
        /// <param name="heightPixels">The height of the annotation (pixels).</param>
        /// <param name="text">The text.</param>
        /// <param name="rotation">The rotation of the annotation in the range [0;360] (degrees, clockwise). </param>
        /// <param name="colorComponents">The color for the stamp.</param>
        void AddRubberStampAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text,
            int rotation, byte[] colorComponents);
    }
}
