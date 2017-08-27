using System.Reflection;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void InferTypeTest()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface ITestInterface
	{
	}
	export interface IInferringTestInterface
	{
		String: Observable<string>;
		Int: Observable<number>;
		Guid: Observable<string>;
		DateTime: Observable<any>;
		TestInterface: Observable<Reinforced.Typings.Tests.SpecificCases.ITestInterface>;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>();
                s.ExportAsInterface<IInferringTestInterface>()
                    .WithPublicProperties()
                    .WithProperty(x => x.Guid, x => x.InferType(_ => "Observable<string>"))
                    .WithProperty(x => x.Int, x => x.InferType(_ => new RtSimpleTypeName("Observable", new RtSimpleTypeName("number"))))
                    .WithProperty(x => x.TestInterface, x => x.InferType((m, r) => new RtSimpleTypeName("Observable", r.ResolveTypeName(typeof(ITestInterface)))))
                    .WithProperty(x => x.DateTime, x => x.InferType((m, r) => string.Format("Observable<{0}>", r.ResolveTypeName(((PropertyInfo)m).PropertyType))))
                    .WithProperty(x => x.String, x => x.InferType(_ => "Observable<string>"))
                    ;
            }, result);
        }
    }
}