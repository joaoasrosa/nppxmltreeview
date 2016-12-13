//using System.Diagnostics;
//using System.IO;
//using System.Xml;
//using System.Xml.Linq;
//using NppXmlTreeviewPlugin.Parsers;
//using Xunit;

//namespace NppXmlTreeviewPlugin.Tests
//{
//    public class NppXmlParserTests
//    {
//        // TODO the tests need to be better
//        [Fact]
//        public void ValidXml()
//        {

//            NppXmlNode node;
//            NppXmlNode.TryParse("<xml1><x1><z/>dsadada</x1><x/></xml1>", out node);
//            //NppXmlNode.TryParse("<xml1><x1><z/>dsadada</x1><x/><joao></joao></xml1>", out node);
//            //NppXmlNode.TryParse(File.ReadAllText(@"c:\temp\test.xml"), out node);
//            Assert.Equal(2, node.ChildNodes.Count);
//        }
//    }
//}
