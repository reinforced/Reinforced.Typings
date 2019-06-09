using Reinforced.Typings.Fluent;
using Xunit;

namespace TypingsExample.Long.Namespace.Structure
{
    class MyClass
    {
        public MyProperty Property { get; set; }
    }

    class MyProperty
    {
        public string Name { get; set; }
    }
}
namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void OverrideNamespaceWithModules()
        {
            const string result = @"
export namespace server {
	export class MyClass
	{
		public Property: server.MyProperty;
	}
	export class MyProperty
	{
		public Name: string;
	}
}";
            var types = new[]
            {
                typeof(TypingsExample.Long.Namespace.Structure.MyClass),
                typeof(TypingsExample.Long.Namespace.Structure.MyProperty)
            };
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules(true, false));
                s.ExportAsClasses(
                    types,
                    conf => conf
                        .OverrideNamespace("server")
                        .WithAllProperties()
                );
            }, result);
        }
    }
}