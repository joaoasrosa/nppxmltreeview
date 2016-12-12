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
        /// <param name="id">The node id.</param>
        public TextBoundary(NppXmlNodePosition startNodePosition, NppXmlNodePosition endNodePosition, int id)
        {
            this.StartNodePosition = startNodePosition;
            this.EndNodePosition = endNodePosition;
            this.Id = id;
        }

        /// <summary>
        /// The start node position of the text.
        /// </summary>
        public NppXmlNodePosition StartNodePosition { get; private set; }

        /// <summary>
        /// The end node position of the text.
        /// </summary>
        public NppXmlNodePosition EndNodePosition { get; private set; }

        /// <summary>
        /// The node id.
        /// </summary>
        public int Id { get; private set; }
    }
}
