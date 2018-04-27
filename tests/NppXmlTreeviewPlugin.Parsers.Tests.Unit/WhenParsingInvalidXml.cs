using System.IO;

using FluentAssertions;

using NppXmlTreeviewPlugin.Parsers.Tests.Unit.Stubs;

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

            var result = NppXmlNode.TryParse(xml, new LoggerStub(), out nppXmlNode);

            result.Should().BeFalse(because: "the XML is invalid");
        }

        [Theory]
        [InlineData(@"./TestFiles/invalid_comments.xml")]
        [InlineData(@"./TestFiles/invalid_nocomments.xml")]
        public void GivenValidXml_ThenNumberOfChildNodesMatch(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode = null;

            NppXmlNode.TryParse(xml, new LoggerStub(), out nppXmlNode);

            nppXmlNode.Should().BeNull(because: "the XML is invalid");
        }
    }
}