using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        enum SomeInitializerEnum
        {
            Value1, 
            Value2, 
            [TsValue(Initializer = "Three")]
            Value3
        }
        [Fact]
        public void EnumStringInitializers()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export enum SomeInitializerEnum { 
		Value1 = ""One"", 

            Value2 = ""Two"", 
            Value3 = ""Three""

        }
    }";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsEnum<SomeInitializerEnum>()
                    .Value(SomeInitializerEnum.Value1, d => d.Initializer("One"))
                    .Value(SomeInitializerEnum.Value2, d => d.Initializer("Two"))
                    .UseString();
            }, result);
        }
    }
}