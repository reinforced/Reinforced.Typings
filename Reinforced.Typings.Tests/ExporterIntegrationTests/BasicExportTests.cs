using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Core;
using Xunit;
using Xunit.Abstractions;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public class InterfaceExporterTests : RtExporterTestBase
    {
        [Fact]
        public void InterfaceAsInterface()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export interface ITestInterface
	{
		String: string;
		Int: number;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }

        [Fact]
        public void ExportOrderTest1()
        {
            const string result1 = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
    export interface ITestClass
	{
		String: string;
		Int: number;
	}	
    export interface ITestInterface
	{
		String: string;
		Int: number;
	}    
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
                s.ExportAsInterface<TestClass>().WithPublicProperties().Order(-1);
            }, result1);
        }

        [Fact]
        public void ExportOrderTest2()
        {
            const string result2 = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
    export interface ITestInterface
	{
		String: string;
		Int: number;
	}  
    export interface ITestClass
	{
		String: string;
		Int: number;
	}	  
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
                s.ExportAsInterface<TestClass>().WithPublicProperties();
            }, result2);
        }

        [Fact]
        public void DecoratorsTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	@sealed export class ClassWithMethods
	{
		@bind public String: string;
		@bind public Int: number;
		@a() @b() public DoSomethinig() : void { } 
	}	  
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<ClassWithMethods>().Decorator("sealed")
                    .WithPublicProperties(a=>a.Decorator("bind"))
                    .WithMethod(c => c.DoSomethinig(), c => c.Decorator("a()", -1))
                    ;
            }, result);
        }
    }
}
