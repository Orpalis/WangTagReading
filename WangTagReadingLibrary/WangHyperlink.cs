namespace WangTagReadingLibrary
{
    /// <summary>
    /// The WangHyperlink class manages the data required when a HYPERLINK_NB is mentionned in the specifications.
    /// </summary>
    public class WangHyperlink
    {
        /// <summary>
        /// The constructor initializes members with the provided values.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="location">The location.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="internalLink">Flag for hyperlink refers to this document.</param>
        /// <param name="canRemoveHyperlink">Flag for can remove hyperlink from mark.</param>
        public WangHyperlink(string link, string location, string workingDirectory, bool internalLink,
            bool canRemoveHyperlink)
        {
            Link = link;
            Location = location;
            WorkingDirectory = workingDirectory;
            InternalLink = internalLink;
            CanRemoveHyperlink = canRemoveHyperlink;
        }


        /// <summary>
        /// The link.
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// The location.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// The working directory.
        /// </summary>
        public string WorkingDirectory { get; private set; }

        /// <summary>
        /// Flag for hyperlink refers to this document.
        /// </summary>
        public bool InternalLink { get; private set; }

        /// <summary>
        /// Flag for can remove hyperlink from mark.
        /// </summary>
        public bool CanRemoveHyperlink { get; private set; }
    }
}
