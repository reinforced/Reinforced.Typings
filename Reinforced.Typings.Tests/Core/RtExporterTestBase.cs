using System;
using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Tokenizing;
using Xunit;

namespace Reinforced.Typings.Tests.Core
{
    public abstract class RtExporterTestBase : ConfigurationBuilderTestBase
    {
        protected string AssertConfiguration(Action<ConfigurationBuilder> configuration, string result, bool compareComments = false)
        {
            var data = InitializeSingleFile(configuration);
            var te = data.Exporter;
            var mfo = data.Files;
            te.Export();
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            var actual = mfo.ExportedFiles[Sample]; //<--- variable to check in debugger
            Assert.True(actual.TokenizeCompare(result, compareComments)); //<--- best place to put breakpoint
            return actual;
        }

        protected string AssertConfiguration(Action<ConfigurationBuilder> configuration, string result, Action<TsExporter> expAction, bool compareComments = false)
        {
            var data = InitializeSingleFile(configuration);
            var te = data.Exporter;
            var mfo = data.Files;
            te.Export();
            expAction(te);
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            var actual = mfo.ExportedFiles[Sample]; //<--- variable to check in debugger
            Assert.True(actual.TokenizeCompare(result, compareComments)); //<--- best place to put breakpoint
            return actual;
        }

        protected void AssertConfiguration(Action<ConfigurationBuilder> configuration, Dictionary<string,string> results, bool compareComments = false)
        {
            var data = InitializeMultipleFiles(configuration);
            var te = data.Exporter;
            var mfo = data.Files;
            te.Export();
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            var exportedFiles = mfo.ExportedFiles; //<--- variable to check in debugger
            foreach (var mfoExportedFile in exportedFiles) 
            {
                var generated = mfoExportedFile.Value; //<--- variable to check in debugger
                Assert.True(results.ContainsKey(mfoExportedFile.Key)); //<--- best place to put breakpoint
                var expected = results[mfoExportedFile.Key];
                Assert.True(generated.TokenizeCompare(expected,compareComments));
            }
        }
    }
}
