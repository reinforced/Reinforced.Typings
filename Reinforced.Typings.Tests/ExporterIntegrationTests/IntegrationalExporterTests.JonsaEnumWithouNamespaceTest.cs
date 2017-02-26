using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void JonsaEnumWithouNamespaceTest()
        {
            const string result = @"
interface IJonsaModel
{
	Enum: JonsaEnum;
}
enum JonsaEnum { 
	Foo = 0, 
	Bar = 1, 
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<JonsaModel>().WithPublicProperties().DontIncludeToNamespace();
                s.ExportAsEnum<JonsaEnum>().DontIncludeToNamespace();
            }, result);
        }
    }
}