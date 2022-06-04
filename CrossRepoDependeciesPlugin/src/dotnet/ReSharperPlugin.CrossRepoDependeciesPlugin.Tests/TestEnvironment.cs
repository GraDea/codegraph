using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.CrossRepoDependenciesPlugin.Tests
{
    [ZoneDefinition]
    public class CrossRepoDependenciesPluginTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<ICrossRepoDependenciesPluginZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<CrossRepoDependenciesPluginTestEnvironmentZone> { }

    [SetUpFixture]
    public class CrossRepoDependenciesPluginTestsAssembly : ExtensionTestEnvironmentAssembly<CrossRepoDependenciesPluginTestEnvironmentZone> { }
}
