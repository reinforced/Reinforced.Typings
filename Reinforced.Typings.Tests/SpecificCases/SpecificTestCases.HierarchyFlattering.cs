using System.Reflection;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public interface ITestFlatteringBase
    {
        string SomeProperty { get; }
        void DoSomething();
    }

    public interface ITestFlatteringChild : ITestFlatteringBase
    {

    }

    public abstract class TestFlatteringBase
    {
        public string SomeProperty { get; set; }

        public abstract void DoSomething();
    }

    public class TestFlatteringChild : TestFlatteringBase
    {
        public override void DoSomething()
        {
            
        }
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void HierarchyFlattering()
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
                s.ExportAsInterface<ITestFlatteringChild>().FlatternHierarchy()
                    .OverrideName("A")
                    .WithPublicProperties()
                    .WithPublicMethods();

                s.ExportAsInterface<TestFlatteringChild>().FlatternHierarchy()
                    .OverrideName("B")
                    .WithPublicProperties()
                    .WithPublicMethods();
            }, result);
        }
    }
}