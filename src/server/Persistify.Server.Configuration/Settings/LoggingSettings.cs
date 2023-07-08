using System;
using Serilog.Events;

namespace Persistify.Server.Configuration.Settings;

public class LoggingSettings
{
    public const string SectionName = "Logging";
    public LogEventLevel Default { get; set; } = LogEventLevel.Information;
    public string? Seq { get; set; }
}
