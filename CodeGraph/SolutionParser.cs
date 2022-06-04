using System.Text.RegularExpressions;
using CodeGraph.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeGraph;

public class SolutionParser
{
    private readonly Repository _repository;

    public SolutionParser(Repository repository)
    {
        _repository = repository;
    }

    public List<QueueModel> GetQueueDependencies()
    {
        var consumers = new List<QueueModel>();

        var workspace = MSBuildWorkspace.Create();

        // open solution we want to analyze
        var solutionToAnalyze =
            workspace.OpenSolutionAsync(_repository.SolutionPath).Result;

        foreach (var project in solutionToAnalyze.Projects.Where(x=>x.Name != "Tests"))
        {
            var compilation = project.GetCompilationAsync().Result;

            foreach (var document in project.Documents)
            {
                var tree = document.GetSyntaxTreeAsync().Result;
                var root = tree.GetRoot();
                var model = compilation.GetSemanticModel(tree);
            
            
                // var invocationSyntaxes = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
                //
                // var classesToPublish = new List<string>();
                // foreach (var invocationSyntax in invocationSyntaxes)
                // {
                //     if (IsQueuePublishInvoke(model, invocationSyntax))
                //     {
                //         classesToPublish.Add((model.GetSymbolInfo(invocationSyntax.ArgumentList.Arguments[0].Expression).Symbol as IFieldSymbol).Type.ToString());
                //     }
                // }
                //
                // classesToPublish = classesToPublish.Distinct().ToList();
            
            
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            
                foreach (var syntax in classes)
                {
                    if (IsConsumerQueueModel(syntax)
                       ) //|| classesToPublish.Contains(syntax.ToString())
                    {
                        if(TryGetQueueModelFromClass(model, syntax, out var queueModel))
                        {
                            consumers.Add(queueModel);
                        }
                    }
                }
            }
        }

        

        return consumers;

    }

    bool TryGetQueueModelFromClass(SemanticModel semanticModel, ClassDeclarationSyntax syntax, out QueueModel queueModel)
    {
        var constructorDeclarationSyntax =
            syntax.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
        if (constructorDeclarationSyntax != null)
        {
            var arguments = constructorDeclarationSyntax.Initializer.ArgumentList.Arguments;
            
            if (arguments.Count == 3)
            {
                var exchangeName = (semanticModel.GetSymbolInfo(arguments[1].Expression).Symbol as IFieldSymbol)
                    .ConstantValue.ToString();
                var routingKey = arguments[2].Expression.ToString();
                var fields = syntax.Members.OfType<PropertyDeclarationSyntax>()
                    .Select(x => x.Identifier.Value.ToString());

                queueModel = new QueueModel(QueueMemberType.Consumer)
                {
                    ExchangeName = exchangeName,
                    RoutingKey = routingKey,
                    Fields = fields.ToArray(),
                    Microservice = _repository.ApplicationName
                };

                return true;
            }
        }

        queueModel = null;
        return false;
    }
    
    bool IsConsumerQueueModel(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.BaseList?.Types.FirstOrDefault(x =>
            "IConsumedQueueMessage" == GetBaseTypeName(x)) != null;
    }

    bool IsQueuePublishInvoke(SemanticModel model, InvocationExpressionSyntax syntax)
    {
        return syntax.ToString().Contains("Publish") && "Cian.Queue.Services.IQueueService" == (model.GetSymbolInfo(
            ((Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax) syntax
                .Expression).Expression).Symbol as IFieldSymbol).Type.ToString();
    }

    string GetBaseTypeName(BaseTypeSyntax baseTypeSyntax)
    {
        if (baseTypeSyntax.Type is QualifiedNameSyntax)
        {
            return (baseTypeSyntax.Type as QualifiedNameSyntax).Right.ToString();
        }
        else if (baseTypeSyntax.Type is IdentifierNameSyntax)
        {
            return baseTypeSyntax.Type.ToString();
        }

        return String.Empty;
    }
}