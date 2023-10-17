using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Persistify.Client.Objects.Generators
{
    public class PersistifyDocumentSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; }

        public PersistifyDocumentSyntaxReceiver()
        {
            CandidateClasses = new List<ClassDeclarationSyntax>();
        }
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
}
