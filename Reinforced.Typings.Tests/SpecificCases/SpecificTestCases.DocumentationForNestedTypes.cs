using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        /// <summary>
        /// Some documentation for nested class
        /// </summary>
        public class SomeReallyNestedClass
        {
            /// <summary>Ctor comment</summary>
            public SomeReallyNestedClass(int x)
            {
            }
        }
        [Fact]
        public void DocumentationForNestedTypes()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	/** Some documentation for nested class */
	export class SomeReallyNestedClass
	{
		/** Ctor comment */
		constructor (x: number) { } 
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().GenerateDocumentation());
                s.TryLookupDocumentationForAssembly(typeof(SpecificTestCases).Assembly);
                s.ExportAsClass<SomeReallyNestedClass>()
                    .WithPublicProperties()
                    .WithConstructor();
            }, result, exp =>
            {
                var doc = exp.Context.Documentation.GetDocumentationMember(typeof(SomeReallyNestedClass));
                Assert.NotNull(doc);
            }, compareComments: true);
        }
    }
}