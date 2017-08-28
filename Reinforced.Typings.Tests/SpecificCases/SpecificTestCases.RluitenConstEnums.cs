using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void RluitenConstEnums()
        {
            const string result = @"
export const enum SomeEnum { 
	One = 0, 
	Two = 1, 
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsEnum<SomeEnum>().Const();
            }, result);
        }
    }
}