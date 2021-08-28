using SqlGenerator.DataTemplates;
using SqlGenerator.Services;
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

        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

        [STAThread]
        static async Task Main()
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
            
            //Copying MySQL script to the clippoard
            OpenClipboard(IntPtr.Zero);
            var ptr = Marshal.StringToHGlobalUni(templateSQL);
            SetClipboardData(13, ptr);
            CloseClipboard();
            Marshal.FreeHGlobal(ptr);

            Console.WriteLine($"[INFO] MySQL copied to your clipboard.");
            Console.WriteLine($"[INFO] Result is in {Assembly.GetExecutingAssembly().Location}\\{RESULT_PATH}");
            Console.ReadLine();
        }
    }
}
