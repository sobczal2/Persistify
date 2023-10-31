using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistify.Tools.Commands;

public class GenerateImageCommand : Command
{
    private readonly Argument<string> _persistifyProjectRootArgument =
        new("persistify-project-root", "Path to the root of the persistify project");

    public GenerateImageCommand()
        : base("generate-image", "Generate a docker image for persistify")
    {
        AddArgument(_persistifyProjectRootArgument);
        _ = _persistifyProjectRootArgument.AddCompletions(ctx =>
        {
            var path = ctx.ParseResult.GetValueForArgument(_persistifyProjectRootArgument);
            return string.IsNullOrWhiteSpace(path)
                ? new List<string>()
                : Directory.GetDirectories(path);
        });
        this.SetHandler(Handle, _persistifyProjectRootArgument);
    }

    private async Task Handle(string path)
    {
        var persistifySlnPath = Path.Combine(path, "Persistify.sln");

        if (!File.Exists(persistifySlnPath))
        {
            Console.WriteLine($"Could not find {persistifySlnPath}");
            return;
        }

        var persistifyDockerFilePath = Path.Combine(path, "src", "server", "Persistify.Server", "Dockerfile");

        if (!File.Exists(persistifyDockerFilePath))
        {
            Console.WriteLine($"Could not find {persistifyDockerFilePath}");
            return;
        }

        var getPersistifyVersionProcess = Process.Start(
            new ProcessStartInfo("nbgv", "get-version -f json")
            {
                WorkingDirectory = path,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        );

        if (getPersistifyVersionProcess is null)
        {
            Console.WriteLine("Could not start nbgv get-version");
            return;
        }

        var stdout = await getPersistifyVersionProcess.StandardOutput.ReadToEndAsync();
        var version = JsonSerializer.Deserialize<NbgvVersion>(stdout);

        if (version is null)
        {
            Console.WriteLine("Could not parse nbgv get-version output");
            return;
        }

        var persistifyVersion = version.Version;

        var persistifyImageName = $"persistify:{persistifyVersion}";

        var buildPersistifyImageProcess = Process.Start(
            new ProcessStartInfo("docker", $"build -t {persistifyImageName} .")
            {
                WorkingDirectory = Path.GetDirectoryName(persistifyDockerFilePath),
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        );

        if (buildPersistifyImageProcess is null)
        {
            Console.WriteLine("Could not start docker build");
            return;
        }

        await buildPersistifyImageProcess.WaitForExitAsync();

        if (buildPersistifyImageProcess.ExitCode != 0)
        {
            Console.WriteLine("Docker build failed");
            return;
        }
    }

    private class NbgvVersion
    {
        [JsonPropertyName("SimpleVersion")]
        public string Version { get; set; } = string.Empty;
    }
}
