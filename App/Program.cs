using Data.DBDataTemplates;
using Service;
using Services;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlGenerator
{
    class Program
    {
        const string RESULT_PATH = "Result.sql";

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
                            Console.WriteLine("Possible params\n--help\n--generate-sql\n--generate-web");
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
                var result = SiteGenerator.GenerateWebsite();
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
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }
    }
}
