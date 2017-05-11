namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangDisplayText class is designed to hold the properties available when the specifications are mentionning the availability of an OIAN_TEXTPRIVDATA structure.
    /// </summary>
    public class WangDisplayText
    {
        /// <summary>
        /// The constructor initializes members with the provided parameters.
        /// </summary>
        /// <param name="orientation">Angle of text baseline to image.</param>
        /// <param name="creationScale">The creation scale.</param>
        /// <param name="text">The text.</param>
        public WangDisplayText(int orientation, uint creationScale, string text)
        {
            Orientation = orientation;
            CreationScale = creationScale;
            Text = text;
        }

        /// <summary>
        /// Angle of text baseline to image in tenths of a degree; valid values are 0, 900, 1800, 2700.
        /// </summary>
        // TODO - David Ometto - 2016-11-24 - Replace the int with an enum to make sure we correctly translate the value.
        public int Orientation { get; }

        /// <summary>
        /// The creation scale.
        /// Always 72000 divided by the vertical resolution of the  base image when writing.
        /// </summary>
        public uint CreationScale { get; }

        /// <summary>
        /// The text for the annotation.
        /// </summary>
        public string Text { get; }
    }
}
