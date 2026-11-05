using Reinforced.Typings.Integrate;
using Xunit;

namespace Reinforced.Typings.Tests.MSBuildCompat
{
    /// <summary>
    /// RemoveTypescriptStep is what makes Reinforced.Typings.targets work against both the
    /// classic Microsoft.TypeScript.MSBuild target names (up to v6.x) and the tsgo-based
    /// target names introduced in v7+ ("SetTscExecutable"/"RunTscCompile").
    /// </summary>
    public class RemoveTypescriptStepTests
    {
        private static string[] Run(string original)
        {
            var step = new RemoveTypescriptStep { Original = original };
            Assert.True(step.Execute());
            return step.Fixed;
        }

        [Fact]
        public void RemovesLegacyCompileTypeScriptTarget()
        {
            var result = Run("ResolveReferences;CompileTypeScript;CoreCompile");
            Assert.Equal(new[] { "ResolveReferences", "CoreCompile" }, result);
        }

        [Fact]
        public void RemovesLegacyCompileTypeScriptWithTSConfigTarget()
        {
            var result = Run("ResolveReferences;CompileTypeScriptWithTSConfig;CoreCompile");
            Assert.Equal(new[] { "ResolveReferences", "CoreCompile" }, result);
        }

        [Fact]
        public void RemovesLegacyGetTypeScriptOutputForPublishingTarget()
        {
            var result = Run("FindConfigFiles;GetTypeScriptOutputForPublishing");
            Assert.Equal(new[] { "FindConfigFiles" }, result);
        }

        [Fact]
        public void RemovesTsgoSetTscExecutableAndRunTscCompileTargets()
        {
            var result = Run("_ComputeNonExistentFileInputs;SetTscExecutable;RunTscCompile;CoreCompile");
            Assert.Equal(new[] { "_ComputeNonExistentFileInputs", "CoreCompile" }, result);
        }

        [Fact]
        public void PreservesUnrelatedTargets()
        {
            var result = Run("BeforeCompile;ResolveAssemblyReferences;AfterCompile");
            Assert.Equal(new[] { "BeforeCompile", "ResolveAssemblyReferences", "AfterCompile" }, result);
        }

        [Fact]
        public void HandlesWhitespaceAndEmptyEntries()
        {
            // Whitespace-only segments between semicolons are trimmed down to an empty string but,
            // just like the original code, are not otherwise filtered out - only recognized target
            // names are removed.
            var result = Run(" CompileTypeScript ; ; CoreCompile ;");
            Assert.Equal(new[] { "", "CoreCompile" }, result);
        }

        [Fact]
        public void HandlesLegacyAndTsgoTargetsPresentSimultaneously()
        {
            // Covers a project that ends up with both hook styles present at once.
            var result = Run("CompileTypeScript;SetTscExecutable;RunTscCompile;CoreCompile");
            Assert.Equal(new[] { "CoreCompile" }, result);
        }
    }
}
