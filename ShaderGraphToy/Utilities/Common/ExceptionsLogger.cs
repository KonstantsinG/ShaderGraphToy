using ShaderGraphToy.Resources;
using System.IO;
using System.Text;

namespace ShaderGraphToy.Utilities.Common
{
    public static class ExceptionsLogger
    {
        private static readonly string _logName;
        private static readonly string _logPath;

        static ExceptionsLogger()
        {
            _logName = $"logs_{DateTime.Now:yyyy-MM-dd}.log";
            _logPath = ResourceManager.LogsPath;

            if (!Directory.Exists(_logPath)) Directory.CreateDirectory(_logPath);
            _logPath = Path.Combine(_logPath, _logName);
        }

        public static void LogError(Exception ex, string sender)
        {
            StringBuilder sb = new();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}");
            sb.AppendLine($"SOURCE: {ex.Source}, DESC: {sender}");
            sb.AppendLine($"STACK_TRACE: {ex.StackTrace}{Environment.NewLine}");

            File.AppendAllText(_logPath, sb.ToString(), Encoding.UTF8);
        }
    }
}
