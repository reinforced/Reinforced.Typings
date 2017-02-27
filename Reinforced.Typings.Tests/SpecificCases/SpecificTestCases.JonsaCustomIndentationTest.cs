using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void JonsaCustomIndentationTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
#export interface ITestInterface
#{
##String: string;
##Int: number;
#}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().TabSymbol("#"));
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }
    }
}