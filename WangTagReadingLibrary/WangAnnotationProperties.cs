namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangAnnotationProperties class holds the properties for a Wang annotation.
    /// </summary>
    internal sealed class WangAnnotationProperties
    {
        /// <summary>
        /// The constructor initializes members to their default values.
        /// </summary>
        public WangAnnotationProperties()
        {
            _markAttributes = null;
            OiGroup = null;
            OiIndex = null;
            _displayText = null;
            _points = null;
            _filename = null;
            _dibInfo = null;
            _rotation = null;
            _hyperlink = null;
        }

        /// <summary>
        /// CopyFrom copies the values for the provided object.
        /// </summary>
        /// <param name="toCopy">The object to be copied.</param>
        public void CopyFrom(WangAnnotationProperties toCopy)
        {
            _markAttributes = toCopy._markAttributes;
            OiGroup = toCopy.OiGroup;
            OiIndex = toCopy.OiIndex;
            _displayText = toCopy._displayText;
            _points = toCopy._points;
            _filename = toCopy._filename;
            _dibInfo = toCopy._dibInfo;
            _rotation = toCopy._rotation;
            _hyperlink = toCopy._hyperlink;
        }

        /// <summary>
        /// SetMarkAttributes sets the mark attributes.
        /// </summary>
        /// <param name="markAttributes">The mark attributes.</param>
        public void SetMarkAttributes(WangMarkAttributes markAttributes)
        {
            _markAttributes = markAttributes;
        }

        /// <summary>
        /// SetDisplayText sets the display text.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        public void SetDisplayText(WangDisplayText displayText)
        {
            _displayText = displayText;
        }

        /// <summary>
        /// SetHyperlink sets the hyperlink.
        /// </summary>
        /// <param name="hyperlink">The hyperlink.</param>
        public void SetHyperlink(WangHyperlink hyperlink)
        {
            _hyperlink = hyperlink;
        }

        /// <summary>
        /// SetPoints sets the points.
        /// </summary>
        /// <param name="points">The points.</param>
        public void SetPoints(int [] points)
        {
            _points = points;
        }

        /// <summary>
        /// SetFilename sets the filename.
        /// </summary>
        /// <param name="filename">The file name.</param>
        public void SetFilename(string filename)
        {
            _filename = filename;
        }

        /// <summary>
        /// SetDibInfo sets the DIB info.
        /// </summary>
        /// <param name="dibInfo">The DIB info.</param>
        public void SetDibInfo(byte[] dibInfo)
        {
            _dibInfo = dibInfo;
        }

        /// <summary>
        /// SetRotation sets the rotation.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        public void SetRotation(WangRotation rotation)
        {
            _rotation = rotation;
        }

        /// <summary>
        /// The flag indicating whether attributes have been are available or not.
        /// </summary>
        public bool HasMarkAttributes
        {
            get { return _markAttributes != null; }
        }

        /// <summary>
        /// The mark attributes.
        /// </summary>
        public WangMarkAttributes MarkAttributes
        {
            get { return _markAttributes; }
        }

        /// <summary>
        /// The flag indicating whether display text is available or not.
        /// </summary>
        public bool HasDisplayText
        {
            get { return _displayText != null; }
        }

        /// <summary>
        /// The display text.
        /// </summary>
        public WangDisplayText DisplayText
        {
            get { return _displayText; }
        }

        /// <summary>
        /// The flag indicating whether hyperlink is available or not.
        /// </summary>
        public bool HasHyperlink
        {
            get { return _hyperlink != null; }
        }
        
        /// <summary>
        /// The hyperlink.
        /// </summary>
        public WangHyperlink Hyperlink
        {
            get { return _hyperlink;  }
        }

        /// <summary>
        /// The flag indicating whether points are available or not.
        /// </summary>
        public bool HasPoints
        {
            get { return _points != null; }
        }

        /// <summary>
        /// The points.
        /// </summary>
        public int[] Points
        {
            get { return _points; }
        }

        /// <summary>
        /// The flag indicating whether a filename is available or not.
        /// </summary>
        public bool HasFilename
        {
            get { return _filename != null; }
        }

        /// <summary>
        /// The filename.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// The flag indicating whether a DIB is available or not.
        /// </summary>
        public bool HasDib
        {
            get { return _dibInfo != null; }
        }

        /// <summary>
        /// The flag indicating whether rotation is available or not.
        /// </summary>
        public bool HasRotation
        {
            get { return _rotation != null; }
        }

        /// <summary>
        /// The rotation.
        /// </summary>
        public WangRotation Rotation
        {
            get { return _rotation; }
        }

        /// <summary>
        /// The DIB info.
        /// </summary>
        public byte[] DibInfo
        {
            get { return _dibInfo; }
        }

        /// <summary>
        /// OiGroup.
        /// </summary>
        public string OiGroup { get; set; }

        /// <summary>
        /// OiIndex.
        /// </summary>
        public string OiIndex { get; set; }

        /// <summary>
        /// The attributes.
        /// </summary>
        private WangMarkAttributes _markAttributes;

        /// <summary>
        /// Display text.
        /// </summary>
        private WangDisplayText _displayText;

        /// <summary>
        /// The points.
        /// </summary>
        private int[] _points;

        /// <summary>
        /// The filename.
        /// </summary>
        private string _filename;

        /// <summary>
        /// The DIB info.
        /// </summary>
        private byte[] _dibInfo;

        /// <summary>
        /// The rotation.
        /// </summary>
        private WangRotation _rotation;

        /// <summary>
        /// The hyperlink.
        /// </summary>
        private WangHyperlink _hyperlink;
    }
}
