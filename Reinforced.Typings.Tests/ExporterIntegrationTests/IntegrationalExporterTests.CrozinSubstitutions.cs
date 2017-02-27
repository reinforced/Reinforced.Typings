using System;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void CrozinSubstitutions()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
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