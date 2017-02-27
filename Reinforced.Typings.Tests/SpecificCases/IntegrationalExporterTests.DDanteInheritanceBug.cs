using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void DDanteInheritanceBug()
        {
            const string result = @"
module Pollux.Models {
	export interface IPolluxEntity<Key>
	{
		EntityId: Key;
		CreatedOn: any;
		ModifiedOn?: any;
	}
	export interface IContactData extends Pollux.Models.IPolluxEntity<number>
	{
		Phone: string;
		AlternatePhone: string;
		PhoneConfirmed: boolean;
		Email: string;
		AlternateEmail: string;
		EmailConfirmed: boolean;
		OwnerId: string;
	}
}";
            AssertConfiguration(config =>
            {
                config.Global(a => a.DontWriteWarningComment());
                var polluxBase = typeof(PolluxEntity<>);

                var types = new[] {polluxBase, typeof(ContactData)};
                // stripped to fit the test

                config.ExportAsInterfaces(types, _icb =>
                {
                    _icb.OverrideNamespace("Pollux.Models")
                        .WithPublicProperties();
                });
            }, result);
        }
    }
}