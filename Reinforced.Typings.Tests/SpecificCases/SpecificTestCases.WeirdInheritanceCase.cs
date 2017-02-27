using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void WeirdInheritanceCase()
        {
            const string result = @"
export interface IBaseClass
{
	Property: string;
	DoSomething() : void;
}
/**
* @todo Automatically implemented from IBaseClass
*/
export class DerivedClass implements IBaseClass
{
	public GetName() : string
	{
		return null;
	}
	public Property: string;
	public DoSomething() : void { } 
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterface<BaseClass>().WithPublicProperties().WithPublicMethods();
                s.ExportAsClass<DerivedClass>().WithPublicProperties().WithPublicMethods();
            }, result, compareComments: true);
        }
    }
}