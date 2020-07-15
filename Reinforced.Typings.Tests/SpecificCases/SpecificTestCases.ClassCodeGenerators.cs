using System;
using System.CodeDom;
using System.Text;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public class CodeGeneratedClass
    {
        public void DoSomething() { }

        public string GetName(int arg) { return string.Empty; }
    }

    public class FunClassCodeGenerator : ClassCodeGenerator
    {
        public override RtClass GenerateNode(Type element, RtClass result, TypeResolver resolver)
        {
            // obtain current namespace
            var ns = this.Context.Location.CurrentNamespace;
            var r = base.GenerateNode(element, result, resolver);
            foreach (var rMember in r.Members)
            {
                var m = rMember as RtFunction;
                if (m != null)
                {
                    m.AccessModifier = null;

                    ns.CompilationUnits.Add(m);
                }
            }

            // return null instead of result to 
            // suppress writing AST of original class 
            // to resulting file
            return null;
        }
    }
    

public class AdditionalEnumGenerator : EnumGenerator
{
    public override RtEnum GenerateNode(Type element, RtEnum result, TypeResolver resolver)
    {
        var resultEnum = base.GenerateNode(element, result, resolver);

        if (Context.Location.CurrentNamespace!=null)
        {
            Context.Location.CurrentNamespace.CompilationUnits.Add(resultEnum);

            StringBuilder enumdescriptor = new StringBuilder();
            enumdescriptor.AppendLine();
            enumdescriptor.AppendLine($"const {resultEnum.EnumName} = new Map<number, string>([");
            bool first = true;
            foreach (var resultEnumValue in resultEnum.Values)
            {
                if (!first) enumdescriptor.AppendLine(",");
                first = false;
                var enumDescription = resultEnumValue.EnumValueName.ToUpper(); //<- here you get your desired enum description string somehow
                enumdescriptor.Append($"[{resultEnum.EnumName}.{resultEnumValue.EnumValueName},'{enumDescription}']");
            }
            enumdescriptor.AppendLine("]);");

            Context.Location.CurrentNamespace.CompilationUnits.Add(new RtRaw(enumdescriptor.ToString()));

        }

        return null;
    }
    }

    

    public partial class SpecificTestCases
    {
        [Fact]
        public void ClassCodeGenerators()
        {
            const string result = @"
DoSomething() : void { } 
GetName(arg: number) : string
{
	return null;
}
enum SomeEnum { 
	One = 0, 
	Two = 1
}

const SomeEnum = new Map<number, string>([
[SomeEnum.One,'ONE'],
[SomeEnum.Two,'TWO']]);
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsClass<CodeGeneratedClass>()
                    .WithPublicMethods()
                    .WithCodeGenerator<FunClassCodeGenerator>()
                    .DontIncludeToNamespace();
                s.ExportAsEnum<SomeEnum>().WithCodeGenerator<AdditionalEnumGenerator>().DontIncludeToNamespace();
                    ;
            }, result);
        }
    }
}