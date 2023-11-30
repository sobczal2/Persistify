using System.CommandLine;
using Persistify.Services;
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Meta;

namespace Persistify.Tools.Commands;

public class GenerateProtosCommand : Command
{
    private readonly Argument<string> _outputPathArgument =
        new("output-path", "Output path for generated .proto files");

    public GenerateProtosCommand()
        : base("generate-protos", "Generate .proto files for persistify services")
    {
        AddArgument(_outputPathArgument);

        _ = _outputPathArgument.AddCompletions(ctx =>
        {
            var path = ctx.ParseResult.GetValueForArgument(_outputPathArgument);

            return string.IsNullOrWhiteSpace(path)
                ? new List<string>()
                : Directory.GetDirectories(path);
        });

        this.SetHandler(Handle, _outputPathArgument);
    }

    private void Handle(
        string path
    )
    {
        var generator = new SchemaGenerator { ProtoSyntax = ProtoSyntax.Proto3 };

        File.WriteAllText(
            Path.Combine(path, "templates.proto"),
            generator.GetSchema<ITemplateService>()
        );
        File.WriteAllText(
            Path.Combine(path, "documents.proto"),
            generator.GetSchema<IDocumentService>()
        );
        File.WriteAllText(Path.Combine(path, "users.proto"), generator.GetSchema<IUserService>());
        File.WriteAllText(
            Path.Combine(path, "presetAnalyzers.proto"),
            generator.GetSchema<IPresetAnalyzerService>()
        );
    }
}
