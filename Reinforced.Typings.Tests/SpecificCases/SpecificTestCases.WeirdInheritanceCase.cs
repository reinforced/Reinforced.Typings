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
	public Property: string;
	public DoSomething() : void { } 
	public GetName() : string
	{
		return null;
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules().ReorderMembers());
                s.ExportAsInterface<BaseClass>().WithPublicProperties().WithPublicMethods();
                s.ExportAsClass<DerivedClass>().WithPublicProperties().WithPublicMethods();
            }, result, compareComments: true);
        }
    }
}