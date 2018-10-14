using System.Collections.Generic;
using System.IO;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
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
///<reference path=""../../bootstrap.d.ts""/>
///<reference path=""../../jquery.d.ts""/>
import * as 'react' from 'React';
import { SomeFluentReferencedType } from '../Fluently/File3';
import * as '$' from 'JQuery';
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
///<reference path=""../../bootstrap.d.ts""/>
import * as 'react' from 'React';

export class SomeIndirectlyReferencedClass
{
}";
            const string file3 = @"
///<reference path=""../../bootstrap.d.ts""/>
import * as 'react' from 'React';

export class SomeFluentReferencedType
{
}";
            AssertConfiguration(s =>
            {
                s.AddReference("../../bootstrap.d.ts");
                s.AddImport("* as 'react'", "React");
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
            }, new Dictionary<string, string>
            {
                { Path.Combine(TargetDir, "Exported", "File1.ts"), file1 },
                { Path.Combine(TargetDir, "Indirect", "File2.ts"), file2 },
                { Path.Combine(TargetDir, "Fluently", "File3.ts"), file3 }
            }, compareComments: true);
        }
    }
}