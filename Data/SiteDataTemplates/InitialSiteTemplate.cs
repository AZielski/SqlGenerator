using System.ComponentModel.DataAnnotations;

namespace Data.SiteDataTemplates
{
    public class InitialSiteTemplate
    {
        [Required]
        public string WebsiteName { get; set; }
        public bool HasNavbar { get; set; } = false;
        public bool HasExternalCss { get; set; } = true;
        public string HTMLContent { get; set; }
        public SubpageTemplate[] Subpages { get; set; }
        public DatabaseSiteTemplate Database { get; set; }
    }
}
