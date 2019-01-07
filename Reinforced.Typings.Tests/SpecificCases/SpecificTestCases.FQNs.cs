using System.Linq;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    namespace FqnTest.Ns1
    {
        public class FqnClass1
        {
            public int AnIntegerPropNs1 { get; set; }
            public string AStringPropNs1 { get; set; }
        }
    }

    namespace FqnTest.Ns2
    {
        using FqnTest.Ns1;
        public class FqnClass2 : FqnClass1
        {
            public int AnIntegerPropNs2 { get; set; }
            public string AStringPropNs2 { get; set; }
        }
    }

    namespace FqnTest.Ns3
    {
        using FqnTest.Ns1;
        public class FqnClass3
        {
            public int AnIntegerPropNs3 { get; set; }
            public string AStringPropNs3 { get; set; }
            public FqnClass1 AClass1PropNs3 { get; set; }
        }
    }


    public partial class SpecificTestCases
    {
        [Fact]
        public void FQNs()
        {
            const string result = @"
export namespace Reinforced.Typings.Tests.SpecificCases.FqnTest.Ns1 {
	export class FqnClass1
	{
		public AnIntegerPropNs1: number;
		public AStringPropNs1: string;
	}
}
export namespace Reinforced.Typings.Tests.SpecificCases.FqnTest.Ns2 {
	export class FqnClass2 extends Reinforced.Typings.Tests.SpecificCases.FqnTest.Ns1.FqnClass1
	{
		public AnIntegerPropNs2: number;
		public AStringPropNs2: string;
	}
}
export namespace Reinforced.Typings.Tests.SpecificCases.FqnTest.Ns3 {
	export class FqnClass3
	{
		public AClass1PropNs3: Reinforced.Typings.Tests.SpecificCases.FqnTest.Ns1.FqnClass1;
		public AnIntegerPropNs3: number;
		public AStringPropNs3: string;
	}
}
";
            var types =new[] { typeof(FqnTest.Ns1.FqnClass1), typeof(FqnTest.Ns2.FqnClass2), typeof(FqnTest.Ns3.FqnClass3) };

            AssertConfiguration(s =>
            {
                s.ExportAsClasses(types.Where(x => x.IsClass), c => c.WithAllFields().WithAllMethods().WithAllProperties());
                s.Global(c => c.UseModules(true, false).DontWriteWarningComment().ReorderMembers());
            }, result);
        }
    }
}