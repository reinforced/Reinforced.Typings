using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for enums
    /// </summary>
    public class EnumGenerator : ITsCodeGenerator<Type,RtEnum>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        public virtual RtEnum Generate(Type element, TypeResolver resolver)
        {
            RtEnum result = new RtEnum();
            var values = Enum.GetValues(element);
            result.EnumName = element.GetName();
            var fields = element.GetFields().Where(c => !c.IsSpecialName).ToDictionary(c => c.Name, c => c);
            var doc = Settings.Documentation.GetDocumentationMember(element);
            if (doc != null)
            {
                RtJsdocNode docNode = new RtJsdocNode();
                if (doc.HasSummary()) docNode.Description = doc.Summary.Text;
                result.Documentation = docNode;
            }
            List<RtEnumValue> valuesResult = new List<RtEnumValue>();
            for (var index = 0; index < values.Length; index++)
            {
                var v = values.GetValue(index);
                var n = Enum.GetName(element, v);

                if (fields.ContainsKey(n))
                {
                    var fieldItself = fields[n];
                    
                    var attr = ConfigurationRepository.Instance.ForEnumValue(fieldItself);
                    if (attr != null) n = attr.Name;
                    RtEnumValue value = new RtEnumValue
                    {
                        EnumValueName = n,
                        EnumValue = Convert.ToInt64(v).ToString()
                    };

                    var valueDoc = Settings.Documentation.GetDocumentationMember(fieldItself);
                    if (valueDoc != null)
                    {
                        RtJsdocNode docNode = new RtJsdocNode();
                        if (doc.HasSummary()) docNode.Description = valueDoc.Summary.Text;
                        value.Documentation = docNode;
                    }

                    valuesResult.Add(value);
                }
            }
            result.Values.AddRange(valuesResult);
            return result;
        }

        /// <summary>
        ///     Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }
    }
}