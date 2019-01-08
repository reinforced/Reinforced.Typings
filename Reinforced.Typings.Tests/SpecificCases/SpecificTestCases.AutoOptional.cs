using System;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        interface OptionalTestInterface
        {
            DateTime? Date { get; }
            int? Number { get; }
            string String { get; }
        }

        class OptionalTestClass
        {
            public DateTime? Date { get; }
            public int? Number { get; }
            public string String { get; }
        }
        [Fact]
        public void AutoOptional()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IOptionalTestInterface
	{
		Date?: any;
		Number?: number;
		String: string;
	}
	export interface IOptionalTestClass
	{
		Date?: any;
		Number?: number;
		String: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().AutoOptionalProperties());
                s.ExportAsInterface<OptionalTestInterface>().WithPublicProperties();
                s.ExportAsInterface<OptionalTestClass>().WithPublicProperties();
            }, result);
        }
    }
}