﻿// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using CodeGraph;
using CodeGraph.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;


List<QueueModel> CollectMetadata(string s, string repoName)
{
    var repository = new Repository($@"{s}{repoName}\");

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

    var repoNames = new[] {"yandex-payments", "billing-accounts"};
    var sources = @"C:\Users\o.sidorov\cian\git\microservices\";

    var result = new List<QueueModel>();
    foreach (var repoName in repoNames)
    {
        result.AddRange(CollectMetadata(sources, repoName));
    }
    
    DbWorker.Save(result);
}
