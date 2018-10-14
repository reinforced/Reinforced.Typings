using System.Collections.Generic;
using System.IO;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ReferencesPart4()
        {
            /**
             * We have removed reference to React and require-import
             * Also we explicitly have added reference to Stuff.ts that exports other types 
             * 
             * File1:
             * Reference to jquery appears as it is explicitly specified via attribute
             * Import to jQuery appears as it has been explecitly specified via attribute
             * Import to React appears as it has been explicitly specified via fluent configuration
             * References to SomeIndirectlyReferencedClass and SomeFluentReferencedType does not appear since
             * we have explicitly mentioned containing file as import.
             * 
             * Warning! This trick works only on RT exported types. If you already have predefined .ts file - it will not work
             * because RT does not know what types are being described there
             * 
             * Refernce to SomeFluentlyReferencedNotExported does not appear as it is not exported entirely
             */

            const string file1 = @"
///<reference path=""../../jquery.d.ts""/>
import * as Stuff from '../Stuff/Stuff';
import * as '$' from 'JQuery';

export class SomeReferencingType
{
	public Indirect() : Stuff.SomeIndirectlyReferencedClass
	{
		return null;
	}
}";
            const string file2 = @"
export class SomeIndirectlyReferencedClass
{
}
export class SomeFluentReferencedType
{
}";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules());

                s.ExportAsClass<SomeReferencingType>()
                    .AddReference(typeof(SomeFluentReferencedType))
                    .AddReference(typeof(SomeFluentlyReferencedNotExported))
                    .AddImport("* as Stuff", "../Stuff/Stuff")
                    .ExportTo("Exported/File1.ts")
                    .WithPublicMethods()
                    ;
                s.ExportAsClass<SomeIndirectlyReferencedClass>().ExportTo("Stuff/Stuff.ts");
                s.ExportAsClass<SomeFluentReferencedType>().ExportTo("Stuff/Stuff.ts");
            }, new Dictionary<string, string>
            {
                { Path.Combine(TargetDir, "Exported", "File1.ts"), file1},
                { Path.Combine(TargetDir, "Stuff", "Stuff.ts"), file2}
            }, compareComments: true);
        }
    }
}