using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WikiUpload;   

namespace Tests.MiscellaneousTests
{
    [TestFixture]
    public class SiteInfoTests
    {

        [Test]
        public void When_FullReply_Then_ParsedCorrectly()
        {
            var expectedExtensions = new List<string>
            {
                "png",
                "gif",
                "jpg",
                "jpeg",
                "webp",
                "ico",
                "pdf",
                "svg",
                "odm",
                "ogg",
                "ogv",
                "oga",
                "flac",
                "opus",
                "wav",
                "webm",
                "mp3",
            };
            var doc = CreateXmlDocument(QueryReply(SiteInfoTestReplies.FullReply));

            var result = new SiteInfo(doc);

            Assert.That(result.BaseUrl, Is.EqualTo("https://aspallar.fandom.com/wiki/Aspallar_Wiki"));
            Assert.That(result.ScriptPath, Is.EqualTo(""));
            Assert.That(result.ArticlePath, Is.EqualTo("/wiki/$1"));
            Assert.That(result.ServerUrl, Is.EqualTo("https://aspallar.fandom.com"));
            Assert.That(result.FileNamespace, Is.EqualTo("Foobar:"));
            Assert.That(result.MediaWikiVersion.ToString(), Is.EqualTo("1.33.3.0"));
            Assert.That(result.Extensions, Is.EquivalentTo(expectedExtensions));
            Assert.That(result.WikiCasing, Is.EqualTo(WikiCasing.FirstLetter));

            Assert.That(result.IsSupportedLanguage("en"), Is.True);
            Assert.That(result.IsSupportedLanguage("aa"), Is.True);
            Assert.That(result.IsSupportedLanguage("zu"), Is.True);
            Assert.That(result.IsSupportedLanguage("zz"), Is.False);
        }

        [Test]
        public void When_MissingFileNamespace_Then_DefaulyFileNamespaceUsed()
        {
            var noFileNamespace = SiteInfoTestReplies.FullReply.Replace(
                @"<ns _idx=""6"" canonical=""File"" case=""first-letter"" id=""6"" subpages="""" xml:space=""preserve"">Foobar</ns>",
                "");
            var doc = CreateXmlDocument(QueryReply(noFileNamespace));

            var result = new SiteInfo(doc);

            Assert.That(result.FileNamespace, Is.EqualTo("File:"));
        }

        [Test]
        public void When_VesionCannotBeParsed_Then_VersaionIsZero()
        {
            var badVersion = SiteInfoTestReplies.FullReply.Replace(
                @"generator=""MediaWiki 1.33.3""",
                @"generator=""Foobar""");
            var doc = CreateXmlDocument(QueryReply(badVersion));

            var result = new SiteInfo(doc);

            Assert.That(result.MediaWikiVersion.ToString(), Is.EqualTo("0.0.0.0"));
        }

        private static XmlDocument CreateXmlDocument(string xmlText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlText);
            return doc;
        }

        private static string ApiReply(string content)
            => $"<?xml version=\"1.0\"?><api>{content}</api>";

        private static string QueryReply(string content)
            => ApiReply($"<query>{content}</query>");

    }
}
