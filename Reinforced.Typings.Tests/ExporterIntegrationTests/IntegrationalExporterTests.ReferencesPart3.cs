using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void ReferencesPart3()
        {
            /**
             * Here code significantly changed since we have switched to modules
             * File1:
             * Reference to File3 appears as import since we are using modules
             * Reference to jquery appears as it is explicitly specified via attribute
             * Import to jQuery appears as it has been explecitly specified via attribute
             * Import to React appears as it has been explicitly specified via fluent configuration
             * Required-import to sideeffects appears as it was explicitly specified in fluent configuration
             * 
             * Reference to ../Indirect/File2 appears as we are exporting method "Indirect" returning SomeIndirectlyReferencedClass
             * 
             * Refernce to SomeFluentlyReferencedNotExported does not appear as it is not exported entirely
             */

            const string file1 = @"
///<reference path=""../../jquery.d.ts""/>
import { SomeFluentReferencedType } from '../Fluently/File3';
import * as '$' from 'JQuery';
import * as React from 'React';
import sideeffects = require('./sideeffects');
import { SomeIndirectlyReferencedClass } from '../Indirect/File2';

export class SomeReferencingType
{
	public Indirect() : SomeIndirectlyReferencedClass
	{
		return null;
	}
}";
            const string file2 = @"
export class SomeIndirectlyReferencedClass
{
}";
            const string file3 = @"
export class SomeFluentReferencedType
{
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment()
                        .UseModules() //<--- this line differs from references part 2
                );
                s.ExportAsClass<SomeReferencingType>()
                    .AddReference(typeof(SomeFluentReferencedType))
                    .AddReference(typeof(SomeFluentlyReferencedNotExported))
                    .AddImport("* as React", "React")
                    .ExportTo("Exported/File1.ts")
                    .AddImport("sideeffects", "./sideeffects", true)
                    .WithPublicMethods()
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