using Data;
using Data.DBDataTemplates;
using Data.SiteDataTemplates;
using Service;
using Services;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlGenerator
{
    class Program
    {
        static async Task Main(string[] args)
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
                            Console.WriteLine(Resources.ResourceManager.GetString("ConsoleHelp"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        private static async Task GenerateSite()
        {
            try
            {
                InitialSiteTemplate template;
                const string RESULT_PATH = "Result/PHPSITE/";

                Console.WriteLine("Pass the path to template.");
                string path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("No path passed, reopen program and pass path.");
                    return;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    template = JsonSerializer.Deserialize<InitialSiteTemplate>(sr.ReadToEnd());
                    sr.Close();
                }

                if (!Directory.Exists(RESULT_PATH))
                {
                    Directory.CreateDirectory($"./{RESULT_PATH}");
                }

                var files = SiteCreator.GenerateWebsite(template);

                foreach (var item in files)
                {
                    var filepath = $"./{RESULT_PATH}/{item.PathToFile}";
                    using (StreamWriter sw = new(filepath))
                    {
                        if (!File.Exists(filepath))
                        {
                            File.Create(filepath).Close();
                        }

                        await sw.WriteAsync(item.FileContent);
                    }
                }

                Console.WriteLine($"[INFO] Result is in {Assembly.GetExecutingAssembly().Location}\\{RESULT_PATH}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        private static async Task GenerateSql()
        {
            try
            {
                InitialDBTemplate template;
                const string RESULT_PATH = "Result.sql";

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

                if (!File.Exists($"./{RESULT_PATH}"))
                {
                    File.Create($"./{RESULT_PATH}").Close();
                }

                var templateSQL = ScriptCreator.GenerateDB(template);

                using (StreamWriter sw = new($"./{RESULT_PATH}"))
                {
                    await sw.WriteAsync(templateSQL);
                }

                Console.WriteLine($"[INFO] Result is in {Assembly.GetExecutingAssembly().Location}\\{RESULT_PATH}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }
    }
}
