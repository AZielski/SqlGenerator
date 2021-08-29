using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SiteDataTemplates
{
    public class InitialSite
    {
        [Required]
        public string WebsiteName { get; set; }
        public bool HasNavbar { get; set; } = false;
        public bool HasDB { get; set; } = false;
        public bool HasExternalCss { get; set; } = true;
        public Subpage[] Subpages { get; set; }
    }
}
