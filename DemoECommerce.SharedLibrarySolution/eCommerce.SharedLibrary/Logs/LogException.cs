using Serilog;

namespace eCommerce.SharedLibrary.Logs
{
    public class LogException
    {

        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebug(ex.Message);
        }

        private static void LogToFile(string message)=>Log.Information(message);
        private static void LogToConsole(string message)=>Log.Warning(message);
        private static void LogToDebug(string message)=>Log.Debug(message);

    }
}
