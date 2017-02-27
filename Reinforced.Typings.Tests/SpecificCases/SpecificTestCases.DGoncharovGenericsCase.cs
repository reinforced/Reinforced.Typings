using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void DGoncharovGenericsCase()
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
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterfaces(new[] {typeof(TypedBasicResult<>)}, x => x.WithPublicProperties());
                s.ExportAsInterface<SelectListItem>().WithPublicProperties();
                s.ExportAsClass<RequestHandler>().WithPublicMethods();
            }, result);
        }
    }
}