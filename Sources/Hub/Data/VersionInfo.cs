using System.Collections.Generic;

namespace Hub
{
    public class VersionInfo
    {
        public List<string> Info { get; set; }

        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }

        public int Build { get; set; }

        public string DownloadUrl { get; set; }

        public override string ToString() => $"v{MajorVersion}.{MinorVersion}.b{Build}";
    }
}
