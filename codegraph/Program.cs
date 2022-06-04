// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Reflection.Emit;
using codegraph;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;


try
{
    Work();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

void Work()
{
    var consumers = new List<MessageSettings>();
    var publishers = new List<MessageSettings>();

    // start Roslyn workspace
    Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
    MSBuildWorkspace workspace = MSBuildWorkspace.Create();

    // open solution we want to analyze
    Solution solutionToAnalyze =
        workspace.OpenSolutionAsync(@"C:\Users\o.sidorov\cian\git\microservices\yandex-payments\YandexPayments.sln").Result;

    Project project = solutionToAnalyze.Projects
            .FirstOrDefault(proj => proj.Name == "Common");
    
    Compilation compilation = project.GetCompilationAsync().Result;
    
    foreach (var document in project.Documents)
    {
        var tree = document.GetSyntaxTreeAsync().Result;
        var root = tree.GetRoot();
        var model = compilation.GetSemanticModel(tree);
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        foreach (var syntax in classes)
        {
            if(IsConsumerQueueModel(syntax))
            {
                //var symbol = model.GetDeclaredSymbol(syntax);
                var constructorDeclarationSyntax = syntax.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
                if (constructorDeclarationSyntax != null)
                {
                    var arguments = constructorDeclarationSyntax.Initializer.ArgumentList.Arguments;

                    if (arguments.Count == 3)
                    {
                        var exchangeName = (model.GetSymbolInfo(arguments[1].Expression).Symbol as IFieldSymbol).ConstantValue.ToString();
                        var routingKey = arguments[2].Expression.ToString();
                        
                        consumers.Add(new MessageSettings()
                        {
                            ExchangeName = exchangeName,
                            RoutingKey = routingKey
                        });
                        //Console.WriteLine($"{syntax.Identifier.Value}: {exchangeName} - {routingKey}");
                    }
                }
            }
            //... need to analyze properties in the class here...
        }
    }

    // var queueModelType = compilation.GetTypesByMetadataName("Common.Queues.PaymentChangedReportingQueueMessage").FirstOrDefault();
    //
    // queueModelType.ToString();
    
    //Assembly.LoadFile()
}

bool IsConsumerQueueModel(ClassDeclarationSyntax classDeclarationSyntax)
{
    return classDeclarationSyntax.BaseList?.Types.FirstOrDefault(x =>
        "IConsumedQueueMessage" == GetBaseTypeName(x)) != null;
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

// void Work()
// {
//     var filepath = @"C:\Users\o.sidorov\cian\git\microservices\yandex-payments\Scheduler\Tasks\HandleCallbackEvents\Services\Implementation\Strategies\PaymentEventHandler.cs";
//     //var filepath =
//         //@"C:\Users\o.sidorov\cian\git\microservices\yandex-payments\Common\Queues\PaymentChangedReportingQueueMessage.cs";
//
//     var tree = GetSyntaxTree(filepath);
//     var root = tree.GetCompilationUnitRoot();
//
//     var collector = new QueuePublishCollector();
//     collector.Visit(root);
//     
//     // foreach (var classDeclaration in collector.in)
//     // {
//     //     Console.WriteLine(classDeclaration);
//     // }
// }


SyntaxTree GetSyntaxTree(string filepath)
{
    return CSharpSyntaxTree.ParseText(File.ReadAllText(filepath));
}


public class MessageSettings
{
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
}