using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public interface IFlattenBase<T1, T2, T3>
    {
        T1 Name { get; set; }

        T2 Value { get; set; }

        List<T3> Set { get; set; }
    }

    public interface IFlattenBase2
    {
        int Id { get; set; }
    }

    public interface IViewModel2
    {
        string Name { get; }
    }

    public interface IFlattenChild1 : IFlattenBase<int, string, IViewModel>, IFlattenBase2
    {
        
    }

    public interface IFlattenChild2 : IFlattenBase<int, string, IFlattenChild2>, IFlattenBase2
    {

    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattening3()
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
		Id: number;
	}
	export interface IFlattenChild2
	{
		Name: number;
		Value: string;
		Set: Reinforced.Typings.Tests.SpecificCases.IFlattenChild2[];
		Id: number;
	}
}";
            
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<IViewModel>()
                    .WithPublicProperties();

                s.ExportAsInterface<IFlattenChild1>()
                    .FlattenHierarchy()
                    .WithPublicProperties();

                s.ExportAsInterface<IFlattenChild2>()
                    .FlattenHierarchy()
                    .WithPublicProperties();
               

            }, result);
        }
    }
}