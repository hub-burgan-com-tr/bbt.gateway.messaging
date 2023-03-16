using Serilog;

namespace bbt.gateway.common.Helpers
{
    public class LogManager
    {
        private readonly Serilog.ILogger _logger;
        public LogManager()
        { 
            _logger = Log.ForContext<LogManager>();
        }

        public void LogCritical(string LogMessage)
        {
            _logger.Fatal(LogMessage);
        }

        public void LogError(string LogMessage)
        {
            _logger.Error(LogMessage);
        }

        public void LogDebug(string LogMessage)
        {
            _logger.Debug(LogMessage);
        }

        public void LogTrace(string LogMessage)
        {
            _logger.Verbose(LogMessage);
        }

        public void LogInformation(string LogMessage)
        {
            _logger.Information(LogMessage);
        }

        public void LogWarning(string LogMessage)
        {
            _logger.Warning(LogMessage);
        }
    }
}
