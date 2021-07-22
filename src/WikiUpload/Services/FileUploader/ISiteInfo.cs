using System;
using System.Collections.Generic;

namespace WikiUpload
{
    internal interface ISiteInfo
    {
        string ArticlePath { get; }
        string BaseUrl { get; }
        List<string> Extensions { get; }
        string FileNamespace { get; }
        Version MediaWikiVersion { get; }
        string ScriptPath { get; }
        string ServerUrl { get; }
        WikiCasing WikiCasing { get; }

        bool IsSupportedLanguage(string langCode);
    }
}