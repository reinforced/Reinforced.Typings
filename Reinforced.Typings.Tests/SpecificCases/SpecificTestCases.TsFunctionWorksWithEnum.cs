using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void TsFunctionWorksWithEnum()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IMyFunctionWithEnumTestClass
	{
		MyProperty: string;
		MyNumber: number;
		MyEnum: TestEnum;
		doSomething1(a: number) : string;
		doSomethingForEnum1(a: TestEnum) : TestEnum;
		doSomethingForEnumWithDefault1(a: TestEnum = TestEnum.One) : TestEnum;
	}
}
enum TestEnum { 
	One = 0
}";
            AssertConfiguration(s =>
            {
                s.Global(x => x.DontWriteWarningComment());
                s.ExportAsInterface<MyFunctionWithEnumTestClass>()
                  .WithPublicMethods(x => x.CamelCase())
                  .WithPublicProperties();
                s.ExportAsEnum<TestEnum>()
                  .DontIncludeToNamespace();
            }, result);
        }
    }

    #region vmandic test with possibly not working TsProperty



    public class MyFunctionWithEnumTestClass
    {

        public string MyProperty { get; set; }

        public int MyNumber { get; set; }

        public TestEnum MyEnum { get; set; }

        [TestFunction]
        public string DoSomething(int a)
        {
            return string.Empty;
        }

        [TestFunction]
        public TestEnum DoSomethingForEnum(TestEnum a)
        {
            return MyEnum;
        }

        [TestFunction]
        public TestEnum DoSomethingForEnumWithDefault(TestEnum a = TestEnum.One)
        {
            this.MyEnum = a;
            return MyEnum;
        }
    }
    public enum TestEnum
    {
        One
    }

    public class TestFunctionWithEnumAttribute : TsFunctionAttribute
    {
        public TestFunctionWithEnumAttribute()
        {
            CodeGeneratorType = typeof(TestFunctionWithEnumAttribute);
        }
    }

    public class TestFunctionWithEnumGenerator : MethodCodeGenerator
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtFuncion GenerateNode(MethodInfo element, RtFuncion result, TypeResolver resolver)
        {
            var b = base.GenerateNode(element, result, resolver);
            if (b == null) return null;
            result.Identifier.IdentifierName = result.Identifier.IdentifierName + "1";
            return b;
        }
    }
    #endregion
}