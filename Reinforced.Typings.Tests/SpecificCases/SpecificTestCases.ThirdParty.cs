using System.Collections.Generic;
using System.IO;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.ReferencesInspection;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        interface IGenericKendoDataNode<T>
        {
            T GenericProperty1 { get; }
        }

        class GenericKendoNodeTest : IGenericKendoDataNode<string>
        {
            public int Property { get; set; }
            public KendoDataNode Parent { get; set; }
            public string GenericProperty1 { get; }
        }

        class GenericKendoNodeTest2<TN> : IGenericKendoDataNode<TN>
        {
            public TN GenericProperty1 { get; }
        }
        [Fact]
        public void ThirdPartyTests()
        {
            const string file1 = @"
import { kendo.data.Node } from '../vendor/kendo.core.ts';
import { kendo.data.GenericNode } from '../vendor/kendo.core.ts';

export interface IMyTreeNode extends kendo.data.Node
{
	Id: number;
}
export interface IGenericKendoNodeTest extends kendo.data.Node<string>
{
	GenericProperty1: string;
	Parent: kendo.data.Node;
	Property: number;
}
export interface IGenericKendoNodeTest2<TN> extends kendo.data.Node<TN>
{
	GenericProperty1: TN;
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment()
                    .UseModules()
                    .ReorderMembers());
                s.ExportAsThirdParty<KendoDataNode>().WithName("kendo.data.Node")
                    .Imports(new RtImport() { From = "../vendor/kendo.core.ts", Target = "{ kendo.data.Node }" });

                s.ExportAsThirdParty(new[] { typeof(IGenericKendoDataNode<>) }, d => d.WithName("kendo.data.Node")
                     .Imports(new RtImport() { From = "../vendor/kendo.core.ts", Target = "{ kendo.data.GenericNode }" }));

                s.ExportAsInterface<MyTreeNode>().WithPublicProperties();
                s.ExportAsInterface<GenericKendoNodeTest>().WithPublicProperties();
                s.ExportAsInterfaces(new[] { typeof(GenericKendoNodeTest2<>) }, d => d.WithPublicProperties());
            }, file1);
        }
    }
}