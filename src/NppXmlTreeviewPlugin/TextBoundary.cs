using NppXmlTreeviewPlugin.Parsers;

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
        /// <param name="startNodePosition">The start node position.</param>
        /// <param name="endNodePosition">The end index.</param>
        public TextBoundary(NppXmlNodePosition startNodePosition, NppXmlNodePosition endNodePosition)
        {
            this.StartNodePosition = startNodePosition;
            this.EndNodePosition = endNodePosition;
        }

        /// <summary>
        /// The start node position of the text.
        /// </summary>
        public NppXmlNodePosition StartNodePosition { get; private set; }

        /// <summary>
        /// The end node position of the text.
        /// </summary>
        public NppXmlNodePosition EndNodePosition { get; private set; }
    }
}
