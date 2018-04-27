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
        public void GivenValidXml_ThenFoo(string path)
        {
            var xml = File.ReadAllText(path);

            NppXmlNode nppXmlNode;

            var result = NppXmlNode.TryParse(xml, out nppXmlNode);

            result.Should().BeTrue();
        }
    }
}