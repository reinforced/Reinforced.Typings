using System;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for method parameter
    /// </summary>
    public class ParameterCodeGenerator : TsCodeGeneratorBase<ParameterInfo,RtArgument>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtArgument GenerateNode(ParameterInfo element,RtArgument result, TypeResolver resolver)
        {
            if (element.IsIgnored()) return null;
            var name = element.Name;
            RtTypeName type;
            var isNullable = false;

            var fa = ConfigurationRepository.Instance.ForMember(element);
            var defaultValue = GetDefaultValue(element, fa);
            if (fa != null)
            {
                if (!string.IsNullOrEmpty(fa.Name)) name = fa.Name;

                if (!string.IsNullOrEmpty(fa.Type)) type = new RtSimpleTypeName(fa.Type);
                else if (fa.StrongType != null)
                {
                    type = resolver.ResolveTypeName(fa.StrongType);
                    isNullable = element.IsOptional;
                }
                else type = resolver.ResolveTypeName(element.ParameterType);
                type = fa.TypeInferers.Infer(element, resolver) ?? type;
            }
            else
            {
                type = resolver.ResolveTypeName(element.ParameterType);
                isNullable = element.IsOptional;
            }
            if (element.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                result.IsVariableParameters = true;
            }
            result.Identifier = new RtIdentifier(name);
            result.Type = type;
            if (isNullable && defaultValue == null) result.Identifier.IsNullable = true;
            if (defaultValue != null) result.DefaultValue = defaultValue;
            AddDecorators(result, ConfigurationRepository.Instance.DecoratorsFor(element));
            return result;
        }

        /// <summary>
        ///     Returns default value for specified parameter info
        /// </summary>
        /// <param name="element">Parameter info</param>
        /// <param name="attr">Parameter attribute</param>
        /// <returns>Serialized to string default value of type that is exposed by mentioned parameter</returns>
        protected virtual string GetDefaultValue(ParameterInfo element, TsParameterAttribute attr)
        {
            object defVal = null;
            if (attr != null)
            {
                defVal = attr.DefaultValue;
            }
            if (defVal == null)
            {
                defVal = element.DefaultValue;
            }

            if (defVal == null) return null;

            if (defVal is string)
            {
                return string.Format("\"{0}\"", defVal);
            }
            if (defVal is bool)
            {
                return ((bool) defVal) ? "true" : "false";
            }
            var ts = defVal.ToString();
            if (string.IsNullOrEmpty(ts)) return null;
            return ts;
        }
    }
}