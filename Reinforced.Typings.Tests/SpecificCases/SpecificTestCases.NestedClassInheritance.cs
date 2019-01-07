using Reinforced.Typings.Fluent;
using Xunit;

namespace TestA
{
    public class TestParentClassA
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}

namespace TestB
{
    using TestA;
    public class SomeOtherClass
    {
        public class SomeDerivedClass : TestParentClassA
        {
            public int OtherId { get; set; }
        }
    }
}

namespace Reinforced.Typings.Tests.SpecificCases
{

    public partial class SpecificTestCases
    {
        [Fact]
        public void NestedClassInheritance()
        {
            const string result = @"
module TestB {
	export interface ISomeDerivedClass extends TestB.ITestParentClassA
	{
		OtherId: number;
	}
	export interface ITestParentClassA
	{
		id: string;
		text: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsInterface<TestB.SomeOtherClass.SomeDerivedClass>()
                    .WithPublicProperties();
                s.ExportAsInterface<TestA.TestParentClassA>()
                    .WithProperty(c => c.Id, v => v.OverrideName("id"))
                    .WithProperty(c => c.Name, v => v.OverrideName("text"))
                    .OverrideNamespace("TestB");
            }, result);
        }
    }
}