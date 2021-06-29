using System;
using System.IO;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Reinforced.Typings.Visitors.TypeScript;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        class LineAfterVisitor : TypeScriptExportVisitor
        {
            public LineAfterVisitor(TextWriter writer, ExportContext exportContext) : base(writer, exportContext)
            {
            }

            public override void Visit(RtEnumValue node)
            {
                WriteLines(@"
// Lick the belly!
");
                base.Visit(node);
            }

            public override void VisitFile(ExportedFile file)
            {
                base.VisitFile(file);
                var ns = file.Namespaces.FirstOrDefault();
                if (ns != null)
                {
                    WriteLines($@"
export = {ns.Name};

");
                }
            }
        }
        [Fact]
        public void LineAfterAnotherTest()
        {
            const string result = @"module Reinforced.Typings.Tests.SpecificCases {
	export enum TestEnum2 {
		
		// Lick the belly!
		
		/** C Value */
		C = 0,
		 
		
		// Lick the belly!
		
		/** D Value */
		D = 1,
		 
		
		// Lick the belly!
		
		/** E Value */
		E = 2		 

	}
}

export = Reinforced.Typings.Tests.SpecificCases;


";
            var str = AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().GenerateDocumentation().UseVisitor<LineAfterVisitor>());
                s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);
                s.ExportAsEnum<TestEnum2>().WithCodeGenerator<LineAfterAddingGenerator>();
            }, result);

            Assert.Equal(result,str);
        }
    }
}