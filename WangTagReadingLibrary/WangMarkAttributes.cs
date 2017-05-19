namespace WangTagReadingLibrary
{
    /// <summary>
    /// WangMarkType enumerates the different Wang annotation types.
    /// </summary>
    public enum WangMarkType
    {
        /// <summary>
        /// 1 = Image embedded
        /// </summary>
        ImageEmbedded,

        /// <summary>
        /// 2 = Image reference
        /// </summary>
        ImageReference,

        /// <summary>
        /// 3 = Straight line
        /// </summary>
        StraightLine,

        /// <summary>
        /// 4 = Freehand line
        /// </summary>
        FreehandLine,

        /// <summary>
        /// 5 = Hollow rectangle
        /// </summary>
        HollowRectangle,

        /// <summary>
        /// 6 = Filled rectangle
        /// </summary>
        FilledRectangle,

        /// <summary>
        /// 7 = Typed text
        /// </summary>
        TypedText,

        /// <summary>
        /// 8 = Text from file
        /// </summary>
        TextFromFile,

        /// <summary>
        /// 9 = Text stamp
        /// </summary>
        TextStamp,

        /// <summary>
        /// 10 = Attach-a-Note
        /// </summary>
        AttachANote,

        /// <summary>
        /// 12 = Form
        /// </summary>
        Form,

        /// <summary>
        /// 13 = OCR region
        /// </summary>
        OcrRegion,

        /// <summary>
        /// 14 = Hollow polygon
        /// </summary>
        /// <remarks>
        /// Please note this type was not mentionned within the documentation. The implementation
        /// of this type is kind of reverse engineering.
        /// </remarks>
        HollowPolygon,

        /// <summary>
        /// 15 = Filled polygon
        /// </summary>
        /// <remarks>
        /// Please note this type was not mentionned within the documentation. The implementation
        /// of this type is kind of reverse engineering.
        /// </remarks>
        FilledPolygon,

        /// <summary>
        /// Invalid
        /// </summary>
        Invalid,
    }

    /// <summary>
    /// The WangMarkAttributes class is designed to hold the properties available when the specifications are mentionning the availability of an OIAN_MARK_ATTRIBUTES structure.
    /// </summary>
    public class WangMarkAttributes
    {
        /// <summary>
        /// The constructor initializes members with the provided values.
        /// </summary>
        public WangMarkAttributes(WangMarkType type)
        {
            Type = type;
            Bounds = new int[4];
            Color1 = new byte[3];
            Color2 = new byte[3];
        }

        /// <summary>
        /// The type of the mark.
        /// </summary>
        public WangMarkType Type { get; private set; }

        /// <summary>
        /// The bounds for the annotation.
        /// </summary>
        /// <remarks>
        /// The order is left, top, right, bottom, see WangAnnotationTranslation.
        /// </remarks>
        public int[] Bounds { get; private set; }

        /// <summary>
        /// The main color; for example, the color of all lines, all rectangles, and standalone text.
        /// </summary>
        /// <remarks>
        /// The order is blue, green, red, see WangAnnotationTranslation.
        /// </remarks>
        public byte[] Color1 { get; private set; }

        /// <summary>
        /// The secondary color; for example, the color of the text of an Attach-a-Note.
        /// </summary>
        /// <remarks>
        /// The order is blue, green, red,, see WangAnnotationTranslation.
        /// </remarks>
        public byte[] Color2 { get; private set; }

        /// <summary>
        /// The flag indicating whether the mark is drawn highlighted or not.
        /// </summary>
        /// <remarks>
        /// Highlighting  performs the same function as a highlighting marker on a  piece of paper. 
        /// Valid only for lines, rectangles, and freehand.
        /// </remarks>
        public bool Highlighting;

        /// <summary>
        /// The flag indicating whether the mark is drawn transparent or not.
        /// </summary>
        /// <remarks>
        /// A transparent  mark does not draw white pixels. That is, transparentreplaces white pixels with whatever is behind those pixels.
        /// Available only for images.
        /// </remarks>
        public bool Transparent;

        /// <summary>
        /// The width of the line in pixels.
        /// </summary>
        public uint LineSize;

        /// <summary>
        /// The font information for the text, consisting of standard  font attributes of font size, name, style, effects, and  background color.
        /// </summary>
        public WangLogFont LogFont;

        // TODO - David Ometto - 2016-11-21 - Add support for reading time_t structure.
        /*
        /// <summary>
        /// The time that the mark was first saved, in seconds, from 00:00:00 1-1-1970 GMT. 
        /// </summary>
        /// <remarks>
        /// Every annotation mark has  time as one of its attributes. 
        /// If you do not set the time before the file is saved, the time is set to the date and time that the save was initiated. 
        /// This time is in the form returned by the "time" C call, which is the number of seconds since midnight 00:00:00 on 1-1-1970 GMT. 
        /// If necessary, refer to your C documentation for a more detailed description.
        ///</remarks>
        public byte[] Time;
        */

        /// <summary>
        /// The flag indicating whether the annotation is visible or not.
        /// </summary>
        public bool Visible;
    }
}
