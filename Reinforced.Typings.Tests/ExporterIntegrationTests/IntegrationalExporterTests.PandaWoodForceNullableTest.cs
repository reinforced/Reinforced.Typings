using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void PandaWoodForceNullableTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export class PandaWoodForceNullableCase
	{
		public PandaWoodProperty?: string;		
	}	  
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<PandaWoodForceNullableCase>();
            }, result);
        }
    }
}