using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        public class MyMultilineClass
        {
            public static string MyStaticProperty = @"My
multiline
string";
        }

     
    [Fact]
        public void NvirthMultilineString()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export class MyMultilineClass
	{
		public static MyStaticProperty: string = `My
multiline
string`;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<MyMultilineClass>().WithPublicFields().WithPublicProperties();
            }, result);
        }
    }
}