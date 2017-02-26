using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void JonsaCustomIndentationTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
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