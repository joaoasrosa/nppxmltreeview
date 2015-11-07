using System.Xml;

namespace NppXmlTreviewPlugin.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Method to validate a string as XML.
        /// </summary>
        /// <param name="xmlString">The xml string.</param>
        /// <param name="xmlDocument">The xml document to parse it.</param>
        /// <returns>True if the string is a valid xml, false otherwise.</returns>
        public static bool IsValidXml(this string xmlString, out XmlDocument xmlDocument)
        {
            try
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                return true;
            }
            catch
            {
                xmlDocument = null;
                return false;
            }
        }
    }
}
