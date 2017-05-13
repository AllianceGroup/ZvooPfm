using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mPower.Web.Admin.Models
{
    public class ReadModelGenerationModel
    {
        public string ReadConnectionString { get; set; }

        public string WriteConnectionString { get; set; }

        public string SetReadModeUrl { get; set; }

        public string DisableReadModeUrl { get; set; }
        
        public string CopyFromDatabase { get; set; }

        public string CopyToDatabase { get; set; }

        public bool UseInMemoryRegeneration { get; set; }

        public int PatchId { get; set; }

        public List<SelectListItem> Patches { get; set; }
    }
}