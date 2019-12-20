using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    class ExplicitGeneric<T>
    {
        
    }

    class SomeOther
    {
        public ExplicitGeneric<object> Prop { get; }

        public ExplicitGeneric<string> Prop2 { get; }
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void GenericsExport3()
        {
            /*
             * Here we export type with materialized generics explicitly. So ExplicitGeneric<object> becomes known type, but
             * generic definition ExpicitGeneric<T> was exported differently so there are 2 different types
             */

            const string result = @"
export interface IExplicitGeneric
{
}
export interface ISomeOther
{
	Prop: IExplicitGeneric;
	Prop2: ICompletelyDifferentType<string>;
}
export interface ICompletelyDifferentType<T>
{
}
";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterfaces(new[]
                {
                    typeof(ExplicitGeneric<object>),
                    typeof(SomeOther),
                }, x => x.WithPublicProperties().WithPublicMethods());
                s.ExportAsInterfaces(new [] {typeof(ExplicitGeneric<>)},d=>d.WithPublicProperties().OverrideName("CompletelyDifferentType"));
            }, result);
        }
    }
}