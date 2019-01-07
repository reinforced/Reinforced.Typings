using System;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region Substitutions

        public class CrozinSubstitutionTest
        {
            public Guid GuidProperty { get; set; }

            public DateTime TimeProperty { get; set; }
        }

        public class CrozinLocalSubstitutionTest
        {
            public Guid OneMoreGuidProperty { get; set; }

            public DateTime OneMoreTimeProperty { get; set; }
        }

        #endregion

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
                s.Global(x => x.DontWriteWarningComment().ReorderMembers());
                s.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
                s.ExportAsInterface<CrozinSubstitutionTest>().WithPublicProperties();
                s.ExportAsInterface<CrozinLocalSubstitutionTest>()
                    .WithPublicProperties()
                    .Substitute(typeof(DateTime), new RtSimpleTypeName("Date"));
            }, result);
        }
    }
}