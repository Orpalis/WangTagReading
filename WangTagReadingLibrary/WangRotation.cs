namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangRotation structure is designed to hold the properties available when the specifications are mentionning the availability of an AN_NEW_ROTATE_STRUCT structure.
    /// </summary>
    public class WangRotation
    {
        /// <summary>
        /// The constructor initializes members with the provided values.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="horizontalResolution">Horizontal resolution of image mark in DPI.</param>
        /// <param name="verticalResolution">Vertical resolution of image mark in DPI.</param>
        public WangRotation(int rotation, int horizontalResolution, int verticalResolution)
        {
            Rotation = rotation;
            OrigHRes = horizontalResolution;
            OrigVRes = verticalResolution;
        }

        /// <summary>
        /// The Rotation.
        /// </summary>
        /// <remarks>
        /// 1=Original
        /// 2=Rotate right (90 degrees clockwise)
        /// 3=Flip (180 degrees clockwise)
        /// 4=Rotate left (270 degrees clockwise)
        /// 5=Vertical mirror (reflected around a 
        /// vertical line)
        /// 6=Vertical mirror + Rotate right
        /// 7=Vertical mirror + Flip
        /// 8=Vertical mirror + Rotate left
        /// </remarks>
        // TODO - David Ometto - 2016-11-24 - Replace the int with an enum to make sure we correctly translate the value (I fear we handle it as an angle for the time being).
        public int Rotation;

        /// <summary>
        /// Horizontal resolution of image mark in DPI.
        /// </summary>
        public int OrigHRes;

        /// <summary>
        /// Vertical resolution of image mark in DPI.
        /// </summary>
        public int OrigVRes;
    }
}
