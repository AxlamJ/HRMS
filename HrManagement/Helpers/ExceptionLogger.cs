using System;
using System.IO;
using Newtonsoft.Json;

namespace HrManagement.Helpers
{

    public class ExceptionLogger
    {
        private static readonly string LogDirectory = "logs"; // Directory for storing logs

        public static void LogException(Exception ex)
        {
            try
            {
                // Ensure log directory exists
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                // Generate a unique log file name using DateTime format "ddMMyyyyHHmmssfff"
                string fileName = $"{DateTime.Now:ddMMyyyyHHmmssfff}.json";
                string filePath = Path.Combine(LogDirectory, fileName);

                // Create log entry
                var logEntry = new
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    InnerException = ex.InnerException?.Message
                };

                // Write log entry to a new JSON file
                File.WriteAllText(filePath, JsonConvert.SerializeObject(logEntry, Formatting.Indented));

                Console.WriteLine($"Exception logged in: {filePath}");
            }
            catch (Exception loggingEx)
            {
                Console.WriteLine("Failed to log exception: " + loggingEx.Message);
            }
        }
    }

}
