using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SiteDataTemplates
{
    public class Subpage
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsInNavbar { get; set; } = true;
        public string HtmlContent { get; set; }
    }
}
