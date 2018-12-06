using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void EnumKeyedDictionary()
        {
            const string result = @"
export enum SomeEnum { 
	One = 0, 
	Two = 1
}
export interface IParametrizedGenericsInterface
{
	Bag: { [key in SomeEnum]: string };
	Bag2: { [key:string]: SomeEnum }[];
	Bag3: { [key in SomeEnum]: SomeEnum }[][];
	Bag4: { [key in SomeEnum]: { [key:string]: SomeEnum[][] } };
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsEnum<SomeEnum>();
                s.ExportAsInterfaces(
                    new[] {typeof(IParametrizedGenericsInterface<SomeEnum, string>)},
                    x => x.WithPublicProperties().WithPublicMethods());
            }, result);
        }
    }
}