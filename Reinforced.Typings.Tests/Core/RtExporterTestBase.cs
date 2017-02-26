using System;
using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Tokenizing;
using Xunit;

namespace Reinforced.Typings.Tests.Core
{
    public abstract class RtExporterTestBase : ConfigurationBuilderTestBase
    {
        protected void AssertConfiguration(Action<ConfigurationBuilder> configuration, string result)
        {
            var data = InitializeSingleFile(configuration);
            var te = data.Exporter;
            var mfo = data.Files;
            te.Export();
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            var actual = mfo.ExportedFiles[Sample];
            Assert.True(actual.TokenizeCompare(result));
        }

        protected void AssertConfiguration(Action<ConfigurationBuilder> configuration, Dictionary<string,string> results)
        {
            var data = InitializeSingleFile(configuration);
            var te = data.Exporter;
            var mfo = data.Files;
            te.Export();
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            foreach (var mfoExportedFile in mfo.ExportedFiles)
            {
                var generated = mfoExportedFile.Value;
                Assert.True(results.ContainsKey(mfoExportedFile.Key));
                var expected = results[mfoExportedFile.Key];
                Assert.True(generated.TokenizeCompare(expected));
            }
        }
    }
}
