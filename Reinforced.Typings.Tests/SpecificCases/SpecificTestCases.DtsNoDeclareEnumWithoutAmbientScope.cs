using Reinforced.Typings.Fluent;
using Xunit;

namespace Enum.Test
{
    public enum Version
    {
        Current = 1
    }
}

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void DtsNoDeclareEnumWithoutAmbientScope()
        {
            const string result = @"
declare module Enum.Test {
	export const enum Version { 
		Current = 1
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules(false, false)
                    .ExportPureTypings(true));
                s.ExportAsEnum<Enum.Test.Version>().Const();
            }, result);
        }
    }
}