// See https://aka.ms/new-console-template for more information

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
    // start Roslyn workspace
    Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
    MSBuildWorkspace workspace = MSBuildWorkspace.Create();

// open solution we want to analyze
    Solution solutionToAnalyze =
        workspace.OpenSolutionAsync(@"C:\Users\o.sidorov\cian\git\microservices\yandex-payments\YandexPayments.sln").Result;

    Project sampleProjectToAnalyze = solutionToAnalyze.Projects
            .FirstOrDefault(proj => proj.Name == "Scheduler");

// get the project's compilation
// compilation contains all the types of the 
// project and the projects referenced by 
// our project. 
    Compilation compilation =
        sampleProjectToAnalyze.GetCompilationAsync().Result;
    
    var queueModelType = compilation.GetTypesByMetadataName("Common.Queues.PaymentChangedReportingQueueMessage");

    //queueModelType.Construct
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