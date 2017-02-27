using System;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void CrozinSubstitutions()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ICrozinSubstitutionTest
	{
		GuidProperty: string;
		TimeProperty: any;
	}
	export interface ICrozinLocalSubstitutionTest
	{
		OneMoreGuidProperty: string;
		OneMoreTimeProperty: Date;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(x => x.DontWriteWarningComment());
                s.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
                s.ExportAsInterface<CrozinSubstitutionTest>().WithPublicProperties();
                s.ExportAsInterface<CrozinLocalSubstitutionTest>()
                    .Substitute(typeof(DateTime), new RtSimpleTypeName("Date"))
                    .WithPublicProperties();
            }, result);
        }
    }
}