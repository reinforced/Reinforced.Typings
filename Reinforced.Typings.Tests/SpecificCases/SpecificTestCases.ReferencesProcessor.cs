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
        /// <summary>
        /// Example of implementation of custom reference processor
        /// </summary>
        private class MyRefProcessor : ReferenceProcessorBase
        {
            /// <summary>
            /// Returns refiltered and reordered import directives that must appear in resulting file.
            /// Return null to remain references list untouched
            /// </summary>
            /// <param name="imports">Set on initially computed imports</param>
            /// <param name="file">File that is being exported currently</param>
            /// <returns>Set of refiltered/reordered imports</returns>
            public override IEnumerable<RtImport> FilterImports(IEnumerable<RtImport> imports, ExportedFile file)
            {
                // here we also can return null to quickly reset references/imports list
                if (file.FileName.EndsWith("kendo.ts")) return imports;

                // modify imports only in meaningful file
                return FilterMainFileImports(imports);
            }

            private IEnumerable<RtImport> FilterMainFileImports(IEnumerable<RtImport> imports)
            {
                foreach (var rtImport in imports)
                {
                    if (!string.IsNullOrEmpty(rtImport.From) && rtImport.From.Contains("./kendo"))
                    {
                        // let's... simply change import path
                        rtImport.From = "../../vendor/kendo/kendo.core.ts";
                    }
                    yield return rtImport;
                }
            }

            /// <summary>
            /// Returns refiltered and reordered reference directives that must appear in resulting file
            /// Return null to remain imports list untouched
            /// </summary>
            /// <param name="references">Set on initially computed references</param>
            /// <param name="file">File that is being exported currently</param>
            /// <returns>Set of refiltered/reordered references</returns>
            public override IEnumerable<RtReference> FilterReferences(IEnumerable<RtReference> references, ExportedFile file)
            {
                // same rules as for imports
                // keep in mind that references are ALWAYS being written
                // whereas imports are not being used until .UseModules set to true
                if (file.FileName.EndsWith("kendo.ts")) return references;
                return FilterMainFileReferences(references);
            }

            private IEnumerable<RtReference> FilterMainFileReferences(IEnumerable<RtReference> references)
            {
                // let's... add some references at the top
                yield return new RtReference() { Path = "some.dummy.reference.ts" };
                foreach (var rtReference in references)
                {
                    yield return rtReference;
                }
            }
        }

        interface KendoDataNode { }

        public class MyTreeNode : KendoDataNode
        {
            public int Id { get; set; }
        }

        [Fact]
        public void ReferencesProcessor()
        {
            const string file1 = @"
///<reference path=""some.dummy.reference.ts""/>
import { kendo.data.Node } from '../../vendor/kendo/kendo.core.ts';

export interface IMyTreeNode extends kendo.data.Node
{
	Id: number;
}";

            const string kendots = @"
export interface kendo.data.Node
{
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment()
                    .UseModules()
                    .ReorderMembers()
                    .WithReferencesProcessor<MyRefProcessor>());
                s.ExportAsInterface<KendoDataNode>()
                    .AutoI(false)
                    .OverrideName("kendo.data.Node")
                    .ExportTo("kendo.ts");
                s.ExportAsInterface<MyTreeNode>().WithPublicProperties().ExportTo("file1.ts");
            } , new Dictionary<string, string>
            {
                { Path.Combine(TargetDir, "file1.ts"), file1 },
                { Path.Combine(TargetDir, "kendo.ts"), kendots },
            });
        }
    }
}