using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Core;

namespace Reinforced.Typings.Tests
{
    public abstract class ConfigurationBuilderTestBase
    {
        protected const string Sample = "target.ts";

        protected TestInitializationData InitializeSingleFile(Action<ConfigurationBuilder> configuration)
        {
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = false,
                SourceAssemblies = new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TestFluentAssembly.TwoInterfaces.IInterface1).Assembly },
                TargetDirectory = "D:\\",
                TargetFile = Sample
            };
            TsExporter te = new TsExporter(ec);
            te.Initialize();
            return new TestInitializationData(mfo, te);
        }
        
        protected TestInitializationData InitializeMultipleFiles(Action<ConfigurationBuilder> configuration)
        {
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = true,
                SourceAssemblies = new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TestFluentAssembly.TwoInterfaces.IInterface1).Assembly },
                TargetDirectory = "D:\\"
            };
            TsExporter te = new TsExporter(ec);
            te.Initialize();
            return new TestInitializationData(mfo, te);
        }

    }

    public class TestInitializationData
    {
        public MockFileOperations Files { get; private set; }

        public TsExporter Exporter { get; private set; }

        public TestInitializationData(MockFileOperations files, TsExporter exporter)
        {
            Files = files;
            Exporter = exporter;
        }
    }
}
