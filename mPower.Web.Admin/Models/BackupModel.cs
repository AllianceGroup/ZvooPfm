using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mPower.Web.Admin.Models
{
    public class BackupModel
    {
        public string ReadHost { get; set; }
        public string ReadDbName { get; set; }
        public string ReadFolder { get; set; }

        public string WriteHost { get; set; }
        public string WriteDbName { get; set; }
        public string WriteFolder { get; set; }

        public List<string> ReadBackups { get; set; }

        public List<string> WriteBackups { get; set; }
    }
}