using System;
using WangTagReadingLibrary;

namespace WangTagReadingLibraryTests
{
    /// <summary>
    /// The WangAnnotationHandlerTest class implements the IWangAnnotationHandler interface for test purpose.
    /// </summary>
    class WangAnnotationHandlerTest : IWangAnnotationHandler
    {
        public void AddEmbeddedImageAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, IntPtr dib)
        {
            throw new NotImplementedException();
        }

        public void AddFreeHandAnnot(int[] pointsCoordinates, byte[] color, int borderWidthPixels)
        {
            throw new NotImplementedException();
        }

        public void AddFreeHandHighligtherAnnot(int[] pointsCoordinates, byte[] colorComponents, int borderWidthPixels)
        {
            throw new NotImplementedException();
        }

        public void AddLineAnnot(byte[] colorComponents, int srcXPixels, int srcYPixels, int dstXPixels, int dstYPixels)
        {
            throw new NotImplementedException();
        }

        public void AddLineAnnot(int srcXPixels, int srcYPixels, int dstXPixels, int dstYPixels, byte[] colorComponents, int borderWidthPixels)
        {
            throw new NotImplementedException();
        }

        public void AddLinkAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, string link)
        {
            throw new NotImplementedException();
        }

        public void AddPolygonAnnot(int[] pointsCoordinates, byte[] borderColor, byte[] backColor, bool fill, int borderWidthPixels)
        {
            throw new NotImplementedException();
        }

        public void AddRectangleAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, byte[] borderColor, byte[] backColor, bool fill, int borderWidthPixels)
        {
            throw new NotImplementedException();
        }

        public void AddRubberStampAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, int rotation, byte[] color)
        {
            throw new NotImplementedException();
        }

        public void AddStickyNoteAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, byte[] foreColor, byte[] fillColor)
        {
            throw new NotImplementedException();
        }

        public void AddTextAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, int rotation)
        {
            throw new NotImplementedException();
        }

        public void AddTextAnnot(int leftPixels, int topPixels, int widthPixels, int heightPixels, string text, bool italic, bool underline, string fontName, int rotation)
        {
            throw new NotImplementedException();
        }
    }
}
