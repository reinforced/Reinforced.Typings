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
##Int: number;
##String: string;
#}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().TabSymbol("#").ReorderMembers());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }
    }
}