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

        protected ExportedFile Setup2Files(string filePath1, string filePath2, Action<ConfigurationBuilder> builder)
        {
            var setup = base.InitializeMultipleFiles(a =>
            {
                builder(a);
                a.ExportAsInterface<TestFluentAssembly.TwoInterfaces.IInterface1>().ExportTo(filePath1);
                a.ExportAsInterface<TestFluentAssembly.TwoInterfaces.IInterface2>().ExportTo(filePath2);
            });

            return setup.Exporter.SetupExportedFile("D:\\" + filePath1);
        }

        [Fact]
        public void SimpleReferenceResolvationTestSingleDir()
        {
            var file = Setup2Files("File1.ts", "File2.ts", x => x.Global(a => a.UseModules(false)));
            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.TwoInterfaces.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2") { Prefix = "TestFluentAssembly.TwoInterfaces" }, typeName, _comparer);
            Assert.Single(file.References.References);
            var rf = file.References.References.First();
            Assert.Equal("File2.ts", rf.Path);
        }

        [Fact]
        public void SimpleModuleResolvationTestSingleDir()
        {
            var file = Setup2Files("File1.ts", "File2.ts", x => x.Global(a => a.UseModules()));

            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.TwoInterfaces.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2"), typeName, _comparer);
            Assert.Single(file.References.Imports);
            var rf = file.References.Imports.First();
            Assert.Equal("./File2", rf.From);
            Assert.Equal("{ IInterface2 }", rf.Target);
        }

        [Fact]
        public void SimpleModuleWithNamespaceResolvationTestSingleDir()
        {
            var file = Setup2Files("File1.ts", "File2.ts", x => x.Global(a => a.UseModules(discardNamespaces: false)));

            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.TwoInterfaces.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2") { Prefix = "File2.TestFluentAssembly.TwoInterfaces" }, typeName, _comparer);
            Assert.Single(file.References.Imports);
            var rf = file.References.Imports.First();
            Assert.True(rf.IsWildcard);
            Assert.Equal("File2", rf.WildcardAlias);

            Assert.Equal("./File2", rf.From);
            Assert.Equal("* as File2", rf.Target);
        }

        [Fact]
        public void SimpleReferenceResolvationTestDifferentDirs()
        {
            var file = Setup2Files("File1.ts", "Another/File2.ts", x => x.Global(a => a.UseModules(false)));
            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.TwoInterfaces.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2") { Prefix = "TestFluentAssembly.TwoInterfaces" }, typeName, _comparer);
            Assert.Single(file.References.References);
            var rf = file.References.References.First();
            Assert.Equal("Another/File2.ts", rf.Path);
        }

        [Fact]
        public void SimpleModuleResolvationTestDifferentDirs()
        {
            var file = Setup2Files("File1.ts", "Another/File2.ts", x => x.Global(a => a.UseModules()));

            var typeName = file.TypeResolver.ResolveTypeName(typeof(TestFluentAssembly.TwoInterfaces.IInterface2));

            Assert.Equal(new RtSimpleTypeName("IInterface2"), typeName, _comparer);
            Assert.Single(file.References.Imports);
            var rf = file.References.Imports.First();
            Assert.Equal("./Another/File2", rf.From);
            Assert.Equal("{ IInterface2 }", rf.Target);
        }
    }
}
