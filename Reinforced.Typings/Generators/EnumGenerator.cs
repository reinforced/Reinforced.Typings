using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for enums
    /// </summary>
    public class EnumGenerator : TsCodeGeneratorBase<Type, RtEnum>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtEnum GenerateNode(Type element, RtEnum result, TypeResolver resolver)
        {
            Context.Location.SetCurrentType(element);

            var names = Enum.GetNames(element);
            var bp = Context.Project.Blueprint(element);
            result.EnumName = bp.GetName();
            result.Order = bp.GetOrder();

            var fields = element._GetFields().Where(c => !c.IsSpecialName).ToDictionary(c => c.Name, c => c);
            var doc = Context.Documentation.GetDocumentationMember(element);
            if (doc != null)
            {
                RtJsdocNode docNode = new RtJsdocNode();
                if (doc.HasInheritDoc()) docNode.AddTag(DocTag.Inheritdoc);
                if (doc.HasSummary()) docNode.Description = doc.Summary.Text;
                result.Documentation = docNode;
            }

            bool stringInit = false;
            var ea = Context.CurrentBlueprint.Attr<TsEnumAttribute>();
            if (ea != null)
            {
                result.IsConst = ea.IsConst;
                stringInit = ea.UseString;
            }
            List<RtEnumValue> valuesResult = new List<RtEnumValue>();
            for (var index = 0; index < names.Length; index++)
            {
                var n = names.GetValue(index) as string;
                var v = Enum.Parse(element, n);

                if (fields.ContainsKey(n))
                {
                    var fieldItself = fields[n];

                    var attr = Context.CurrentBlueprint.ForEnumValue(fieldItself);
                    if (attr != null) n = attr.Name;
                    if (string.IsNullOrEmpty(n)) n = fieldItself.Name;
                    RtEnumValue value = new RtEnumValue
                    {
                        EnumValueName = n,
                        EnumValue = Convert.ToInt64(v).ToString()
                    };

                    if (stringInit)
                    {
                        if (attr != null && !string.IsNullOrEmpty(attr.Initializer)) value.EnumValue = String.Concat("\"", attr.Initializer, "\"");
                        else
                        {
                            value.EnumValue = String.Concat("\"", n, "\"");
                        }
                    }

                    var valueDoc = Context.Documentation.GetDocumentationMember(fieldItself);
                    if (valueDoc != null)
                    {
                        RtJsdocNode docNode = new RtJsdocNode();
                        if (valueDoc.HasInheritDoc()) docNode.AddTag(DocTag.Inheritdoc);
                        if (valueDoc.HasSummary()) docNode.Description = valueDoc.Summary.Text;
                        value.Documentation = docNode;
                    }

                    valuesResult.Add(value);
                }
            }
            result.Values.AddRange(valuesResult);
            AddDecorators(result, Context.CurrentBlueprint.GetDecorators());

            Context.Location.ResetCurrentType();
            return result;
        }
    }
}