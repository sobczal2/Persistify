using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

var config = ManualConfig.CreateEmpty()
    .AddLogger(ConsoleLogger.Default)
    .AddHardwareCounters()
    .AddJob(Job.ShortRun)
    .WithArtifactsPath("../../../artifacts")
    .AddExporter(HtmlExporter.Default);

foreach (var analyser in DefaultConfig.Instance.GetAnalysers())
{
    config.AddAnalyser(analyser);
}

foreach (var validator in DefaultConfig.Instance.GetValidators())
{
    config.AddValidator(validator);
}

foreach (var columnProvider in DefaultConfig.Instance.GetColumnProviders())
{
    config.AddColumnProvider(columnProvider);
}


BenchmarkRunner.Run(Assembly.GetExecutingAssembly(), config);
