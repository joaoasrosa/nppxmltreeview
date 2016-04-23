using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace NppXmlTreeviewPlugin.Parsers
{
    /// <summary>
    /// Represents a Notepadd++ Xml Node.
    /// </summary>
    public class NppXmlNode
    {
        /// <summary>
        /// Stores the line position.
        /// </summary>
        private class LinePosition
        {
            private int _previousLineNumber;
            private int _previousLinePosition;

            public void SetPosition(int lineNumber, int linePosition, int lineShift)
            {
                if (lineNumber.Equals(this._previousLineNumber))
                {
                    this.Position += linePosition - this._previousLinePosition;
                }
                else
                {
                    this.Position += linePosition + lineShift;
                }

                this._previousLineNumber = lineNumber;
                this._previousLinePosition = linePosition;
            }

            public int Position { get; private set; }
        }

        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="startPosition">The node start position.</param>
        internal NppXmlNode(string name, int startPosition)
        {
            this.Name = name;
            this.StartPosition = startPosition;
            this.ChildNodes = new List<NppXmlNode>();
        }

        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="startPosition">The node start position.</param>
        /// <param name="parent">The parent node.</param>
        internal NppXmlNode(string name, int startPosition, NppXmlNode parent)
        {
            this.Name = name;
            this.StartPosition = startPosition;
            this.ChildNodes = new List<NppXmlNode>();
            this.Parent = parent;
        }

        /// <summary>
        /// Method to try parse the XML.
        /// </summary>
        /// <param name="xml">The XMl as string.</param>
        /// <param name="nppXmlNode">The Notepad++ XmlNode.</param>
        /// <returns>True if parse successfully, false otherwise.</returns>
        public static bool TryParse(string xml, out NppXmlNode nppXmlNode)
        {
            nppXmlNode = null;
            var position = new LinePosition();

            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    using (var xmlTextReader = new XmlTextReader(stringReader))
                    {
                        while (xmlTextReader.Read())
                        {
                            position.SetPosition(xmlTextReader.LineNumber, xmlTextReader.LinePosition,
                                GetLineShift(xmlTextReader.Value, xmlTextReader.NodeType));

                            if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                            {
                                continue;
                            }

                            nppXmlNode = new NppXmlNode(xmlTextReader.Name, position.Position);
                            ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, nppXmlNode, position);
                        }
                        if (null == nppXmlNode)
                        {
                            throw new ArgumentException("nppXmlNode");
                        }

                        nppXmlNode.EndPosition = xml.Length;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                nppXmlNode = null;
                return false;
            }
        }

        /// <summary>
        /// Method to read a child or a sibling of the node.
        /// </summary>
        /// <param name="xmlTextReader">The XML text reader.</param>
        /// <param name="currentDepth">The current depth on the XML tree.</param>
        /// <param name="node">The XML node</param>
        /// <param name="position">The current position in the text.</param>
        private static void ReadChildOrSibling(XmlTextReader xmlTextReader, int currentDepth, NppXmlNode node, LinePosition position)
        {
            while (xmlTextReader.Read())
            {
                position.SetPosition(xmlTextReader.LineNumber, xmlTextReader.LinePosition,
                    GetLineShift(xmlTextReader.Value, xmlTextReader.NodeType));

                // It's a sibling.
                if (currentDepth == xmlTextReader.Depth)
                {
                    if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                    {
                        return;
                    }

                    var sibling = new NppXmlNode(xmlTextReader.Name, position.Position, node.Parent);
                    node.Parent.ChildNodes.Add(sibling);
                    ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, sibling, position);

                    position.SetPosition(xmlTextReader.LineNumber, xmlTextReader.LinePosition,
                        GetLineShift(xmlTextReader.Value, xmlTextReader.NodeType));
                    sibling.EndPosition = position.Position;

                    return;
                }

                if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                {
                    continue;
                }

                // It's a child.
                var child = new NppXmlNode(xmlTextReader.Name, position.Position, node);
                node.ChildNodes.Add(child);
                ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, child, position);

                position.SetPosition(xmlTextReader.LineNumber, xmlTextReader.LinePosition,
                    GetLineShift(xmlTextReader.Value, xmlTextReader.NodeType));
                child.EndPosition = position.Position;
            }
        }

        /// <summary>
        /// Get the line shift base on tab ('\t'), whitespaces and node type ocurrences.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="xmlNodeType">The node type.</param>
        /// <returns>The line shift.</returns>
        private static int GetLineShift(string value, XmlNodeType xmlNodeType)
        {
            switch (xmlNodeType)
            {
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Whitespace:
                    return Regex.Matches(value, "\t").Count * 4 + Regex.Matches(value, " ").Count;
                case XmlNodeType.EndElement:
                    return 3;
                default:
                    return 0;
            }

        }

        /// <summary>
        /// The start position of the node.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// The end position of the node.
        /// </summary>
        public int EndPosition { get; private set; }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The childrens of the node.
        /// </summary>
        public List<NppXmlNode> ChildNodes { get; }

        /// <summary>
        /// The parent of the node.
        /// </summary>
        public NppXmlNode Parent { get; }

        /// <summary>
        /// Flag to indicate if the node has child nodes.
        /// </summary>
        public bool HasChildNodes => this.ChildNodes.Any();
    }
}
