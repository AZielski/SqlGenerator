using System;
using System.IO;
using System.Threading.Tasks;

namespace Helpers
{
    /// <summary>
    /// Class used for logging errors and traces. Class and all their functions are static.
    /// </summary>
    public static class LogHelper
    {
        private const string Path = "./LOGFILE";

        /// <summary>
        /// Log a trace to logfile with message.
        /// </summary>
        /// <param name="message">Message to save.</param>
        public static async Task LogTraceAsync(string message)
        {
            try
            {
                await CreateLogAsync($"[TRACE {DateTime.Now}] Message: {message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Log an error to logfile with message and exception.
        /// </summary>
        /// <param name="message">Message to save.</param>
        /// <param name="ex">Exception to save.</param>
        public static async Task LogErrorAsync(string message, Exception ex)
        {
            try
            {
                await CreateLogAsync($"[TRACE {DateTime.Now}] Message: {message} Exception: {ex}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Log an error to logfile with message and exception.
        /// </summary>
        /// <param name="message">Message to save.</param>
        /// <param name="ex">Exception to save.</param>
        public static void LogError(string message, Exception ex)
        {
            try
            {
                Task.Run(() => CreateLogAsync($"[TRACE {DateTime.Now}] Message: {message} Exception: {ex}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Creates log in output file. Output file name: LOGFILE_[DATE_NOW with hour].
        /// </summary>
        /// <param name="toWrite">Text which will be saved.</param>
        private static async Task CreateLogAsync(string toWrite)
        {
            var filePath = $"./Logs/{Path}_{DateTime.Now:MMddyyyyHH}.txt";
            Console.WriteLine(toWrite);

            try
            {
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                await using var sw = new StreamWriter(filePath);
                await sw.WriteAsync(toWrite + "\n");
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}