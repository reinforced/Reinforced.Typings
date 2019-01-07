using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region Decorators

        public class ClassWithMethods
        {
            public string String { get; set; }
            public int Int { get; set; }

            [TsDecorator("b()")]
            public void DoSomethinig()
            {

            }
        }



        #endregion

        [Fact]
        public void DecoratorsTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	@sealed export class ClassWithMethods
	{
		@bind public Int: number;
		@bind public String: string;
		@a() @b() public DoSomethinig() : void { } 
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsClass<ClassWithMethods>()
                    .WithPublicProperties(a => a.Decorator("bind"))
                    .WithMethod(c => c.DoSomethinig(), c => c.Decorator("a()", -1))
                    .Decorator("sealed")
                    ;
            }, result);
        }
    }
}