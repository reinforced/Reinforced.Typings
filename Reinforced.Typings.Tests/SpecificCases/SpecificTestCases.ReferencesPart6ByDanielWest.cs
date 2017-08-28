using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ReferencesPart6ByDanielWest()
        {
            /**
             * Specific test case with equal folder names by Daniel West
             */

            const string file1 = @"
export namespace Reinforced.Typings.Tests.SpecificCases {
	export enum SomeEnum { 
		One = 0, 
		Two = 1, 
	}
}";
            const string file2 = @"
import * as Enum from '../../APIv2/Models/TimeAndAttendance/Enum';

export namespace Reinforced.Typings.Tests.SpecificCases {
	export class SomeViewModel
	{
		public Enum: Enum.Reinforced.Typings.Tests.SpecificCases.SomeEnum;
	}
}";

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules(discardNamespaces: false));

                s.ExportAsEnum<SomeEnum>().ExportTo("Areas/APIv2/Models/TimeAndAttendance/Enum.ts");
                s.ExportAsClass<SomeViewModel>().WithPublicProperties().ExportTo("Areas/Reporting/Models/Model.ts");
            }, new Dictionary<string, string>()
            {
                {"D:\\Areas\\APIv2\\Models\\TimeAndAttendance\\Enum.ts", file1},
                {"D:\\Areas\\Reporting\\Models\\Model.ts", file2},
            }, compareComments: true);
        }
    }
}