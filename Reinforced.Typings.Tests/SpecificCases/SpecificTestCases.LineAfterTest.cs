using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        class LineAfterAddingGenerator : EnumGenerator
        {
            /// <summary>
            ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
            ///     WriterWrapper (3rd argument) using TypeResolver if necessary
            /// </summary>
            /// <param name="element">Element code to be generated to output</param>
            /// <param name="result">Resulting node</param>
            /// <param name="resolver">Type resolver</param>
            public override RtEnum GenerateNode(Type element, RtEnum result, TypeResolver resolver)
            {
                var r = base.GenerateNode(element, result, resolver);
                if (r == null) return null;
                foreach (var val in r.Values)
                {
                    val.LineAfter = " ";
                }
                return r;
            }
        }
        [Fact]
        public void LineAfterTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export enum TestEnum2 { 
		/** C Value */
		C = 0, 
		 
		/** D Value */
		D = 1, 
		 
		/** E Value */
		E = 2		 

	}
}";
            var str = AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
                s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);
                s.ExportAsEnum<TestEnum2>().WithCodeGenerator<LineAfterAddingGenerator>();
            }, result);

            Assert.Equal(str,result);
        }
    }
}