using System.IO;

using FluentAssertions;

using Xunit;

namespace NppXmlTreeviewPlugin.Parsers.Tests.Unit
{
    public class WhenParsingInvalidXml
    {
        [Theory]
        [InlineData(@"./TestFiles/invalid_comments.xml")]
        [InlineData(@"./TestFiles/invalid_nocomments.xml")]
        public void GivenValidXml_ThenTryParseIsValid(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            var result = NppXmlNode.TryParse(xml, out nppXmlNode);

            result.Should().BeFalse(because: "the XML is invalid");
        }

        [Theory]
        [InlineData(@"./TestFiles/invalid_comments.xml")]
        [InlineData(@"./TestFiles/invalid_nocomments.xml")]
        public void GivenValidXml_ThenNumberOfChildNodesMatch(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode = null;

            NppXmlNode.TryParse(xml, out nppXmlNode);

            nppXmlNode.Should().BeNull(because: "the XML is invalid");
        }
    }
}