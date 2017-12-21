using System.IO;
using GdPicture14;

namespace WangTiffToTiff
{
    /// <summary>
    /// The WangTiffToTiffConvert class offers conversion from Wang annotated TIFF to GdPicture/XMP annotated TIFF.
    /// </summary>
    internal static class WangTiffToGdPictureTiffConvert
    {
        /// <summary>
        /// The PageInfo structure holds a couple of information required to convert the coordinates of the Wang annotation.
        /// </summary>
        private struct PageInfo
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
        /// Convert the provided Wang annotated TIFF to a GdPicture/XMP annotated TIFF.
        /// </summary>
        /// <param name="strOutputFilePath">The path to the output tiff.</param>
        /// <param name="errorMessage">Used to retrieve an error message.</param>
        /// <param name="strInputFilePath">The path to the input tiff.</param>
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

                // Save tiff to output file
                GdPictureStatus status = pageCount > 1
                    ? gdPictureImaging.TiffSaveMultiPageToFile(imageId, strOutputFilePath,
                        TiffCompression.TiffCompressionAUTO, 100)
                    : gdPictureImaging.SaveAsTIFF(imageId, strOutputFilePath, false,
                        TiffCompression.TiffCompressionAUTO, 100);

                gdPictureImaging.ReleaseGdPictureImage(imageId);

                if (status != GdPictureStatus.OK)
                {
                    return false;
                }

                // Writes the annotations to the tiff
                return WriteAnnotations(strOutputFilePath, ref errorMessage, pageInfo, pageCount, textExtension);
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
        /// WriteAnnotations writes the tiff to a file including its annotations.
        /// </summary>
        /// <param name="strOutputFilePath">The path to the output file.</param>
        /// <param name="errorMessage">Used to retrieve an error message.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <param name="pageInfo">The page information.</param>
        /// <param name="textExtension">The extension for the text file.</param>
        /// <returns>true if the conversion succeded otherwise returns false.</returns>
        private static bool WriteAnnotations(string strOutputFilePath, ref string errorMessage, PageInfo[] pageInfo,
            int pageCount, string textExtension)
        {
            // Load the tiff from the stream
            using (MemoryStream stream = new MemoryStream())
            {
                using (FileStream fileStream = File.OpenRead(strOutputFilePath))
                {
                    stream.SetLength(fileStream.Length);
                    fileStream.Read(stream.GetBuffer(), 0, (int)fileStream.Length);
                }

                using (AnnotationManager annotationManager = new AnnotationManager())
                {
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
                                WangTiffToGdPictureTiffWangAnnotationHandler handler =
                                    new WangTiffToGdPictureTiffWangAnnotationHandler(pageInfo[pageIndex].WidthPixels,
                                        pageInfo[pageIndex].HeightPixels, pageInfo[pageIndex].WidthInches,
                                        pageInfo[pageIndex].HeightInches, annotationManager, null);
                                wangAnnotationsReadingSuccess = WangTagReadingLibrary.WangAnnotationsReader.Read(
                                    handler,
                                    pageInfo[pageIndex].WangTagData);
                            }
                            else
                            {
                                using (
                                    StreamWriter textWriter =
                                        new StreamWriter(strOutputFilePath + "." + pageIndex + textExtension))
                                {
                                    WangTiffToGdPictureTiffWangAnnotationHandler handler =
                                        new WangTiffToGdPictureTiffWangAnnotationHandler(
                                            pageInfo[pageIndex].WidthPixels,
                                            pageInfo[pageIndex].HeightPixels, pageInfo[pageIndex].WidthInches,
                                            pageInfo[pageIndex].HeightInches, annotationManager, textWriter);
                                    wangAnnotationsReadingSuccess = WangTagReadingLibrary.WangAnnotationsReader.Read(
                                        handler,
                                        pageInfo[pageIndex].WangTagData);
                                }
                            }
                            if (!wangAnnotationsReadingSuccess)
                            {
                                annotationManager.Close();
                                errorMessage = "An error occurred while reading Wang annotations.";
                                return false;
                            }
                        }
                    }

                    // Writes the tiff as a file
                    status = annotationManager.SaveDocumentToTIFF(strOutputFilePath,
                        TiffCompression.TiffCompressionAUTO);
                    if (status != GdPictureStatus.OK)
                    {
                        errorMessage = annotationManager.GetStat().ToString();
                        return false;
                    }
                    annotationManager.Close();
                }
            }
            return true;
        }
    }
}
