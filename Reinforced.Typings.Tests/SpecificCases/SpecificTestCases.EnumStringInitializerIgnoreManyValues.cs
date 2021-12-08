using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        enum EnumToIgnoreSomeValues
        {
            Value1, 
            Value2, 
            Value3
        }
        
        [Fact]
        public void EnumMultipleIgnoreValues()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export enum SomeInitializerEnum { 
        Value3 = ""Value3"" 
    }
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsEnum<EnumToIgnoreSomeValues>()
                    .Value(EnumToIgnoreSomeValues.Value1, d => d.Ignore())
                    .Value(EnumToIgnoreSomeValues.Value2, d => d.Ignore())
                    .UseString();
            }, result);
        }
    }
}