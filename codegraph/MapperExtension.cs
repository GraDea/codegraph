using codegraph.common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace codegraph;

public static class MapperExtension
{
    public static QueueModel ToModel(this ClassDeclarationSyntax node, string mcs)
    {
        return new QueueModel()
        {
             Microservice = mcs,
             Fields = node.Members.Where(d => d.IsKind(SyntaxKind.PropertyDeclaration))
                 .OfType<ClassDeclarationSyntax>().ToList().Select(s=>s.Identifier.ToString()).ToArray()
        };
    }
}