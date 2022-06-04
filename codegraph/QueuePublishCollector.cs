using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace codegraph;

public class QueuePublishCollector : CSharpSyntaxWalker
{
    // <Snippet4>
    public ICollection<InvocationExpressionSyntax> Invocations { get; } = new List<InvocationExpressionSyntax>();
    // </Snippet4>


    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        base.VisitInvocationExpression(node);

        if (node.ToString().Contains("Publish"))
        {
            node.ToString();
        }
    }
}