using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public class FlatternBase<T1, T2, T3>
    {
        public T1 Name { get; set; }

        public T2 Value { get; set; }

        public List<T3> Set { get; set; }
    }

    public interface IViewModel
    {
        string Name { get; }
    }

    public class FlatternChild1 : FlatternBase<int, string, IViewModel>
    {
        
    }

    public class FlatternChild2 : FlatternBase<int, string, FlatternChild2>
    {

    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattering2()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IViewModel
	{
		Name: string;
	}
	export interface IFlatternChild1
	{
		Name: number;
		Value: string;
		Set: Reinforced.Typings.Tests.SpecificCases.IViewModel[];
	}
	export interface IFlatternChild2
	{
		Name: number;
		Value: string;
		Set: Reinforced.Typings.Tests.SpecificCases.IFlatternChild2[];
	}
}";
            
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<IViewModel>()
                    .WithPublicProperties();

                s.ExportAsInterface<FlatternChild1>()
                    .FlatternHierarchy()
                    .WithPublicProperties();

                s.ExportAsInterface<FlatternChild2>()
                    .FlatternHierarchy()
                    .WithPublicProperties();
               

            }, result);
        }
    }
}