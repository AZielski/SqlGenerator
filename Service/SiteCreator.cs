using System;
using System.Collections.Generic;
using Data;
using Data.SiteDataTemplates;

namespace Service
{
    public static class SiteCreator
    {
        public static List<ReturnSiteModel> GenerateWebsite(InitialSiteTemplate template)
        {
            try
            {
                var returnSite = new List<ReturnSiteModel>();
                var navbar = GenerateNavbar(template.Subpages);

                returnSite.Add(PrepareIndex(template));

                if (template.HasExternalCss)
                {
                    returnSite.Add(new ReturnSiteModel()
                    {
                        FileContent = Resources.CSSTemplate,
                        PathToFile = "styles.css"
                    });
                }

                return returnSite;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        private static ReturnSiteModel PrepareIndex(InitialSiteTemplate template)
        {
            try
            {
                var phpTemplate = Resources.PHPTemplate;
                var returnData = new ReturnSiteModel();

                phpTemplate = phpTemplate.Replace("##NAVBAR##", GenerateNavbar(template.Subpages));
                phpTemplate = phpTemplate.Replace("##WEBSITE_TITLE##", template.WebsiteName);
                phpTemplate = phpTemplate.Replace("##HTML_CONTENT##", template.HTMLContent);
                phpTemplate = phpTemplate.Replace("##DB##", PrepareDatabaseData(template.Database));

                returnData.PathToFile = "index.php";
                returnData.FileContent = phpTemplate;

                return returnData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        private static string PrepareDatabaseData(DatabaseSiteTemplate template)
        {
            try
            {
                var databaseReplace = Resources.ResourceManager.GetString("DBTemplate");

                databaseReplace = databaseReplace.Replace("##DATABASE##", template.DBName);
                databaseReplace = databaseReplace.Replace("##USERNAME##", template.ServerLogin);
                databaseReplace = databaseReplace.Replace("##PASSWORD##", template.ServerPassword);
                databaseReplace = databaseReplace.Replace("##SERVER_NAME##", template.ServerName);

                return databaseReplace;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        private static string GenerateNavbar(SubpageTemplate[] template)
        {
            try
            {
                if (template == null)
                {
                    return "";
                }

                var navbar = Resources.NavbarTemplate;
                string linksToSubpages = "";

                foreach (var item in template)
                {
                    linksToSubpages += Resources.ResourceManager.GetString("PHPLink").Replace("##PAGE_NAME_URI##", item.Name.Replace(" ", "")).Replace("##PAGE_NAME##", item.Name);
                }

                navbar = navbar.Replace("##LINKS##", linksToSubpages);
                return navbar;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }
    }
}
