using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region DGoncharovGenericsTestCase

        public class SelectListItem
        {
            public string Text { get; set; }

            public string Value { get; set; }
        }

        public class TypedBasicResult<T>
        {
            public int Status { get; set; }

            public string Message { get; set; }

            public T Data { get; set; }
        }

        public class RequestHandler
        {
            public TypedBasicResult<IEnumerable<SelectListItem>> DoRequest()
            {
                return null;
            }
        }

        #endregion

        [Fact]
        public void DGoncharovGenericsCase()
        {
            const string result = @"
export interface ITypedBasicResult<T>
{
	Data: T;
	Message: string;
	Status: number;
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
                s.Global(a => a.DontWriteWarningComment().UseModules().ReorderMembers());
                s.ExportAsInterfaces(new[] {typeof(TypedBasicResult<>)}, x => x.WithPublicProperties());
                s.ExportAsInterface<SelectListItem>().WithPublicProperties();
                s.ExportAsClass<RequestHandler>().WithPublicMethods();
            }, result);
        }
    }
}