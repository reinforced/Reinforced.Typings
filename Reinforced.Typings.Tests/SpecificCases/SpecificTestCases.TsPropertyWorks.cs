using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void TsPropertyWorks()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IMyPropertyTestClass
	{
		myNumber: number;
		MyNumber: number;
		myProperty: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(x=>x.DontWriteWarningComment().ReorderMembers());
                s.ExportAsInterface<MyPropertyTestClass>().WithPublicProperties();
            }, result);
        }
    }

    #region vmandic test with possibly not working TsProperty

    public class MyPropertyTestClass
    {
        [TsProperty(ShouldBeCamelCased = true)]
        public string MyProperty { get; set; }

        [TestProperty]
        public int MyNumber { get; set; }
    }

    public class TestPropertyAttribute : TsPropertyAttribute
    {
        public TestPropertyAttribute()
        {
            this.ShouldBeCamelCased = true;
            this.CodeGeneratorType = typeof(LegacyPropertyDuplicator);
        }
    }

    public class LegacyPropertyDuplicator : PropertyCodeGenerator
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtField GenerateNode(MemberInfo element, RtField result, TypeResolver resolver)
        {
            var b = base.GenerateNode(element, result, resolver);
            if (b == null) return null;

            var pascalCaseName = b.Identifier.IdentifierName.Substring(0, 1).ToUpperInvariant()
                                 + b.Identifier.IdentifierName.Substring(1);

            var newField = new RtField()
            {
                AccessModifier = b.AccessModifier,
                Identifier = new RtIdentifier(pascalCaseName),
                InitializationExpression = b.InitializationExpression,
                IsStatic = b.IsStatic,
                Documentation = b.Documentation,
                Order = b.Order,
                Type = b.Type
            };
            if (Context.Location.CurrentClass != null)
            {
                Context.Location.CurrentClass.Members.Add(newField);
            }
            if (Context.Location.CurrentInterface != null)
            {
                Context.Location.CurrentInterface.Members.Add(newField);
            }
            return b;
        }
    }
    #endregion
}