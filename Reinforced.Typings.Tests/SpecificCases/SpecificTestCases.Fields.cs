using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
	    class TestExportWithFields
	    {
		    public string StringField;
		    public int NumberField;
	    }

	    [Fact]
        public void FieldsWithBuilderConfig()
        {
            const string result = @"
				class TestExportWithFields
					{
						public StringField: string;
						public NumberField: number;
					}
				";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<TestExportWithFields>()
	                .WithPublicFields((config) =>
	                {
		                var fieldInfo = config.Member;
	                })
	                .DontIncludeToNamespace()
	            ;
            }, result);
        }
    }
}