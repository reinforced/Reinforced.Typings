using System;
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
                var m = rMember as RtFuncion;
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
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ReorderMembers());
                s.ExportAsClass<CodeGeneratedClass>()
                            .WithPublicMethods()
                            .WithCodeGenerator<FunClassCodeGenerator>()
                            .DontIncludeToNamespace()
                    ;
            }, result);
        }
    }
}