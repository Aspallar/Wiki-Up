using CommandLine;

namespace TestServer
{
    internal class Options
    {
        [Option(Default = 0)]
        public int Exists { get; set; }

        [Option(Default = 0)]
        public int Delay { get; set; }

        [Option(Default = 0)]
        public int InvalidXml { get; set; }

        [Option]
        public bool InvalidLoginXml { get; set; }

        [Option]
        public bool LoginTimeout { get; set; }

        [Option]
        public bool UploadTimeout { get; set; }

        [Option(Default = 0, HelpText = "-1 to always maxlag")]
        public int MaxLag { get; set; }

        [Option]
        public bool ShowReply { get; set; }


    }
}
