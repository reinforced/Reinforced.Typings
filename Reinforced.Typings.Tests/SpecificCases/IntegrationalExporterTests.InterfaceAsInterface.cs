using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void InterfaceAsInterface()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ITestInterface
	{
		String: string;
		Int: number;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }
    }
}