using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ReferencesPart5()
        {
            /**
             * How let's make some noise with discarding namespaces            
             * 
             * Import tu Stuff appeared as specified it explicitly
             * 
             * SomeIndirectlyReferencedClass exported within its namespace as we are using modules without
             * discarding namespaces
             * 
             * SomeFluentReferencedType is exported without namespace as we have explicitly specified it
             * 
             * Also I've added nullable enum property just in case
             */

            const string file1 = @"
///<reference path=""../../jquery.d.ts""/>
import * as Stuff from '../Stuff/Stuff';
import * as '$' from 'JQuery';

export namespace Reinforced.Typings.Tests.SpecificCases {
	export class AnothreReferencingType
	{
		public Property: Stuff.SomeFluentReferencedType;
		public Enum?: Stuff.SomeIndirectEnum;
		public Indirect() : Stuff.Reinforced.Typings.Tests.SpecificCases.SomeIndirectlyReferencedClass
		{
			return null;
		}
	}
}";
            const string file2 = @"
export namespace Reinforced.Typings.Tests.SpecificCases {
	export class SomeIndirectlyReferencedClass
	{
	}
}
export class SomeFluentReferencedType
{
}
export enum SomeIndirectEnum { 
	One = 0, 
	Two = 1, 
	Three = 2 
}";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules(discardNamespaces: false));

                s.ExportAsClass<AnothreReferencingType>()
                    .AddReference(typeof(SomeFluentReferencedType))
                    .AddReference(typeof(SomeFluentlyReferencedNotExported))
                    .AddImport("* as Stuff", "../Stuff/Stuff")
                    .ExportTo("Exported/File1.ts")
                    .WithPublicMethods()
                    .WithPublicProperties()
                    .WithProperty(c => c.Enum, c => c.ForceNullable())
                    ;
                s.ExportAsClass<SomeIndirectlyReferencedClass>().ExportTo("Stuff/Stuff.ts");
                s.ExportAsClass<SomeFluentReferencedType>().DontIncludeToNamespace().ExportTo("Stuff/Stuff.ts");
                s.ExportAsEnum<SomeIndirectEnum>().DontIncludeToNamespace().ExportTo("Stuff/Stuff.ts");
            }, new Dictionary<string, string>()
            {
                {"D:\\Exported\\File1.ts", file1},
                {"D:\\Stuff\\Stuff.ts", file2},
            }, compareComments: true);
        }
    }
}