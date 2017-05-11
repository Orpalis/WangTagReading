namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangAnnotationTranslation class offers a few utilities for interpret Wang annotations.
    /// </summary>
    internal static class WangAnnotationTranslation
    {
        /// <summary>
        /// The 0 based index of the left coordinate in an array detailing a rectangle.
        /// </summary>
        public const int LeftIndex = 0;

        /// <summary>
        /// The 0 based index of the top coordinate in an array detailing a rectangle.
        /// </summary>
        public const int TopIndex = 1;

        /// <summary>
        /// The 0 based index of the right coordinate in an array detailing a rectangle.
        /// </summary>
        public const int RightIndex = 2;

        /// <summary>
        /// The 0 based index of the bottom coordinate in an array detailing a rectangle.
        /// </summary>
        public const int BottomIndex = 3;

        /// <summary>
        /// The 0 based index of the blue component in an array detailing a color.
        /// </summary>
        public const int BlueIndex = 0;

        /// <summary>
        /// The 0 based index of the green component in an array detailing a color.
        /// </summary>
        public const int GreenIndex = 1;

        /// <summary>
        /// The 0 based index of the red component in an array detailing a color.
        /// </summary>
        public const int RedIndex = 2;

        /// <summary>
        /// PointX retrieves the X value for the point at the given index.
        /// </summary>
        /// <param name="index">The 0 based index of the point.</param>
        /// <param name="values">The array of values.</param>
        /// <returns>X</returns>
        public static int PointX(int index, int[] values)
        {
            return values[index*2];
        }

        /// <summary>
        /// PointY retrieves the Y value for the point at the given index.
        /// </summary>
        /// <param name="index">The 0 based index of the point.</param>
        /// <param name="values">The array of values.</param>
        /// <returns>Y</returns>
        public static int PointY(int index, int[] values)
        {
            return values[index * 2 + 1];
        }

        /// <summary>
        /// PointsCount retrieves the number of points within the values.
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <returns>The number of values.</returns>
        public static int PointsLength(int[] values)
        {
            return values.Length/2;
        }
    }
}
