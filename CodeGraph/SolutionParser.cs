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

        using var workspace = MSBuildWorkspace.Create();

        // open solution we want to analyze
        Solution solutionToAnalyze =
            workspace.OpenSolutionAsync(_repository.SolutionPath).Result;

        // Project project = solutionToAnalyze.Projects
        //     .FirstOrDefault(proj => proj.Name == "Common");

        var classesToPublish = new List<string>();
        foreach (var project in solutionToAnalyze.Projects.Where(x=>x.Name != "Tests"))
        {
            Compilation compilation = project.GetCompilationAsync().Result;

            foreach (var document in project.Documents)
            {
                var tree = document.GetSyntaxTreeAsync().Result;
                var root = tree.GetRoot();
                var model = compilation.GetSemanticModel(tree);
            
            
                var invocationSyntaxes = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
                
                foreach (var invocationSyntax in invocationSyntaxes)
                {
                    if (IsQueuePublishInvoke(model, invocationSyntax))
                    {
                        classesToPublish.Add(GetFullTypeNameFromExpression(model,
                            invocationSyntax.ArgumentList.Arguments[0].Expression));
                    }
                }
                
                classesToPublish = classesToPublish.Distinct().ToList();
            
            
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            
                foreach (var syntax in classes)
                {
                    var fullTypeName =
                        (model.GetDeclaredSymbol(syntax) as INamedTypeSymbol).ConstructedFrom.ToString();
                    
                    if (IsConsumerQueueModel(syntax))
                    {
                        if(TryGetQueueModelFromClass(model, syntax, QueueMemberType.Consumer, out var queueModel))
                        {
                            consumers.Add(queueModel);
                        }
                    }
                    
                    if(classesToPublish.Contains(fullTypeName))
                    {
                        if(TryGetQueueModelFromClass(model, syntax, QueueMemberType.Publisher, out var queueModel))
                        {
                            consumers.Add(queueModel);
                        }
                    }
                }
            }
        }

        

        return consumers;

    }

    bool TryGetQueueModelFromClass(SemanticModel semanticModel, ClassDeclarationSyntax syntax, QueueMemberType memberType, out QueueModel queueModel)
    {
        queueModel = null;
        if (semanticModel.GetDeclaredSymbol(syntax)?.IsAbstract ?? false) // TODO: Добавить поддержку множественного наследования и абтрактных моделей
        {
            return false;
        }
        
        var constructorDeclarationSyntax =
            syntax.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
        
        if (constructorDeclarationSyntax != null)
        {
            var arguments = constructorDeclarationSyntax.Initializer.ArgumentList.Arguments;
            
            if (arguments.Count == 3)
            {
                var exchangeName = GetValueFromExpressionSyntax(semanticModel, arguments[1].Expression);
                var routingKey = GetValueFromExpressionSyntax(semanticModel, arguments[2].Expression);
                var fields = syntax.Members.OfType<PropertyDeclarationSyntax>()
                    .Select(x => x.Identifier.Value.ToString());

                queueModel = new QueueModel(memberType)
                {
                    ExchangeName = exchangeName,
                    RoutingKey = routingKey,
                    Fields = fields.ToArray(),
                    Microservice = _repository.ApplicationName
                };

                return true;
            }
        }

        return false;
    }

    string GetFullTypeNameFromExpression(SemanticModel semanticModel, ExpressionSyntax expressionSyntax)
    {
        if (expressionSyntax is IdentifierNameSyntax)
        {
            var symbol = semanticModel.GetSymbolInfo(expressionSyntax).Symbol;
            
            return ((symbol as ILocalSymbol)?.Type ?? (symbol as IParameterSymbol)?.Type) //TODO: IParameterSymbol - добавить поддержку generic methods
                .ToString();
        }
        if (expressionSyntax is ObjectCreationExpressionSyntax)
        {
            return semanticModel.GetTypeInfo(expressionSyntax).Type.ToString();
        }

        throw new NotSupportedException($"Не поддерживаемый expression {expressionSyntax.GetType()}");
    }

    string GetValueFromExpressionSyntax(SemanticModel semanticModel, ExpressionSyntax expression)
    {
        if (expression is LiteralExpressionSyntax)
        {
            return expression.ToString().Replace(@"""", "");
        }
        else if (expression is MemberAccessExpressionSyntax)
        {
            return (semanticModel.GetSymbolInfo(expression).Symbol as IFieldSymbol).ConstantValue.ToString();
        }
        else if(expression is IdentifierNameSyntax)
        {
            return (semanticModel.GetSymbolInfo(expression).Symbol as IFieldSymbol).ConstantValue.ToString();
        }

        throw new NotSupportedException($"Не поддерживаемый тип expression {expression.GetType().ToString()}");
    }
    
    bool IsConsumerQueueModel(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.BaseList?.Types.FirstOrDefault(x =>
            "IConsumedQueueMessage" == GetBaseTypeName(x)) != null;
    }

    bool IsQueuePublishInvoke(SemanticModel model, InvocationExpressionSyntax syntax)
    {
        if (syntax.ToString().Contains("Publish"))
        {
            if ((model.GetSymbolInfo(syntax).Symbol as IMethodSymbol)?.IsExtensionMethod ?? false)
            {
                //TODO: Сделать обработку методов расширения, чтобы заходить в них и получать создание queue модели. Найдено в монолите: PublishUserProfileChangedQueueMessage
                return false;
            }
            
            var memberAccessExpressionSyntax = syntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax != null)
            {
                return "Cian.Queue.Services.IQueueService" == (model.GetSymbolInfo(memberAccessExpressionSyntax.Expression).Symbol as IFieldSymbol)?.Type.ToString();
            }
            
            // return "Cian.Queue.Services.IQueueService" == (model.GetSymbolInfo(
            //     ((Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax) syntax
            //         .Expression).Expression).Symbol as IFieldSymbol)?.Type.ToString();
        }

        return false;
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