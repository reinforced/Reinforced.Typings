using System;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void InlineFunctionImplementation()
        {
            string implementation1 = Guid.NewGuid().ToString() + ";";
            string implementation2 = Guid.NewGuid().ToString() + ";";

            string result = $@"
module Reinforced.Typings.Tests.SpecificCases {{
	export class ClassWithManyMethods
	{{
		public DoSomethinig() : void
		{{
			{implementation1}
		}}
		public DoSomethingElse() : void
		{{
			{implementation1}
		}}
		public DoSomethingElseWithResult() : string
		{{
			{implementation2}
		}}
	}}
}}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<ClassWithManyMethods>()
                    .WithPublicMethods(c => c.Implement(implementation1))
                    .WithMethod(c => c.DoSomethingElseWithResult(), c => c.Implement(implementation2))
                    ;
            }, result);
        }
    }
}