using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    enum ExportedEnum
    {
        One = 1
    }

    enum NotExportedEnum
    {
        Two = 2
    }
    class ConstantTestA
    {
        public const string ConstantString = "a";
        public static string StaticString = "b";

        public static int StaticNumber = 42;
        public const int ConstantNumber = 42;

        public const ExportedEnum ConstExEnum = ExportedEnum.One;
        public static ExportedEnum StaticExEnum = ExportedEnum.One;

        public const NotExportedEnum ConstNotExEnum = NotExportedEnum.Two;
        public static NotExportedEnum StaticNotExEnum = NotExportedEnum.Two;

    }
    class ConstantTestB
    {
        public static string StaticString = "b";
        public object MyObject { get; set; }
    }

    class ConstantTestC
    {
        public const string WillBeInitialized = "a";
        public static string WillNotBeInitialized = "b";
    }

    public partial class SpecificTestCases
    {
        [Fact]
        public void ConstantProperties()
        {
	        // NB this test fails on Mono because the Members property of RtClass does not 
	        // order the tokens in the same order as the CLR on Windows (ie it's CLR-specific, relying on a certain order)
	        // (Similar to the way that the order of attributes in C# code is CLR-dependent and shouldn't be relied on)
            const string result = @"
module Test {
	export enum ExportedEnum { 
		One = 1
	}
	export class ConstantTestA
	{
		public static StaticString: string = 'b';
		public static StaticNumber: number = 42;
		public static StaticExEnum: Test.ExportedEnum = Test.ExportedEnum.One;
		public static StaticNotExEnum: number = 2;
		public static ConstantString: string = 'a';
		public static ConstantNumber: number = 42;
		public static ConstExEnum: Test.ExportedEnum = Test.ExportedEnum.One;
		public static ConstNotExEnum: number = 2;
	}
	export class ConstantTestB
	{
		public static StaticString: string = 'Hello, I\'m string that originally was \'b\'';
		public MyObject: any = { a: 10, b: 5 };
	}
	export class ConstantTestC
	{
		public static WillNotBeInitialized: string;
		public static WillBeInitialized: string = 'a';
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsEnum<ExportedEnum>().OverrideNamespace("Test");
                s.ExportAsClass<ConstantTestA>()
                    .WithAllFields()
                    .WithAllProperties().OverrideNamespace("Test");

                s.ExportAsClass<ConstantTestB>()
                    .OverrideNamespace("Test")
                    .WithProperty(x => x.MyObject, x => x.InitializeWith((m, tr, v) => "{ a: 10, b: 5 }"))
                    .WithField("StaticString",
                        x => x.InitializeWith((m, tr, v) => $"'Hello, I\\'m string that originally was \\'{v}\\''"));

                s.ExportAsClass<ConstantTestC>()
                    .WithPublicFields()
                    .OverrideNamespace("Test")
                    .WithField("WillNotBeInitialized", c => c.Constant(false));
            }, result);
        }
    }
}