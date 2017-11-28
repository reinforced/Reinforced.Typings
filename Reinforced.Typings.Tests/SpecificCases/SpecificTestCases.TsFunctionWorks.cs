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
        public void TsFunctionWorks()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IMyFunctionTestClass
	{
		MyProperty: string;
		MyNumber: number;
		doSomething1(a: number) : string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(x=>x.DontWriteWarningComment());
                s.ExportAsInterface<MyFunctionTestClass>()
                .WithPublicMethods(x=>x.CamelCase())
                .WithPublicProperties();
            }, result);
        }
    }

    #region vmandic test with possibly not working TsProperty

    public class MyFunctionTestClass
    {
        
        public string MyProperty { get; set; }
        
        public int MyNumber { get; set; }

        [TestFunction]
        public string DoSomething(int a)
        {
            return string.Empty;
        }
    }

    public class TestFunctionAttribute : TsFunctionAttribute
    {
        public TestFunctionAttribute()
        {
            CodeGeneratorType = typeof(TestFunctionGenerator);
        }
    }

    public class TestFunctionGenerator : MethodCodeGenerator
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