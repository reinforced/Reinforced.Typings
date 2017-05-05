using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void PandaWoodIgnoreSuitableTypeWarningCase()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IPandaWoodIgnoreSuitableTypeNotFoundCase
	{
		UnknownTypeFieldName: any;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.IgnoreSuitableTypeWarning());
                s.ExportAsInterface<PandaWoodIgnoreSuitableTypeNotFoundCase>().WithPublicProperties();
            }, result);
        }
    }
}