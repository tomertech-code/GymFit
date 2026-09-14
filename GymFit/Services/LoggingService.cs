using System.Text;

namespace GymFit.Web.Services
{

    public interface ILoggingService
    {
        Task LogErrorAsync(Exception ex, string additionalInfo = "");
        Task LogInfoAsync(string message);
        Task LogWarningAsync(string message);
        Task<List<LogEntry>> GetRecentLogsAsync(int count = 100);
        Task ClearOldLogsAsync(int daysToKeep = 30);
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? AdditionalInfo { get; set; }
    }

    public class LoggingService : ILoggingService
    {
        private readonly string _logDirectory;
        private readonly string _errorLogPath;
        private readonly string _infoLogPath;
        private static readonly object _lock = new object();

        public LoggingService(IWebHostEnvironment environment)
        {
            _logDirectory = Path.Combine(environment.ContentRootPath, "Logs");
            _errorLogPath = Path.Combine(_logDirectory, "errors.log");
            _infoLogPath = Path.Combine(_logDirectory, "info.log");

            // Create Logs directory if it doesn't exist
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogErrorAsync(Exception ex, string additionalInfo = "")
        {
            var logEntry = new StringBuilder();
            logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR");
            logEntry.AppendLine($"Message: {ex.Message}");
            logEntry.AppendLine($"Type: {ex.GetType().FullName}");

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                logEntry.AppendLine($"Additional Info: {additionalInfo}");
            }

            logEntry.AppendLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                logEntry.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                logEntry.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }

            logEntry.AppendLine(new string('-', 80));

            await WriteToFileAsync(_errorLogPath, logEntry.ToString());
        }

        public async Task LogInfoAsync(string message)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] INFO: {message}{Environment.NewLine}";
            await WriteToFileAsync(_infoLogPath, logEntry);
        }

        public async Task LogWarningAsync(string message)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] WARNING: {message}{Environment.NewLine}";
            await WriteToFileAsync(_errorLogPath, logEntry);
        }

        private async Task WriteToFileAsync(string filePath, string content)
        {
            lock (_lock)
            {
                File.AppendAllText(filePath, content);
            }
            await Task.CompletedTask;
        }

        public async Task<List<LogEntry>> GetRecentLogsAsync(int count = 100)
        {
            var logs = new List<LogEntry>();

            if (File.Exists(_errorLogPath))
            {
                var lines = await File.ReadAllLinesAsync(_errorLogPath);
                var entries = ParseLogEntries(lines);
                logs.AddRange(entries.Take(count));
            }

            return logs.OrderByDescending(l => l.Timestamp).ToList();
        }

        public async Task ClearOldLogsAsync(int daysToKeep = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

            foreach (var logFile in Directory.GetFiles(_logDirectory, "*.log"))
            {
                var fileInfo = new FileInfo(logFile);
                if (fileInfo.LastWriteTimeUtc < cutoffDate)
                {
                    File.Delete(logFile);
                }
            }

            await Task.CompletedTask;
        }

        private List<LogEntry> ParseLogEntries(string[] lines)
        {
            var entries = new List<LogEntry>();
            LogEntry? currentEntry = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("[") && line.Contains("]"))
                {
                    if (currentEntry != null)
                    {
                        entries.Add(currentEntry);
                    }

                    var parts = line.Split(']');
                    if (parts.Length >= 2)
                    {
                        var timestampStr = parts[0].TrimStart('[');
                        var level = parts[1].Trim();

                        currentEntry = new LogEntry
                        {
                            Timestamp = DateTime.Parse(timestampStr),
                            Level = level,
                            Message = parts.Length > 2 ? parts[2].Trim() : ""
                        };
                    }
                }
                else if (currentEntry != null)
                {
                    currentEntry.Message += Environment.NewLine + line;
                }
            }

            if (currentEntry != null)
            {
                entries.Add(currentEntry);
            }

            return entries;
        }
    }
}
