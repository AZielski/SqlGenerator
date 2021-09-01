using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.SiteDataTemplates;
using Helpers;

namespace Service
{
    public static class SiteCreator
    {
        public static List<ReturnSiteModel> CreateWebsite(InitialSiteTemplate template)
        {
            try
            {
                var navbar = GenerateNavbar(template.Subpages);
                var db = PrepareDatabaseData(template.Database);

                var returnSite = new List<ReturnSiteModel> { PreparePage(navbar, template.WebsiteName, template.HtmlContent, db, true, false) };

                foreach (var subpage in template.Subpages)
                {
                    returnSite.Add(
                      PreparePage(navbar, subpage.Name, subpage.HtmlContent, db)
                    );
                }

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
                LogHelper.LogError($"Error thrown in method {nameof(CreateWebsite)}", ex);
                throw;
            }
        }

        private static ReturnSiteModel PreparePage(string navBar, string websiteName, string htmlContent, string dbReplacement, bool isIndex = false, bool isSubpage = true)
        {
            try
            {
                var phpTemplate = Resources.PHPTemplate;

                if (isIndex)
                {
                    phpTemplate = phpTemplate.Replace("##WEBSITE_TITLE##", websiteName);
                }

                phpTemplate = phpTemplate.Replace("##NAVBAR##", navBar);
                phpTemplate = phpTemplate.Replace("##HTML_CONTENT##", htmlContent);
                phpTemplate = phpTemplate.Replace("##DB##", dbReplacement);

                return new ReturnSiteModel()
                {
                    FileContent = phpTemplate,
                    IsSubpage = isSubpage,
                    PathToFile = isIndex ? "index.php" : $"{ websiteName.Replace(" ", "") }.php"
                };
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Error thrown in method {nameof(PreparePage)}", ex);
                throw;
            }
        }

        private static string PrepareDatabaseData(DatabaseSiteTemplate template)
        {
            try
            {
                var databaseReplace = Resources.ResourceManager.GetString("DBTemplate") ?? "";

                databaseReplace = databaseReplace.Replace("##DATABASE##", template.DbName);
                databaseReplace = databaseReplace.Replace("##USERNAME##", template.ServerLogin);
                databaseReplace = databaseReplace.Replace("##PASSWORD##", template.ServerPassword);
                databaseReplace = databaseReplace.Replace("##SERVER_NAME##", template.ServerName);

                return databaseReplace;
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Error thrown in method {nameof(PrepareDatabaseData)}", ex);
                throw;
            }
        }

        private static string GenerateNavbar(SubpageTemplate[] template)
        {
            try
            {
                var phpTemplate = Resources.ResourceManager.GetString("PHPLink");
                var links = "";

                foreach (var item in template)
                {
                    links += phpTemplate.Replace("##PAGE_NAME_URI##", item.Name.Replace(" ", "")).Replace("##PAGE_NAME##", item.Name);
                }

                Console.WriteLine(links);
                return Resources.ResourceManager.GetString("PHPNavbar").Replace("##LINKS##", links);
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Error thrown in method {nameof(GenerateNavbar)}", ex);
                throw;
            }
        }
    }
}
