using System;
using System.Collections.Generic;

namespace Hub
{
    public class LocalEditors
    {
        public List<LocalEditor> Editors { get; set; }
    }

    public class LocalEditor
    {
        public VersionInfo VersionList { get; set; }
        public string path;
    }
}
