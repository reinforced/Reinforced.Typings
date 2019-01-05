using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    class GenericModel<TModel>
    {
        
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void GenericsExport2()
        {
            const string result = @"
export class GenericModel<TModel>
{
}
";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsClasses(new[]
                {
                    typeof(GenericModel<>)
                }, x => x.DontIncludeToNamespace<ClassExportBuilder>().WithPublicProperties().WithPublicMethods());
                
            }, result);
        }
    }
}