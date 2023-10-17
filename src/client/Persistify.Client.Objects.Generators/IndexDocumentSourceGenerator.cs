using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Persistify.Client.Objects.Generators;

[Generator]
public class IndexDocumentSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new PersistifyDocumentSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var source = @"
namespace ConsoleApp1;

public partial class Animal
{
    public void SayHello()
    {
        Console.WriteLine($""Hello, my name is {Name} and I'm {Age} years old."");
    }
}
";
        context.AddSource("Animal.SayHello", source);
    }
}
