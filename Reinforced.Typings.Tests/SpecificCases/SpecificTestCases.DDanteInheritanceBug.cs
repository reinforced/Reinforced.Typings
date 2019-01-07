using System;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        #region DDante case

        public abstract class PolluxEntity<Key>
        {
            public Key EntityId { get; set; }

            public DateTime CreatedOn { get; set; }

            public DateTime? ModifiedOn { get; set; }


            public PolluxEntity()
            {
                this.CreatedOn = DateTime.Now;
            }
        }


        public class ContactData : PolluxEntity<long>
        {
            public string Phone { get; set; }
            public string AlternatePhone { get; set; }
            public bool PhoneConfirmed { get; set; }

            public string Email { get; set; }
            public string AlternateEmail { get; set; }
            public bool EmailConfirmed { get; set; }

            public virtual string OwnerId { get; set; }

            public ContactData()
            {
            }
        }

        #endregion

        [Fact]
        public void DDanteInheritanceBug()
        {
            const string result = @"
module Pollux.Models {
	export interface IPolluxEntity<Key>
	{
		CreatedOn: any;
		EntityId: Key;
		ModifiedOn?: any;
	}
	export interface IContactData extends Pollux.Models.IPolluxEntity<number>
	{
		AlternateEmail: string;
		AlternatePhone: string;
		Email: string;
		EmailConfirmed: boolean;
		OwnerId: string;
		Phone: string;
		PhoneConfirmed: boolean;
	}
}";
            AssertConfiguration(config =>
            {
                config.Global(a => a.DontWriteWarningComment().ReorderMembers());
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