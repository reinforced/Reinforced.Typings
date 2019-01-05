using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public interface ITestFlatteningBase
    {
        string SomeProperty { get; }
        void DoSomething();
    }

    public interface ITestFlatteningChild : ITestFlatteningBase
    {

    }

    public abstract class TestFlatteningBase
    {
        public string SomeProperty { get; set; }

        public abstract void DoSomething();
    }

    public class TestFlatteningChild : TestFlatteningBase
    {
        public override void DoSomething()
        {
            
        }
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattening()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IA
	{
		SomeProperty: string;
		DoSomething() : void;
	}
	export interface IB
	{
		SomeProperty: string;
		DoSomething() : void;
	}
}";
            
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestFlatteningChild>().FlattenHierarchy()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .OverrideName("A");

                s.ExportAsInterface<TestFlatteningChild>().FlattenHierarchy()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .OverrideName("B");
            }, result);
        }
    }
}