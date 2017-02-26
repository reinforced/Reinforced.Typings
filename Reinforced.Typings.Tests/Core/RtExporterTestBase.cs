using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Tokenizing;
using Xunit;

namespace Reinforced.Typings.Tests.Core
{
    public abstract class RtExporterTestBase
    {
        protected void AssertConfiguration(Action<ConfigurationBuilder> configuration, string result)
        {
            const string sample = "target.ts";
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = false,
                SourceAssemblies = new Assembly[] { Assembly.GetExecutingAssembly() },
                TargetDirectory = "D:\\",
                TargetFile = sample
            };
            TsExporter te = new TsExporter(ec);
            te.Export();
            Assert.True(mfo.DeployCalled);
            Assert.True(mfo.TempRegistryCleared);
            Assert.True(mfo.ExportedFiles[sample].TokenizeCompare(result));
        }

        protected void AssertConfiguration(Action<ConfigurationBuilder> configuration, Dictionary<string,string> results)
        {
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = true,
                SourceAssemblies = new Assembly[] { Assembly.GetExecutingAssembly() },
                TargetDirectory = "D:\\"
            };
            TsExporter te = new TsExporter(ec);
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
