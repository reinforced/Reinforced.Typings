using Reinforced.Typings.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        interface IUnresolvedToUnknownInterface
        {
            DateTime? Date { get; }
        }

        [Fact]
        public void UnresolvedToUnknown()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IUnresolvedToUnknownInterface
	{
		Date: unknown;
	}
}";

            AssertConfiguration(s =>
            {
                s.Global(c => c.UnresolvedToUnknown(true));
                s.ExportAsInterface<IUnresolvedToUnknownInterface>().WithPublicProperties();
            }, result);
        }
    }
}
