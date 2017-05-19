namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangNamedBlockHeader class holds the header of a named block.
    /// </summary>
    public class WangNamedBlockHeader
    {
        /// <summary>
        /// The constructor initializes members with the provided values.
        /// </summary>
        /// <param name="name">Name of named block.</param>
        /// <param name="size">Size of named block.</param>
        public WangNamedBlockHeader(string name, int size)
        {
            Name = name;
            Size = size;
        }

        /// <summary>
        /// Name of named block.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Size of named block.
        /// </summary>
        public int Size { get; private set; }
    }
}
