using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void NewForceNullable()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface INewForceNullableTest
	{
		NilInt?: number;
		NotNilInt: number;
		ForceNotNullableInt: number;
		ForceNullableInt?: number;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<INewForceNullableTest>()
                    .WithPublicProperties()
                    .WithProperty(c => c.ForceNotNullableInt, c => c.ForceNullable(false))
                    .WithProperty(c => c.ForceNullableInt, c => c.ForceNullable())
                    ;
            }, result);
        }
    }
}