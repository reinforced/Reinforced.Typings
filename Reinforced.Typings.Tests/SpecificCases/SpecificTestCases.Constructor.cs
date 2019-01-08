using Reinforced.Typings.Ast;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        class ClassWithDefaultConstructor
        {
            
        }

        class ClassWithDefaultConstructorWithBody
        {
            
        }

        class ClassWithParametersConstructor
        {
            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public ClassWithParametersConstructor(int x, string y, bool z)
            {
            }
        }
        [Fact]
        public void GenerateConstructors()
        {
            const string result = @"
export class ClassWithDefaultConstructor
{
	constructor () { } 
}
export class ClassWithDefaultConstructorWithBody
{
	constructor ()
	{
		this.invokeSomeMethod();
	}
}
export class ClassWithParametersConstructor
{
	constructor (x: number, y: string, z: boolean)
	{
		this.x = x; this.y = y; this.z = z;
	}
}
";

            const string resultDts = @"
declare class ClassWithDefaultConstructor
{
	constructor (); 
}
declare class ClassWithDefaultConstructorWithBody
{
	constructor (); 
}
declare class ClassWithParametersConstructor
{
	constructor (x: number, y: string, z: boolean); 
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules().ReorderMembers());

                s.ExportAsClass<ClassWithDefaultConstructor>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor()
                    ;
                s.ExportAsClass<ClassWithDefaultConstructorWithBody>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor(new RtRaw("this.invokeSomeMethod();"))
                    ;
                s.ExportAsClass<ClassWithParametersConstructor>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor(new RtRaw("this.x = x; this.y = y; this.z = z;"))
                    ;
            }, result, compareComments: true);

            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().UseModules().ReorderMembers().ExportPureTypings());

                s.ExportAsClass<ClassWithDefaultConstructor>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor()
                    ;
                s.ExportAsClass<ClassWithDefaultConstructorWithBody>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor(new RtRaw("this.invokeSomeMethod();"))
                    ;
                s.ExportAsClass<ClassWithParametersConstructor>()
                    .WithPublicProperties()
                    .WithPublicMethods()
                    .WithConstructor(new RtRaw("this.x = x; this.y = y; this.z = z;"))
                    ;
            }, resultDts, compareComments: true);
        }
    }
}