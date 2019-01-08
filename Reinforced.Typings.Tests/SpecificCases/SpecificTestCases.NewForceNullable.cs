using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region New ForceNullable test

        public interface INewForceNullableTest
        {
            [TsProperty(ForceNullable = true)]
            int? NilInt { get; }
            int NotNilInt { get; }
            int? ForceNotNullableInt { get; }
            int ForceNullableInt { get; }
        }

        #endregion

        [Fact]
        public void NewForceNullable()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface INewForceNullableTest
	{
		ForceNotNullableInt: number;
		ForceNullableInt?: number;
		NilInt?: number;
		NotNilInt: number;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsInterface<INewForceNullableTest>()
                    .WithPublicProperties()
                    .WithProperty(c => c.ForceNotNullableInt, c => c.ForceNullable(false))
                    .WithProperty(c => c.ForceNullableInt, c => c.ForceNullable())
                    ;
            }, result);
        }
    }
}