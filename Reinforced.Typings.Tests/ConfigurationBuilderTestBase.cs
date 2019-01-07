using System;
using System.Reflection;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Core;

namespace Reinforced.Typings.Tests
{
    public abstract class ConfigurationBuilderTestBase
    {
        protected const string TargetDir = "targetDir";
        protected const string Sample = "target.ts";

        protected TestInitializationData InitializeSingleFile(Action<ConfigurationBuilder> configuration)
        {
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TestFluentAssembly.TwoInterfaces.IInterface1).Assembly },mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = false,
                TargetDirectory = TargetDir,
                TargetFile = Sample
            };
            TsExporter te = new TsExporter(ec);
            te.Initialize();
            return new TestInitializationData(mfo, te);
        }
        
        protected TestInitializationData InitializeMultipleFiles(Action<ConfigurationBuilder> configuration)
        {
            MockFileOperations mfo = new MockFileOperations();

            ExportContext ec = new ExportContext(new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TestFluentAssembly.TwoInterfaces.IInterface1).Assembly },mfo)
            {
                ConfigurationMethod = configuration,
                Hierarchical = true,
                TargetDirectory = TargetDir
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
