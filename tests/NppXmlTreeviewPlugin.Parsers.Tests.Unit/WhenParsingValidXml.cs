using System.IO;

using Xunit;

namespace NppXmlTreeviewPlugin.Parsers.Tests.Unit
{
    public class WhenParsingValidXml
    {
        [Theory]
        [InlineData(@"./NPP_comments.xml")]
        [InlineData(@"./NPP_nocomments.xml")]
        public void GivenValidXml_ThenFoo(string path)
        {
            var xml = File.ReadAllText(path);

            var result = NppXmlNode.TryParse(xml, out var nppXmlNode);

            result.Should().BeTrue();
        }
    }
}