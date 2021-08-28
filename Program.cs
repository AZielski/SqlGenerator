using SqlGenerator.DataTemplates;
using SqlGenerator.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlGenerator
{
    class Program
    {
        const string RESULT_PATH = "Result.sql";

        static async Task Main()
        {
            InitialDBTemplate template;

            Console.WriteLine("[INFO] Pass the path to template");
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

            using (StreamWriter sw = new($"./{RESULT_PATH}"))
            {
                await sw.WriteAsync(ScriptCreator.GenerateDB(template));
            }

            Console.WriteLine($"[INFO] Result is in {Assembly.GetExecutingAssembly().Location}\\{RESULT_PATH}");
            
            Console.ReadLine();
        }
    }
}
