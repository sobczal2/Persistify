using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Persistify.Client.Objects.Generators;

public class PersistifyDocumentSyntaxReceiver : ISyntaxReceiver
{
    public PersistifyDocumentSyntaxReceiver()
    {
        CandidateClasses = new List<ClassDeclarationSyntax>();
    }

    public List<ClassDeclarationSyntax> CandidateClasses { get; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList is not null)
            {
                CandidateClasses.Add(classDeclarationSyntax);
            }
        }
    }
}
