using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace WikiUpload
{
    internal class SiteInfo
    {
        private readonly HashSet<string> _languages;

        public string BaseUrl { get; }

        public string ScriptPath { get; }

        public List<string> Extensions { get; }

        public Version MediaWikiVersion { get; }

        public SiteInfo(XmlDocument doc)
        {
            var general = doc.SelectSingleNode("/api/query/general");
            BaseUrl = general.Attributes["base"].Value;
            ScriptPath = general.Attributes["scriptpath"].Value;
            MediaWikiVersion = ParseVersion(general.Attributes["generator"]?.Value);

            Extensions = ParseFileExtensions(doc.SelectNodes("/api/query/fileextensions/fe"));
            _languages = ParseLanguages(doc.SelectNodes("/api/query/languages/lang"));
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
                return new Version("0.0.0.0");

            var match = Regex.Match(generator, @"^MediaWiki\s+(\d+\.\d+\.\d+)");

            if (!match.Success)
                return new Version("0.0.0.0");

            return new Version(match.Groups[1].Value + ".0");
        }
    }
}
