// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using CodeGraph;
using CodeGraph.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;


List<QueueModel> CollectMetadata(string path)
{
    var repository = new Repository(path);

    var parser = new SolutionParser(repository);

    var result = parser.GetQueueDependencies();

    return result;
}

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

    var repoPaths = new[]
    {
        @"C:\Users\o.sidorov\cian\git\microservices\yandex-payments", 
        @"C:\Users\o.sidorov\cian\git\microservices\billing-accounts",
        @"C:\Users\o.sidorov\cian\git\microservices\billing-payments",
        @"C:\Users\o.sidorov\cian\git\microservices\billing-subscriptions",
        //@"C:\Users\o.sidorov\cian\git\monolith\csharp" -- пока не поддерживается
    };

    var result = new List<QueueModel>();
    foreach (var path in repoPaths)
    {
        result.AddRange(CollectMetadata(path));
    }
    
    DbWorker.Save(result);
}
