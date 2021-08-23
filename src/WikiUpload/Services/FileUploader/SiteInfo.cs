using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace WikiUpload
{
    internal class SiteInfo : ISiteInfo
    {
        private readonly HashSet<string> _languages;

        public string BaseUrl { get; private set; }

        public string ScriptPath { get; private set; }

        public string ArticlePath { get; private set; }

        public string ServerUrl { get; private set; }

        public string FileNamespace { get; }
        
        public string CategoryNamespace { get; }

        public List<string> Extensions { get; }

        public Version MediaWikiVersion { get; private set; }

        public WikiCasing WikiCasing { get; private set; }

        public SiteInfo(XmlDocument doc)
        {
            ParseGeneralElement(doc);

            FileNamespace = ExtractFileNamespace(doc);
            CategoryNamespace = ExtractCategoryNamespace(doc);
            Extensions = ParseFileExtensions(doc.SelectNodes("/api/query/fileextensions/fe"));
            _languages = ParseLanguages(doc.SelectNodes("/api/query/languages/lang"));
        }

        private void ParseGeneralElement(XmlDocument doc)
        {
            var generalAttributes = doc.SelectSingleNode("/api/query/general").Attributes;
            BaseUrl = generalAttributes["base"].Value;
            ScriptPath = generalAttributes["scriptpath"].Value;
            ArticlePath = generalAttributes["articlepath"].Value;
            ServerUrl = generalAttributes["server"].Value;
            MediaWikiVersion = ParseVersion(generalAttributes["generator"]?.Value);
            WikiCasing = ParseWikiCasing(generalAttributes["case"].Value);
        }

        private WikiCasing ParseWikiCasing(string caseValue)
        {
            switch (caseValue)
            {
                case "first-letter":
                    return WikiCasing.FirstLetter;
                case "case-sensitive":
                    return WikiCasing.CaseSensitive;
                default:
                    return WikiCasing.Unknown;
            }
        }

        private string ExtractFileNamespace(XmlDocument doc)
        {
            var ns = doc.SelectSingleNode("/api/query/namespaces/ns[@_idx=\"6\"]");
            return ns == null ? "File:" : ns.InnerText + ":";
        }

        private string ExtractCategoryNamespace(XmlDocument doc)
        {
            var ns = doc.SelectSingleNode("/api/query/namespaces/ns[@_idx=\"14\"]");
            return ns == null ? "Category:" : ns.InnerText + ":";
        }

        public bool IsSupportedLanguage(string langCode) => _languages.Contains(langCode);

        private static List<string> ParseFileExtensions(XmlNodeList extNodes)
        {
            var extensions = new List<string>();
            foreach (XmlNode fe in extNodes)
                extensions.Add(fe.Attributes["ext"].Value);
            return extensions;
        }

        private static HashSet<string> ParseLanguages(XmlNodeList langNodes)
        {
            var languages = new HashSet<string>();
            foreach (XmlNode node in langNodes)
                languages.Add(node.Attributes["code"].Value);
            return languages; ;
        }

        private static Version ParseVersion(string generator)
        {
            if (generator == null)
                return DefaultVersion();

            var match = Regex.Match(generator, @"^MediaWiki\s+(\d+\.\d+\.\d+)");

            if (!match.Success)
                return DefaultVersion();

            return new Version(match.Groups[1].Value + ".0");
        }

        private static Version DefaultVersion() => new Version("0.0.0.0");
    }
}
