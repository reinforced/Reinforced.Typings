using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{

    public partial class SpecificTestCases
    {
        [Fact]
        public void InvalidFlattenOrder()
        {
            const string result = @"";


            Assert.Throws<RtException>(() =>
            {
                AssertConfiguration(s =>
                {
                    s.Global(a => a.DontWriteWarningComment());
                    s.ExportAsInterface<IViewModel>()
                        .WithPublicProperties();

                    s.ExportAsInterface<IFlattenChild1>()
                        .WithPublicProperties()
                        .FlattenHierarchy();

                    s.ExportAsInterface<IFlattenChild2>()
                        .WithPublicProperties()
                        .FlattenHierarchy();


                }, result);
            });

            
        }
    }
}