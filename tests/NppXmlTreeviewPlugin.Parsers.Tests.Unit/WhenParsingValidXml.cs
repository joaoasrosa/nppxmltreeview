using System.IO;

using FluentAssertions;

using Xunit;

namespace NppXmlTreeviewPlugin.Parsers.Tests.Unit
{
    public class WhenParsingValidXml
    {
        [Theory]
        [InlineData(@"./TestFiles/NPP_comments.xml")]
        [InlineData(@"./TestFiles/NPP_nocomments.xml")]
        public void GivenValidXml_ThenTryParseIsValid(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            var result = NppXmlNode.TryParse(xml, out nppXmlNode);

            result.Should().BeTrue(because: "the XML is valid");
        }

        [Theory]
        [InlineData(@"./TestFiles/NPP_comments.xml", 3)]
        [InlineData(@"./TestFiles/NPP_nocomments.xml", 3)]
        public void GivenValidXml_ThenNumberOfChildNodesMatch(string path, int nodeCount)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            NppXmlNode.TryParse(xml, out nppXmlNode);

            nppXmlNode.ChildNodes.Count.Should().Be(nodeCount, because: $"the number of child nodes is {nodeCount}");
        }
    }
}