using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace NppXmlTreeviewPlugin.Parsers
{
    /// <summary>
    /// Represents a Notepadd++ Xml Node.
    /// </summary>
    public class NppXmlNode
    {
        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="startPosition">The node start position.</param>
        internal NppXmlNode(string name, NppXmlNodePosition startPosition)
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
        internal NppXmlNode(string name, NppXmlNodePosition startPosition, NppXmlNode parent)
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

            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    using (var xmlTextReader = new XmlTextReader(stringReader))
                    {
                        while (xmlTextReader.Read())
                        {
                            if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                            {
                                continue;
                            }

                            nppXmlNode = new NppXmlNode(xmlTextReader.Name, new NppXmlNodePosition(xmlTextReader));
                            
                            ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, nppXmlNode);
                        }
                        if (null == nppXmlNode)
                        {
                            throw new ArgumentException("nppXmlNode");
                        }

                        nppXmlNode.EndPosition = new NppXmlNodePosition(xmlTextReader, true);

                        return true;
                    }
                }
            }
            catch 
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
        private static void ReadChildOrSibling(XmlTextReader xmlTextReader, int currentDepth, NppXmlNode node)
        {
            while (xmlTextReader.Read())
            {
                // It's a sibling.
                if (currentDepth == xmlTextReader.Depth)
                {
                    if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                    {
                        return;
                    }
                    
                    var sibling = new NppXmlNode(xmlTextReader.Name, new NppXmlNodePosition(xmlTextReader), node.Parent);
                    node.Parent.ChildNodes.Add(sibling);
                    
                    ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, sibling);

                    sibling.EndPosition = new NppXmlNodePosition(xmlTextReader, true);

                    return;
                }

                if (xmlTextReader.NodeType != XmlNodeType.Element || !xmlTextReader.IsStartElement())
                {
                    continue;
                }

                // It's a child.
                var child = new NppXmlNode(xmlTextReader.Name, new NppXmlNodePosition(xmlTextReader), node);
                node.ChildNodes.Add(child);
                
                ReadChildOrSibling(xmlTextReader, xmlTextReader.Depth, child);

                child.EndPosition = new NppXmlNodePosition(xmlTextReader, true);
            }
        }

        /// <summary>
        /// The start position of the node.
        /// </summary>
        public NppXmlNodePosition StartPosition { get; private set; }

        /// <summary>
        /// The end position of the node.
        /// </summary>
        public NppXmlNodePosition EndPosition { get; private set; }

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
