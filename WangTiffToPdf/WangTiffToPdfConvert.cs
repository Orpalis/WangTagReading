using System.IO;
using GdPicture14;

namespace WangTiffToPdf
{
    /// <summary>
    /// The WangTiffToPdfConvert class offers conversion from TIFF to PDF including conversion of the annotations.
    /// </summary>
    internal static class WangTiffToPdfConvert
    {
        /// <summary>
        /// The PageInfo structure holds a couple of information required to convert the coordinates of the Wang annotation.
        /// </summary>
        struct PageInfo
        {
            /// <summary>
            /// The width of the image (pixels).
            /// </summary>
            public int WidthPixels;

            /// <summary>
            /// The height of the image (pixels).
            /// </summary>
            public int HeightPixels;

            /// <summary>
            /// The width of the image (inches).
            /// </summary>            
            public double WidthInches;

            /// <summary>
            /// The height of the image (inches).
            /// </summary>
            public double HeightInches;

            /// <summary>
            /// The data available within the Wang tag.
            /// </summary>
            public byte[] WangTagData;
        }

        /// <summary>
        /// Convert the provided TIFF to a PDF, including conversion of the annotations.
        /// </summary>
        /// <param name="strOutputFilePath">The path to the Pdf.</param>
        /// <param name="errorMessage">Used to retrieve an error message.</param>
        /// <param name="strInputFilePath">The path to the tiff.</param>
        /// <param name="textExtension">The extension for the text file.</param>
        /// <returns>true if the conversion succeded otherwise returns false.</returns>
        public static bool Convert(string strOutputFilePath, ref string errorMessage, string strInputFilePath,
            string textExtension = "")
        {
            using (GdPictureImaging gdPictureImaging = new GdPictureImaging())
            {
                // Load the image

                int imageId = gdPictureImaging.CreateGdPictureImageFromFile(strInputFilePath);
                if (imageId == 0)
                {
                    errorMessage = System.Convert.ToString(gdPictureImaging.GetStat().ToString());
                    gdPictureImaging.ReleaseGdPictureImage(imageId);
                    return false;
                }

                int pageCount = gdPictureImaging.GetPageCount(imageId);

                // Collect the information required for the conversion of the annotations

                PageInfo[] pageInfo = ReadPageInfo(gdPictureImaging, imageId, pageCount);

                using (MemoryStream stream = new MemoryStream())
                {
                    // Convert to Pdf in a stream

                    if (!ConvertToPdf(stream, ref errorMessage, gdPictureImaging, imageId, pageCount))
                    {
                        gdPictureImaging.ReleaseGdPictureImage(imageId);
                        return false;
                    }

                    gdPictureImaging.ReleaseGdPictureImage(imageId);

                    // Writes the annotations to the pdf

                    if (
                        !WriteAnnotations(strOutputFilePath, ref errorMessage, stream, pageCount, pageInfo,
                            textExtension))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// ConvertToPdf the provided TIFF to a PDF.
        /// </summary>
        /// <param name="stream">The stream where to write the Pdf.</param>
        /// <param name="errorMessage">Used to retrieve an error message.</param>
        /// <param name="gdPictureImaging">The GdPictureImaging.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <returns>true if the conversion succeded otherwise returns false.</returns>
        private static bool ConvertToPdf(Stream stream, ref string errorMessage, GdPictureImaging gdPictureImaging,
            int imageId,
            int pageCount)
        {
            using (GdPicturePDF gdPicturePdf = new GdPicturePDF())
            {
                gdPicturePdf.NewPDF(false);

                PdfAdvancedImageCompression advancedCompression = default(PdfAdvancedImageCompression);
                gdPicturePdf.SetCompressionForBitonalImage(PdfCompression.PdfCompressionJBIG2);
                gdPicturePdf.SetCompressionForColorImage(PdfCompression.PdfCompressionJPEG);

                for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                {
                    // Page selection is 1 based index
                    gdPictureImaging.SelectPage(imageId, pageIndex + 1);
                    gdPicturePdf.AddImageFromGdPictureImage(imageId, advancedCompression);
                    if (gdPicturePdf.GetStat() != GdPictureStatus.OK)
                    {
                        errorMessage = "Error adding bitmap to the PDF. Status: " + gdPicturePdf.GetStat().ToString();
                        gdPicturePdf.CloseDocument();
                        return false;
                    }
                }

                gdPicturePdf.SaveToStream(stream);

                if (gdPicturePdf.GetStat() != GdPictureStatus.OK)
                {
                    errorMessage = "Error saving PDF. Status: " + gdPicturePdf.GetStat().ToString();
                    gdPicturePdf.CloseDocument();
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// ReadPageInfo reads the page information required for the conversion of the annotations.
        /// </summary>
        /// <param name="gdPictureImaging">The GdPictureImaging.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <returns>An array with the information.</returns>
        private static PageInfo[] ReadPageInfo(GdPictureImaging gdPictureImaging, int imageId, int pageCount)
        {
            PageInfo[] pageInfo = new PageInfo[pageCount];

            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                // Page selection is 1 based index
                gdPictureImaging.SelectPage(imageId, pageIndex + 1);
                pageInfo[pageIndex].WidthPixels = gdPictureImaging.GetWidth(imageId);
                pageInfo[pageIndex].HeightPixels = gdPictureImaging.GetHeight(imageId);
                pageInfo[pageIndex].WidthInches = gdPictureImaging.GetWidthInches(imageId);
                pageInfo[pageIndex].HeightInches = gdPictureImaging.GetHeightInches(imageId);
                pageInfo[pageIndex].WangTagData = null;
                int tagCount = gdPictureImaging.TagCount(imageId);
                for (int tagIndex = 1; tagIndex <= tagCount; tagIndex++)
                {
                    Tags tag = gdPictureImaging.TagGetID(imageId, tagIndex);
                    if (tag == Tags.TagWangAnnotations)
                    {
                        int length = gdPictureImaging.TagGetLength(imageId, tagIndex);
                        pageInfo[pageIndex].WangTagData = new byte[length];
                        gdPictureImaging.TagGetValueBytes(imageId, tagIndex, ref pageInfo[pageIndex].WangTagData);
                    }
                }
            }

            return pageInfo;
        }

        /// <summary>
        /// WriteAnnotations writes the pdf to a file including its annotations.
        /// </summary>
        /// <param name="strOutputFilePath">The path to the output file.</param>
        /// <param name="errorMessage">Used to retrieve an error message.</param>
        /// <param name="stream">The stream containing the PDF.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="textExtension">The extension for the text file.</param>
        /// <returns>true if the conversion succeded otherwise returns false.</returns>
        private static bool WriteAnnotations(string strOutputFilePath, ref string errorMessage, MemoryStream stream,
            int pageCount,
            PageInfo[] pageInfo, string textExtension)
        {
            using (AnnotationManager annotationManager = new AnnotationManager())
            {
                // Load the pdf from the stream

                GdPictureStatus status = annotationManager.InitFromStream(stream);
                if (status != GdPictureStatus.OK)
                {
                    errorMessage = annotationManager.GetStat().ToString();
                    return false;
                }

                // Write the annotation from the pages

                for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                {
                    if (pageInfo[pageIndex].WangTagData != null)
                    {
                        annotationManager.SelectPage(pageIndex + 1);
                        bool wangAnnotationsReadingSuccess;
                        if (textExtension == "")
                        {
                            WangTiffToPdfWangAnnotationHandler handler =
                                new WangTiffToPdfWangAnnotationHandler(pageInfo[pageIndex].WidthPixels,
                                    pageInfo[pageIndex].HeightPixels, pageInfo[pageIndex].WidthInches,
                                    pageInfo[pageIndex].HeightInches, annotationManager, null);
                            wangAnnotationsReadingSuccess = WangTagReadingLibrary.WangAnnotationsReader.Read(handler,
                                pageInfo[pageIndex].WangTagData);
                        }
                        else
                        {
                            using (
                                StreamWriter textWriter =
                                    new StreamWriter(strOutputFilePath + "." + pageIndex + textExtension))
                            {
                                WangTiffToPdfWangAnnotationHandler handler =
                                    new WangTiffToPdfWangAnnotationHandler(pageInfo[pageIndex].WidthPixels,
                                        pageInfo[pageIndex].HeightPixels, pageInfo[pageIndex].WidthInches,
                                        pageInfo[pageIndex].HeightInches, annotationManager, textWriter);
                                wangAnnotationsReadingSuccess = WangTagReadingLibrary.WangAnnotationsReader.Read(
                                    handler,
                                    pageInfo[pageIndex].WangTagData);
                            }
                        }

                        status = wangAnnotationsReadingSuccess
                            ? annotationManager.BurnAnnotationsToPage(true)
                            : GdPictureStatus.GenericError;

                        if (status != GdPictureStatus.OK)
                        {
                            annotationManager.Close();
                            errorMessage = wangAnnotationsReadingSuccess
                                ? annotationManager.GetStat().ToString()
                                : "An error occurred while reading Wang annotations.";
                            return false;
                        }
                    }
                }

                // Writes the pdf as a file

                status = annotationManager.SaveDocumentToPDF(strOutputFilePath);
                if (status != GdPictureStatus.OK)
                {
                    errorMessage = annotationManager.GetStat().ToString();
                    return false;
                }
                annotationManager.Close();
            }
            return true;
        }
    }
}
