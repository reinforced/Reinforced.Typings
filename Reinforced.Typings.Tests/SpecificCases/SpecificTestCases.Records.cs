using System.Threading.Tasks;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public record Person(string FirstName, string LastName);

    public partial class SpecificTestCases
    {
        

        [Fact]
        public void Records()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IPerson
	{
		FirstName: string;
		LastName: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<Person>().WithPublicProperties();
            }, result);
        }
    }
}