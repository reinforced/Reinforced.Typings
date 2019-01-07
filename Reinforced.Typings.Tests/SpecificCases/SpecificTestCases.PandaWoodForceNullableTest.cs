using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region ForceNullable property

        public class PandaWoodForceNullableCase
        {
            [TsProperty(ForceNullable = true)]
            public string PandaWoodProperty { get; set; }
        }

        #endregion

        [Fact]
        public void PandaWoodForceNullableTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
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