using Data.DBDataTemplates;
using Data.SiteDataTemplates;
using Helpers;
using Service;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlGenerator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                foreach (var argument in args)
                {
                    switch (argument)
                    {
                        case "--generate-web":
                            await GenerateSite();
                            break;
                        case "--generate-sql":
                            await GenerateSql();
                            break;
                        case "--help":
                        default:
                            Console.WriteLine("Possible params\n--help\n--generate-sql\n--generate-web");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Thrown by method {nameof(Main)}", ex);
            }
        }

        private static async Task GenerateSite()
        {
            try
            {
                InitialSiteTemplate templateSql;
                const string resultPath = "Result/Website/";

                Console.WriteLine("Pass the path to template.");
                string path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("No path passed, reopen program and pass path.");
                    return;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    templateSql = JsonSerializer.Deserialize<InitialSiteTemplate>(await sr.ReadToEndAsync());
                    sr.Close();
                }

                if (!Directory.Exists($"./{resultPath}"))
                {
                    Directory.CreateDirectory($"./{resultPath}");
                }

                if (!Directory.Exists($"./{resultPath}/Subpages/"))
                {
                    Directory.CreateDirectory($"./{resultPath}/Subpages/");
                }

                foreach (var item in SiteCreator.CreateWebsite(templateSql))
                {
                    string pathInResultFolder;

                    if (item.IsSubpage)
                    {
                        pathInResultFolder = $"./{resultPath}/Subpages/{item.PathToFile}";
                    }
                    else
                    {
                        pathInResultFolder = $"./{resultPath}/{item.PathToFile}";
                    }

                    using StreamWriter sw = new(pathInResultFolder);
                    await sw.WriteAsync(item.FileContent);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Thrown by method {nameof(GenerateSite)}", ex);
            }
        }

        private static async Task GenerateSql()
        {
            try
            {
                InitialDBTemplate template;
                const string resultPath = "Result.sql";

                Console.WriteLine("Pass the path to template.");
                string path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("No path passed, reopen program and pass path.");
                    return;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    template = JsonSerializer.Deserialize<InitialDBTemplate>(sr.ReadToEnd());
                    sr.Close();
                }

                if (!File.Exists($"./{resultPath}"))
                {
                    File.Create($"./{resultPath}").Close();
                }

                var templateSQL = ScriptCreator.CreateDb(template);

                using (StreamWriter sw = new($"./{resultPath}"))
                {
                    await sw.WriteAsync(templateSQL);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Thrown by method {nameof(GenerateSql)}", ex);
            }
        }
    }
}
