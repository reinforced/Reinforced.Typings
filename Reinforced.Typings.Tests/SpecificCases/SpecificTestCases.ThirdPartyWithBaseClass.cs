using System;
using System.Collections.Generic;
using System.IO;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.ReferencesInspection;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        class ThirdPartyBaseClass
        {
            public string SomeProperty { get; set; }
        }

        class ExportedClass : ThirdPartyBaseClass
        {
            public int Property { get; set; }
            public string GenericProperty1 { get; set; }
        }

        [Fact]
        public void ExportAsInterfaceWithThirdPartyBaseClass()
        {
            const string file1 = @"import { IThirdPartyBaseInterface } from 'third-party';

export interface IExportedClass extends IThirdPartyBaseInterface
{
  GenericProperty1: string;
  Property: number;
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment()
                    .UseModules()
                    .ReorderMembers());

                s.ExportAsThirdParty<ThirdPartyBaseClass>()
                    .WithName("IThirdPartyBaseInterface")
                    .Imports(new RtImport()
                    {
                        From = "third-party",
                        Target = "{ IThirdPartyBaseInterface }"
                    });

                s.ExportAsInterface<ExportedClass>().WithPublicProperties();
            }, file1);
        }

        [Fact]
        public void ExportAsClassWithThirdPartyBaseClass()
        {
            const string file1 = @"import { ThirdPartyBaseClass } from 'third-party';

export class ExportedClass extends ThirdPartyBaseClass
{
	public GenericProperty1: string;
	public Property: number;
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment()
                    .UseModules()
                    .ReorderMembers());

                s.ExportAsThirdParty<ThirdPartyBaseClass>()
                    .WithName("ThirdPartyBaseClass")
                    .Imports(new RtImport()
                    {
                        From = "third-party",
                        Target = "{ ThirdPartyBaseClass }"
                    });

                s.ExportAsClass<ExportedClass>().WithPublicProperties();
            }, file1);
        }
    }
}