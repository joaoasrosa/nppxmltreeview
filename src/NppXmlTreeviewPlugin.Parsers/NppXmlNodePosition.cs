using System.Xml;

namespace NppXmlTreeviewPlugin.Parsers
{
    /// <summary>
    ///     The npp xml node position.
    /// </summary>
    public class NppXmlNodePosition
    {
        /// <summary>
        ///     The line number.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        ///     The line position.
        /// </summary>
        public int LinePosition { get; }

        /// <summary>
        ///     Internal constructor
        /// </summary>
        /// <param name="xmlTextReader">The XML text reader.</param>
        /// <param name="isEndPosition">Flag to indicate if it is a end position.</param>
        internal NppXmlNodePosition(XmlTextReader xmlTextReader, bool isEndPosition = false)
        {
            LineNumber = xmlTextReader.LineNumber - 1;
            LinePosition = xmlTextReader.LinePosition + (isEndPosition ? xmlTextReader.Name.Length : -2);
        }
    }
}