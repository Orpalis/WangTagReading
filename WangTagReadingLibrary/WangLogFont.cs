namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangLogFont class is designed to hold the properties available when the specifications are mentionning the availability of a LOGFONT structure.
    /// </summary>
    public class WangLogFont
    {
        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public int Height;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public int Width;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public int Escapement;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public int Orientation;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public int Weight;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public bool Italic;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public bool Underline;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public bool StrikeOut;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public byte CharSet;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public byte OutPrecision;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public byte ClipPrecision;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public byte Quality;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public byte PitchAndFamily;

        /// <summary>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
        /// </summary>
        public string FaceName;
    }
}
