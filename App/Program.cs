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
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                if (string.IsNullOrEmpty(args[1]) || !File.Exists(args[1]))
                {
                    Console.WriteLine(@"No path passed or path was incorrect, reopen program and pass path.");
                    return;
                }

                switch (args[0])
                {
                    case "--generate-web":
                        await GenerateSite(args[1]);
                        return;
                    case "--generate-sql":
                        await GenerateSql(args[1]);
                        return;
                    default:
                        await LogHelper.LogTraceAsync("Possible params\n--help\n--generate-sql <Json path>\n--generate-web <Json path>");
                        return;
                }
            }
            catch (Exception ex)
            {
                await LogHelper.LogErrorAsync($"Thrown by method {nameof(Main)}", ex);
            }
        }

        private static async Task GenerateSite(string pathToJson)
        {
            try
            {
                const string resultPath = "Result/Website/";

                using var sr = new StreamReader(pathToJson);
                var template = JsonSerializer.Deserialize<InitialSiteTemplate>(await sr.ReadToEndAsync());
                sr.Close();

                if (!Directory.Exists($"./{resultPath}/Subpages/"))
                {
                    Directory.CreateDirectory($"./{resultPath}/Subpages/");
                }

                foreach (var item in SiteCreator.CreateWebsite(template))
                {
                    var pathInResultFolder = item.IsSubpage ? $"./{resultPath}/Subpages/{item.PathToFile}" : $"./{resultPath}/{item.PathToFile}";
                    await using StreamWriter sw = new(pathInResultFolder);
                    await sw.WriteAsync(item.FileContent);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                await LogHelper.LogErrorAsync($"Thrown by method {nameof(GenerateSite)}", ex);
            }
        }

        private static async Task GenerateSql(string pathToJson)
        {
            try
            {
                const string resultPath = "Result.sql";

                using var sr = new StreamReader(pathToJson);
                var template = JsonSerializer.Deserialize<InitialDBTemplate>(await sr.ReadToEndAsync());
                sr.Close();

                if (!File.Exists($"./{resultPath}"))
                {
                    File.Create($"./{resultPath}").Close();
                }

                await using StreamWriter sw = new($"./{resultPath}");
                await sw.WriteAsync(ScriptCreator.CreateDb(template));
            }
            catch (Exception ex)
            {
                await LogHelper.LogErrorAsync($"Thrown by method {nameof(GenerateSql)}", ex);
            }
        }
    }
}