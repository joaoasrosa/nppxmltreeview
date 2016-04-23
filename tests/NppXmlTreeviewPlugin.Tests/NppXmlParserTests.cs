using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NppXmlTreeviewPlugin.Parsers;
using Xunit;

namespace NppXmlTreeviewPlugin.Tests
{
    public class NppXmlParserTests
    {
        // TODO the tests need to be better
        [Fact]
        public void ValidXml()
        {
            //var node = NppXmlParser.Parse("<xml1 das= ><x dasdsa /></xml1>");
            //Assert.Equal(1, node.ChildNodes.Count);

            //node = NppXmlParser.Parse("<xml1 das= >AAA<x dasdsa /></xml1>");
            //Assert.Equal(1, node.ChildNodes.Count);

            //var xml = File.ReadAllText(@"C:\Users\pmni_\Desktop\miradouro virtual.txt");
            //var node1 = NppXmlParser.Parse(xml);

            //var node = NppXmlParser.Parse("<?xml><xml1><x1      dsad ><z/>dsadada</x1><x/></xml1>");
            //Assert.Equal(2, node.ChildNodes.Count);


            XDocument xml = XDocument.Load(@"c:\temp\test.xml", LoadOptions.SetLineInfo);


            foreach (var descendantNode in xml.DescendantNodes())
            {
                var lineInfo = (IXmlLineInfo) descendantNode;
                Debug.WriteLine(lineInfo.LinePosition);
                Debug.WriteLine(lineInfo.LineNumber);
            }


            NppXmlNode node;
            //NppXmlNode.TryParse("<xml1><x1><z/>dsadada</x1><x/></xml1>", out node);
            //NppXmlNode.TryParse("<xml1><x1><z/>dsadada</x1><x/><joao></joao></xml1>", out node);
            NppXmlNode.TryParse(File.ReadAllText(@"c:\temp\test.xml"), out node);
            Assert.Equal(2, node.ChildNodes.Count);
        }
    }
}
