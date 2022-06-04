// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using codegraph;
using codegraph.common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;


Void SaveMetadata(String s, String repoName1)
{
    var repository = new Repository($@"{s}{repoName1}\");

    var parser = new SolutionParser(repository);

    var dependencies = parser.GetQueueDependencies();

    DbWorker.Save(dependencies);
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

    var repoNames = new[] {"yandex-payments"};
    var sources = @"C:\Users\o.sidorov\cian\git\microservices\";
    foreach (var repoName in repoNames)
    {
        SaveMetadata(sources, repoName);
    }
}
