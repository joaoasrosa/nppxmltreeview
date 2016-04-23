namespace NppXmlTreeviewPlugin
{
    /// <summary>
    /// Struct to store the text boundary for the treenode.
    /// </summary>
    public struct TextBoundary
    {
        /// <summary>
        /// Default struct constructor.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public TextBoundary(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        /// <summary>
        /// The start index of the text.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// The end index of the text.
        /// </summary>
        public int EndIndex { get; private set; }
    }
}
