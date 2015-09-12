using System;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    public class ParameterCodeGenerator : ITsCodeGenerator<ParameterInfo>
    {
        protected string GetDefaultValue(ParameterInfo element,TsParameterAttribute attr)
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
                return ((bool) defVal) ? "true" : "false";
            }
            var ts =  defVal.ToString();
            if (string.IsNullOrEmpty(ts)) return null;
            return ts;

        }
        public virtual void Generate(ParameterInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;
            string name = element.Name;
            string type = null;
            bool isNullable = false;
            string defaultValue = null;

            var fa = element.GetCustomAttribute<TsParameterAttribute>();
            defaultValue = GetDefaultValue(element, fa);
            if (fa != null)
            {
                if (!string.IsNullOrEmpty(fa.Name)) name = fa.Name;

                if (!string.IsNullOrEmpty(fa.Type)) type = fa.Type;
                else if (fa.StrongType != null)
                {
                    type = resolver.ResolveTypeName(fa.StrongType);
                    isNullable = fa.StrongType.IsNullable() || element.IsOptional;
                }
                else type = resolver.ResolveTypeName(element.ParameterType);
            }
            else
            {
                type = resolver.ResolveTypeName(element.ParameterType);
                isNullable = element.ParameterType.IsNullable() || element.IsOptional;
            }
            if (element.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                sw.Write("...");
            }
            sw.Write(name);
            if (isNullable) sw.Write("?");
            sw.Write(":{0}", type);
            if (defaultValue != null)
            {
                sw.Write(" = {0}",defaultValue);
            }
        }
    }
}
