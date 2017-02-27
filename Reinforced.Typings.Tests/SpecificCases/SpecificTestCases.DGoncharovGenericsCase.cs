using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void DGoncharovGenericsCase()
        {
            const string result = @"
export interface ITypedBasicResult<T>
{
	Status: number;
	Message: string;
	Data: T;
}
export interface ISelectListItem
{
	Text: string;
	Value: string;
}
export class RequestHandler
{
	public DoRequest() : ITypedBasicResult<ISelectListItem[]>
	{
		return null;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());
                s.ExportAsInterfaces(new[] {typeof(TypedBasicResult<>)}, x => x.WithPublicProperties());
                s.ExportAsInterface<SelectListItem>().WithPublicProperties();
                s.ExportAsClass<RequestHandler>().WithPublicMethods();
            }, result);
        }
    }
}