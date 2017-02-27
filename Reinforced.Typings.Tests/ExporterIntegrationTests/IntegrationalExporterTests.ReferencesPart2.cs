using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void ReferencesPart2()
        {
            /**
             * File1:
             * Reference to File3 appears as it is explicitly specified: .AddReference(typeof(SomeFluentReferencedType))
             * Reference to jquery appears as it is explicitly specified via attribute
             * Imports does not appear as we are not using modules
             * Reference to File2 appears as we are exporting method "Indirect" returning SomeIndirectlyReferencedClass
             * Refernce to SomeFluentlyReferencedNotExported does not appear as it is not exported entirely
             */

            const string file1 = @"
///<reference path=""../../jquery.d.ts""/>
///<reference path=""../Fluently/File3.ts""/>
///<reference path=""../Indirect/File2.ts""/>

module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export class SomeReferencingType
	{
		public Indirect() : Reinforced.Typings.Tests.ExporterIntegrationTests.SomeIndirectlyReferencedClass
		{
			return null;
		}
	}
}";
            const string file2 = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export class SomeIndirectlyReferencedClass
	{
	}
}";
            const string file3 = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export class SomeFluentReferencedType
	{
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsClass<SomeReferencingType>()
                    .AddReference(typeof(SomeFluentReferencedType))
                    .AddReference(typeof(SomeFluentlyReferencedNotExported))
                    .AddImport("* as React", "React")
                    .ExportTo("Exported/File1.ts")
                    .AddImport("sideeffects", "./sideeffects", true)
                    .WithPublicMethods() //<--- this line differs from references part 1
                    ;
                s.ExportAsClass<SomeIndirectlyReferencedClass>().ExportTo("Indirect/File2.ts");
                s.ExportAsClass<SomeFluentReferencedType>().ExportTo("Fluently/File3.ts");
            }, new Dictionary<string, string>()
            {
                {"D:\\Exported\\File1.ts", file1},
                {"D:\\Indirect\\File2.ts", file2},
                {"D:\\Fluently\\File3.ts", file3},
            }, compareComments: true);
        }
    }
}