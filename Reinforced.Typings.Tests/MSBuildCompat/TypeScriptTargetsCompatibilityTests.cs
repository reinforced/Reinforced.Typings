using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Reinforced.Typings.Tests.MSBuildCompat
{
    /// <summary>
    /// Guards the Microsoft.TypeScript.MSBuild compatibility branching in stuff/Reinforced.Typings.targets
    /// against regressions: it must keep working for the classic package (CompileTypeScript /
    /// CompileTypeScriptWithTSConfig, up to v6.x) as well as the tsgo-based v7+ package
    /// (SetTscExecutable / RunTscCompile), selected via the $(UsingMicrosoftTsGo) flag.
    /// </summary>
    public class TypeScriptTargetsCompatibilityTests
    {
        private static readonly XNamespace Msb = "http://schemas.microsoft.com/developer/msbuild/2003";

        private static XDocument LoadTargetsFile()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Reinforced.Typings.sln")))
            {
                dir = dir.Parent;
            }

            Assert.True(dir != null, "Could not locate repository root from " + AppContext.BaseDirectory);

            var targetsPath = Path.Combine(dir.FullName, "stuff", "Reinforced.Typings.targets");
            Assert.True(File.Exists(targetsPath), "Expected to find " + targetsPath);
            return XDocument.Load(targetsPath);
        }

        private static XElement GetTarget(XDocument doc, string targetName)
        {
            return doc.Descendants(Msb + "Target").Single(t => (string)t.Attribute("Name") == targetName);
        }

        private static string ConditionOfMSBuildTask(XDocument doc, string targetName, string targetsValue)
        {
            var target = GetTarget(doc, targetName);
            var task = target.Elements(Msb + "MSBuild").Single(t => (string)t.Attribute("Targets") == targetsValue);
            return (string)task.Attribute("Condition") ?? "";
        }

        [Fact]
        public void LegacyCompileTypeScriptInvocationIsSkippedForTsgoPackage()
        {
            var condition = ConditionOfMSBuildTask(LoadTargetsFile(), "ReinforcedTypingsGenerate", "CompileTypeScript");
            Assert.Contains("$(UsingMicrosoftTsGo)", condition);
            Assert.Contains("!= 'true'", condition);
        }

        [Fact]
        public void LegacyCompileTypeScriptWithTSConfigInvocationIsSkippedForTsgoPackage()
        {
            var condition = ConditionOfMSBuildTask(LoadTargetsFile(), "ReinforcedTypingsGenerate", "CompileTypeScriptWithTSConfig");
            Assert.Contains("$(UsingMicrosoftTsGo)", condition);
            Assert.Contains("!= 'true'", condition);
        }

        [Fact]
        public void TsgoRunTscCompileInvocationOnlyRunsForTsgoPackage()
        {
            var condition = ConditionOfMSBuildTask(LoadTargetsFile(), "ReinforcedTypingsGenerate", "SetTscExecutable;RunTscCompile");
            Assert.Contains("$(UsingMicrosoftTsGo)", condition);
            Assert.Contains("== 'true'", condition);
        }

        [Fact]
        public void DisableStepStripsBothLegacyAndTsgoDependsOnChains()
        {
            var target = GetTarget(LoadTargetsFile(), "ConditionallyDisableTypeScriptCompilation");
            var originals = target.Elements(Msb + "RemoveTypeScriptStep")
                .Select(s => (string)s.Attribute("Original"))
                .ToArray();

            Assert.Contains("$(CompileDependsOn)", originals);
            Assert.Contains("$(PublishPipelineCollectFilesCore)", originals);
            Assert.Contains("$(BuiltProjectOutputGroupDependsOn)", originals);
            // Tsgo (v7+) hooks TypeScript compilation into CoreCompileDependsOn instead of the legacy lists above.
            Assert.Contains("$(CoreCompileDependsOn)", originals);
        }
    }
}
