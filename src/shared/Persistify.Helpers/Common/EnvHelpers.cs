using System;
using Microsoft.Extensions.Configuration;

namespace Persistify.Helpers.Common;

public static class EnvHelpers
{
    public static void AddNpmPathFromArgs(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.AddCommandLine(args);

        var configuration = builder.Build();

        var npmArgument = configuration["npm"];
        if (!string.IsNullOrEmpty(npmArgument))
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            path += ";" + npmArgument;
            Environment.SetEnvironmentVariable("PATH", path);
        }
    }
}
