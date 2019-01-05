using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void DecoratorsTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	@sealed export class ClassWithMethods
	{
		@bind public String: string;
		@bind public Int: number;
		@a() @b() public DoSomethinig() : void { } 
	}	  
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<ClassWithMethods>()
                    .WithPublicProperties(a => a.Decorator("bind"))
                    .WithMethod(c => c.DoSomethinig(), c => c.Decorator("a()", -1))
                    .Decorator("sealed")
                    ;
            }, result);
        }
    }
}