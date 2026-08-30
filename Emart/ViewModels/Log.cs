namespace EMart.ViewModels
{
    public class Log
    {
        public static void WriteLog(Exception ex)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, $"Log_{DateTime.Now:ddMMyyyy}.txt");

                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine("---------------------------------------------------");
                    sw.WriteLine($"Date       : {DateTime.Now}");
                    sw.WriteLine($"Machine    : {Environment.MachineName}");
                    sw.WriteLine($"User       : {Environment.UserName}");
                    sw.WriteLine($"Message    : {ex.Message}");
                    sw.WriteLine($"Source     : {ex.Source}");
                    sw.WriteLine($"Method     : {ex.TargetSite?.Name}");
                    sw.WriteLine($"StackTrace : {ex.StackTrace}");
                    sw.WriteLine("---------------------------------------------------");
                    sw.WriteLine();
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine(logEx.Message);
                Log.WriteLog(ex);
                Log.WriteLog(logEx);
            }
        }
    }
}
