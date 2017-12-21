using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public class FlattenBase<T1, T2, T3>
    {
        public T1 Name { get; set; }

        public T2 Value { get; set; }

        public List<T3> Set { get; set; }
    }

    public interface IViewModel
    {
        string Name { get; }
    }

    public class FlattenChild1 : FlattenBase<int, string, IViewModel>
    {
        
    }

    public class FlattenChild2 : FlattenBase<int, string, FlattenChild2>
    {

    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattening2()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IViewModel
	{
		Name: string;
	}
	export interface IFlattenChild1
	{
		Name: number;
		Value: string;
		Set: Reinforced.Typings.Tests.SpecificCases.IViewModel[];
	}
	export interface IFlattenChild2
	{
		Name: number;
		Value: string;
		Set: Reinforced.Typings.Tests.SpecificCases.IFlattenChild2[];
	}
}";
            
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<IViewModel>()
                    .WithPublicProperties();

                s.ExportAsInterface<FlattenChild1>()
                    .FlattenHierarchy()
                    .WithPublicProperties();

                s.ExportAsInterface<FlattenChild2>()
                    .FlattenHierarchy()
                    .WithPublicProperties();
               

            }, result);
        }
    }
}