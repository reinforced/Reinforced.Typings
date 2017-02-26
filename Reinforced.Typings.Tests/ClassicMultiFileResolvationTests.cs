using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Core;
using Xunit;

namespace Reinforced.Typings.Tests
{
    public class ClassicMultiFileResolvationTests : RtExporterTestBase
    {
        private readonly TypeNameEqualityComparer _comparer = new TypeNameEqualityComparer();
        

        [Fact]
        public void SimpleReferenceResolvationTest()
        {
            var setup = base.InitializeMultipleFiles(a =>
            {
                a.Global(x => x.UseModules(false));
                a.ExportAsInterface<TestFluentAssembly.IInterface1>().ExportTo("File1.ts");
                a.ExportAsInterface<TestFluentAssembly.IInterface2>().ExportTo("File2.ts");
            });

            var file = setup.Exporter.SetupExportedFile("D:\\File1.ts");
            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2") { Prefix = "TestFluentAssembly" }, typeName, _comparer);
            Assert.Single(file.References.References);
            var rf = file.References.References.First();
            Assert.Equal(rf.Path,"File2.ts");
        }

        [Fact]
        public void SimpleModuleResolvationTest()
        {
            var setup = base.InitializeMultipleFiles(a =>
            {
                a.Global(x => x.UseModules());
                a.ExportAsInterface<TestFluentAssembly.IInterface1>().ExportTo("File1.ts");
                a.ExportAsInterface<TestFluentAssembly.IInterface2>().ExportTo("File2.ts");
            });

            var file = setup.Exporter.SetupExportedFile("D:\\File1.ts");
            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2"), typeName, _comparer);
            Assert.Single(file.References.Imports);
            var rf = file.References.Imports.First();
            Assert.Equal(rf.From, "./File2.ts");
            Assert.Equal(rf.Target, "{ IInterface2 }");
        }

        [Fact]
        public void SimpleModuleWithNamespaceResolvationTest()
        {
            var setup = base.InitializeMultipleFiles(a =>
            {
                a.Global(x => x.UseModules(discardNamespaces:false));
                a.ExportAsInterface<TestFluentAssembly.IInterface1>().ExportTo("File1.ts");
                a.ExportAsInterface<TestFluentAssembly.IInterface2>().ExportTo("File2.ts");
            });

            var file = setup.Exporter.SetupExportedFile("D:\\File1.ts");
            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2") {Prefix = "File2.TestFluentAssembly" }, typeName, _comparer);
            Assert.Single(file.References.Imports);
            var rf = file.References.Imports.First();
            Assert.True(rf.IsWildcard);
            Assert.Equal(rf.WildcardAlias,"File2");

            Assert.Equal(rf.From, "./File2.ts");
            Assert.Equal(rf.Target, "* as File2");
        }

    }
}
