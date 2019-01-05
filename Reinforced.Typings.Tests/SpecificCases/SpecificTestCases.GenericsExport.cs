using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void GenericsExport()
        {
            const string result = @"
export interface ISimpleGenericsInterface<T>
{
	Property: T;
	Method(param: T) : T;
}
export interface IParametrizedGenericsInterface<T, T2>
{
	Bag: { [key:T]: T2 };
	Bag2: { [key:T2]: T }[];
	Bag3: { [key:T]: T }[][];
	Bag4: { [key:T]: { [key:T2]: T[][] } };
}
export interface IAttributeParametrization extends ISimpleGenericsInterface<any>
{
}
export interface IChildParametrized<T> extends IParametrizedGenericsInterface<T, number>
{
}
export interface ITriforce<T1, T2, T3>
{
}
export interface ITrimplementor1<TMaster> extends ITriforce<TMaster, TMaster[], { [key:number]: TMaster[] }>
{
}
export interface ITrimplementor2<TMaster, TChild> extends ITriforce<TMaster, TMaster[], { [key:number]: TMaster[] }>
{
	Property: TChild;
}
export interface ITrimplementor3<TMaster, TChild> extends ITriforce<TMaster, TMaster[], { [key:number]: TChild[] }>
{
	Property: [TMaster,TChild];
}
export interface IParametrized
{
	Bag: { [key:number]: string };
	Bag2: { [key:string]: number }[];
	Bag3: { [key:number]: number }[][];
	Bag4: { [key:number]: { [key:string]: number[][] } };
}";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterfaces(new[]
                {
                    typeof(ISimpleGenericsInterface<>),
                    typeof(IParametrizedGenericsInterface<,>),
                    typeof(IAttributeParametrization<>),
                    typeof(IChildParametrized<>),
                    typeof(ITriforce<,,>),
                    typeof(ITrimplementor1<>),
                    typeof(ITrimplementor2<,>),
                    typeof(ITrimplementor3<,>),
                }, x => x.WithPublicProperties().WithPublicMethods());

                s.ExportAsInterfaces(new[] {typeof(IParametrizedGenericsInterface<int, string>)},
                    x => x.WithPublicProperties().WithPublicMethods().OverrideName("Parametrized"));
            }, result);
        }
    }
}