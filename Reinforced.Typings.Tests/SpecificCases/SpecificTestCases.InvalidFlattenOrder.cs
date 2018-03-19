using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public class Flat1 { }
    public class Flat2 { }
    public class Flat3 { }
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
                    s.ExportAsInterface<Flat1>()
                        .WithPublicProperties()
                        .FlattenHierarchy();
                }, result);
            });

            Assert.Throws<RtException>(() =>
            {
                AssertConfiguration(s =>
                {
                    s.Global(a => a.DontWriteWarningComment());
                    s.ExportAsInterface<Flat2>()
                        .WithPublicMethods()
                        .FlattenHierarchy();
                }, result);
            });

            Assert.Throws<RtException>(() =>
            {
                AssertConfiguration(s =>
                {
                    s.Global(a => a.DontWriteWarningComment());
                    s.ExportAsInterface<Flat3>()
                        .WithPublicFields()
                        .FlattenHierarchy();
                }, result);
            });
        }
    }
}