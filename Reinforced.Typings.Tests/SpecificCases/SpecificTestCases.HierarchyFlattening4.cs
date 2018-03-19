using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{

    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattening4()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IFlattenChild1
	{
		Name_For_1: number;
	}
	export interface IFlattenChild2
	{
		Name_For_2: number;
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());

                s.ExportAsInterface<IFlattenChild1>()
                    .FlattenHierarchy()
                    .WithProperty(x => x.Name, x => x.OverrideName("Name_For_1"));

                s.ExportAsInterface<IFlattenChild2>()
                    .FlattenHierarchy()
                    .WithProperty(x => x.Name, x => x.OverrideName("Name_For_2"));


            }, result);
        }
    }
}