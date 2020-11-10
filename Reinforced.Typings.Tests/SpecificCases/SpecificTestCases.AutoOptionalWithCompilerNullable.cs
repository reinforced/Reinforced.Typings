using System;
using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

#nullable enable

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        public interface OptionalTestInterfaceNullableAttribute
        {
            DateTime? Date { get; }
            int Number { get; }
            public int[]? Numbers { get; }
            string? String { get; set; }
            public IList<int>? SomeList { get; }
        }

        public class OptionalTestClassNullableAttribute
        {
            public DateTime? Date { get; }
            public int? Number { get; }
            public int[] Numbers { get; }
            public string String { get; }
            public IList<int> SomeList { get; }
        }

        [Fact]
        public void AutoOptionalIfCompilerNullableAttribute()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IOptionalTestInterfaceNullableAttribute
	{
		Date?: any;
		Number: number;
		Numbers?: number[];
		String?: string;
		SomeList?: number[];
	}
	export interface IOptionalTestClassNullableAttribute
	{
		Date?: any;
		Number?: number;
		Numbers: number[];
		String: string;
		SomeList: number[];
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().AutoOptionalProperties());
                s.ExportAsInterface<OptionalTestInterfaceNullableAttribute>().WithPublicProperties();
                s.ExportAsInterface<OptionalTestClassNullableAttribute>().WithPublicProperties();
            }, result);
        }
        
        public interface OptionalTestParameterInterfaceNullableAttribute
        {
	        public int TransformSomeValue(int mode, string? data);
	        public string TransformSomeValueNonNullParameter(int mode, string data);
        }

        [Fact]
        public void AutoOptionalOfParameterIfCompilerNullableAttribute()
        {
	        const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IOptionalTestParameterInterfaceNullableAttribute
	{
		TransformSomeValue(mode: number, data?: string) : number;
		TransformSomeValueNonNullParameter(mode: number, data: string) : string;
	}
}";
	        AssertConfiguration(s =>
	        {
		        s.Global(a => a.DontWriteWarningComment().AutoOptionalProperties());
		        s.ExportAsInterface<OptionalTestParameterInterfaceNullableAttribute>().WithPublicMethods();
	        }, result);
        }
    }
}