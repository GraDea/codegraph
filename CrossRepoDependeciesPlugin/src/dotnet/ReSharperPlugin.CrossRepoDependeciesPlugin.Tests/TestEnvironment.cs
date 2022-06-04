using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.CrossRepoDependeciesPlugin.Tests
{
    [ZoneDefinition]
    public class CrossRepoDependeciesPluginTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<ICrossRepoDependeciesPluginZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<CrossRepoDependeciesPluginTestEnvironmentZone> { }

    [SetUpFixture]
    public class CrossRepoDependeciesPluginTestsAssembly : ExtensionTestEnvironmentAssembly<CrossRepoDependeciesPluginTestEnvironmentZone> { }
}
