using System.IO;

using FluentAssertions;

using NppXmlTreeviewPlugin.Parsers.Tests.Unit.Stubs;

using Xunit;

namespace NppXmlTreeviewPlugin.Parsers.Tests.Unit
{
    public class WhenParsingValidXml
    {
        [Theory]
        [InlineData(@"./TestFiles/valid_comments.xml")]
        [InlineData(@"./TestFiles/valid_nocomments.xml")]
        public void GivenValidXml_ThenTryParseIsValid(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            var result = NppXmlNode.TryParse(xml, new LoggerStub(), out nppXmlNode);

            result.Should().BeTrue(because: "the XML is valid");
        }

        [Theory]
        [InlineData(@"./TestFiles/valid_comments.xml", 3)]
        [InlineData(@"./TestFiles/valid_nocomments.xml", 3)]
        public void GivenValidXml_ThenNumberOfChildNodesMatch(string path, int nodeCount)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            NppXmlNode.TryParse(xml, new LoggerStub(), out nppXmlNode);

            nppXmlNode.ChildNodes.Count.Should().Be(nodeCount, because: $"the number of child nodes is {nodeCount}");
        }
    }
}