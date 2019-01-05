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
        class MyRefProcessor : ReferenceProcessorBase
        {
            public override IEnumerable<RtImport> FilterImports(IEnumerable<RtImport> imports, ExportedFile file)
            {
                foreach (var rtImport in imports)
                {
                    if (!string.IsNullOrEmpty(rtImport.From) && rtImport.From.Contains("kendo.data"))
                        continue;
                    yield return rtImport;
                }
            }

            public override IEnumerable<RtReference> FilterReferences(IEnumerable<RtReference> references, ExportedFile file)
            {
                return references;
            }
        }

        [TsThirdParty]
        interface KendoDataNode { }

        public class MyTreeNode : KendoDataNode
        {
            public int Id { get; set; }
        }

        [Fact]
        public void ReferencesProcessor()
        {
            const string file1 = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ITestInterface
	{
		String: string;
		Int: number;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterface<KendoDataNode>().WithPublicProperties()
                    .AutoI(false)
                    .OverrideName("kendo.data.Node")
                    .ExportTo("kendo.ts");
                s.ExportAsInterface<MyTreeNode>().WithPublicProperties().ExportTo("file1.ts");
            } , new Dictionary<string, string>
            {
                { Path.Combine(TargetDir, "Exported", "File1.ts"), file1 },
                { Path.Combine(TargetDir, "Exported", "kendo.ts"), file1 },
            });
        }
    }
}