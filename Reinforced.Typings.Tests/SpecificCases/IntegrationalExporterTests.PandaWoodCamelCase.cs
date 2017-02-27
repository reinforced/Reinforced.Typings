using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void PandaWoodCamelCase()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IPandaWoodCamelCaseTest
	{
		id: number;
		eta() : void;
		isoDate(date: any) : void;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().CamelCaseForMethods().CamelCaseForProperties());
                s.ExportAsInterface<PandaWoodCamelCaseTest>().WithPublicProperties().WithPublicMethods();
            }, result);
        }
    }
}