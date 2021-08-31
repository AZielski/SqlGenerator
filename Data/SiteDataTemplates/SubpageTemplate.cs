using System.ComponentModel.DataAnnotations;

namespace Data.SiteDataTemplates
{
    public class SubpageTemplate
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsInNavbar { get; set; } = true;
        public string HtmlContent { get; set; }
    }
}
