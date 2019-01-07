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
		DateTime: Observable<any>;
		Guid: Observable<string>;
		Int: Observable<number>;
		String: Observable<string>;
		TestInterface: Observable<Reinforced.Typings.Tests.SpecificCases.ITestInterface>;
		SomeMethod1(arg: Observable<number>) : Observable<number>;
		SomeMethod2(arg: Observable<number>) : Observable<number>;
		SomeMethod3(arg: Observable<Reinforced.Typings.Tests.SpecificCases.ITestInterface>) : Observable<Reinforced.Typings.Tests.SpecificCases.ITestInterface>;
		SomeMethod4(arg: Observable<number>) : Observable<number>;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsInterface<ITestInterface>();
                s.ExportAsInterface<IInferringTestInterface>()
                    .WithPublicProperties(x => x.InferType((m, t) => new RtSimpleTypeName("Observable", t.ResolveTypeName(((PropertyInfo)m).PropertyType))))
                    .WithProperty(x => x.Guid, x => x.InferType(_ => "Observable<string>"))
                    .WithProperty(x => x.Int, x => x.InferType(_ => new RtSimpleTypeName("Observable", new RtSimpleTypeName("number"))))
                    .WithProperty(x => x.TestInterface, x => x.InferType((m, r) => new RtSimpleTypeName("Observable", r.ResolveTypeName(typeof(ITestInterface)))))
                    .WithProperty(x => x.DateTime, x => x.InferType((m, r) => string.Format("Observable<{0}>", r.ResolveTypeName(((PropertyInfo)m).PropertyType))))
                    .WithProperty(x => x.String, x => x.InferType(_ => "Observable<string>"))
                    .WithMethod(x => x.SomeMethod1(Ts.Parameter<int>(t => t.InferType(_ => "Observable<number>"))), x => x.InferType(_ => "Observable<number>"))
                    .WithMethod(x => x.SomeMethod2(Ts.Parameter<int>(
                            t => t.InferType(_ => new RtSimpleTypeName("Observable", new RtSimpleTypeName("number"))))),
                            x => x.InferType(_ => new RtSimpleTypeName("Observable", new RtSimpleTypeName("number"))))
                    .WithMethod(x => x.SomeMethod3(Ts.Parameter<int>(t => t.InferType((m, r) => new RtSimpleTypeName("Observable", r.ResolveTypeName(typeof(ITestInterface)))))),
                            x => x.InferType((m, r) => new RtSimpleTypeName("Observable", r.ResolveTypeName(typeof(ITestInterface)))))
                    .WithMethod(x => x.SomeMethod4(Ts.Parameter<int>(t => t.InferType((m, r) => string.Format("Observable<{0}>", r.ResolveTypeName(m.ParameterType))))),
                        x => x.InferType((m, r) => string.Format("Observable<{0}>", r.ResolveTypeName(m.ReturnType))))
                    ;
            }, result);
        }
    }
}