using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        class A
        {
            public IDictionary<Guid, String> GuidDictionary { get; set; }
        }

        [Fact]
        public void GenericGuidDictionary()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IA
	{
        GuidDictionary: { [key:string]: string };
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
                s.ExportAsInterface<A>().WithPublicProperties();

            }, result, exp =>
            {
                var warnings = exp.Context.Warnings.Count();
                Assert.Equal(0, warnings);
            });
        }
    }
}
