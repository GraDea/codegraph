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

    var repository = new Repository(@"C:\Users\o.sidorov\cian\git\microservices\yandex-payments\");

    var parser = new SolutionParser(repository);

    var dependencies = parser.GetQueueDependencies();
    
    DbWorker.Save(dependencies);
}
