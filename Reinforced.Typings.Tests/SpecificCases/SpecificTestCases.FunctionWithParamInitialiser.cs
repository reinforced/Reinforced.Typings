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
        private class MyFunctionTestClassWithParamInitializer
        {
        
            public string MyProperty { get; set; }
        
            public int MyNumber { get; set; }

            public string DoSomething(int a = 4)
            {
                return a == 4 ? "Hello World" : string.Empty;
            }
        }

        [Fact]
        public void FunctionWithParamInitializerAsInterface()
        {
            const string result = @"
                module Reinforced.Typings.Tests.SpecificCases {
	                export interface IMyFunctionTestClassWithParamInitializer
	                {
		                MyProperty: string;
		                MyNumber: number;
		                doSomething(a?: number) : string;
	                }
                }
            ";

            AssertConfiguration(s =>
            {
                s.Global(x=>x.DontWriteWarningComment());
                s.ExportAsInterface<MyFunctionTestClassWithParamInitializer>()
                .WithPublicMethods(x=>x.CamelCase())
                .WithPublicProperties();
            }, result);
        }

        [Fact]
        public void FunctionWithParamInitializerAsClass()
        {
            const string result = @"
                module Reinforced.Typings.Tests.SpecificCases {
	                export class MyFunctionTestClassWithParamInitializer
	                {
		                public MyProperty: string;
		                public MyNumber: number;
		                public doSomething(a: number = 4) : string
		                {
			                return null;
		                }
	                }
                }
                ";
            AssertConfiguration(s =>
            {
                s.Global(x=>x.DontWriteWarningComment());
                s.ExportAsClass<MyFunctionTestClassWithParamInitializer>()
                    .WithPublicMethods(x=>x.CamelCase())
                    .WithPublicProperties();
            }, result);
        }
    }
}