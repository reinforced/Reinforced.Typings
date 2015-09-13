using System;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for method parameter
    /// </summary>
    public class ParameterCodeGenerator : ITsCodeGenerator<ParameterInfo>
    {
        /// <summary>
        /// Returns default value for specified parameter info
        /// </summary>
        /// <param name="element">Parameter info</param>
        /// <param name="attr">Parameter attribute</param>
        /// <returns>Serialized to string default value of type that is exposed by mentioned parameter</returns>
        protected string GetDefaultValue(ParameterInfo element, TsParameterAttribute attr)
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
                return String.Format("\"{0}\"", defVal);
            }
            if (defVal is bool)
            {
                return ((bool)defVal) ? "true" : "false";
            }
            var ts = defVal.ToString();
            if (string.IsNullOrEmpty(ts)) return null;
            return ts;

        }
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(ParameterInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;
            string name = element.Name;
            string type;
            bool isNullable = false;

            var fa = element.GetCustomAttribute<TsParameterAttribute>();
            var defaultValue = GetDefaultValue(element, fa);
            if (fa != null)
            {
                if (!string.IsNullOrEmpty(fa.Name)) name = fa.Name;

                if (!string.IsNullOrEmpty(fa.Type)) type = fa.Type;
                else if (fa.StrongType != null)
                {
                    type = resolver.ResolveTypeName(fa.StrongType);
                    isNullable = element.IsOptional;
                }
                else type = resolver.ResolveTypeName(element.ParameterType);
            }
            else
            {
                type = resolver.ResolveTypeName(element.ParameterType);
                isNullable = element.IsOptional;
            }
            if (element.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                sw.Write("...");
            }
            sw.Write(name);
            if (isNullable && defaultValue == null) sw.Write("?"); //wait what? Ts do not have nullable parameters
            sw.Write(":{0}", type);
            if (defaultValue != null)
            {
                sw.Write(" = {0}", defaultValue);
            }
        }

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }
    }
}
