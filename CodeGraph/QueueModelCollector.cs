using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGraph;

public class QueueModelCollector : CSharpSyntaxWalker
{
    // <Snippet4>
    public ICollection<ClassDeclarationSyntax> classes { get; } = new List<ClassDeclarationSyntax>();
    // </Snippet4>

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        base.VisitClassDeclaration(node);
        var queue = node.BaseList?.Types.FirstOrDefault(x =>
            "QueueReportingMessage" == (x.Type as QualifiedNameSyntax)?.Right.ToString());
        if (queue != null)
        {
            classes.Add(node);
        }
    }

}