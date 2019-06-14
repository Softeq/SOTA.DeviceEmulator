using System;
using Serilog.Events;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Logging
{
    public class LogEventViewModel
    {
        public LogEventViewModel(DateTimeOffset time, LogEventLevel level, string message)
        {
            Time = time;
            Level = level;
            Message = message;
        }

        public DateTimeOffset Time { get; }
        public LogEventLevel Level { get; }
        public string Message { get; }
    }
}