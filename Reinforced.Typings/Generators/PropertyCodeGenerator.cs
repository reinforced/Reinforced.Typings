using System;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for properties
    /// </summary>
    public class PropertyCodeGenerator : ITsCodeGenerator<MemberInfo>
    {
        public virtual void Generate(MemberInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;

            var t = GetType(element);
            string typeName = null;
            string propName = element.Name;
            var tp = element.GetCustomAttribute<TsPropertyAttribute>();
            if (tp != null)
            {
                if (tp.StrongType != null)
                {
                    typeName = resolver.ResolveTypeName(tp.StrongType);
                }
                else if (!string.IsNullOrEmpty(tp.Type))
                {
                    typeName = tp.Type;
                }

                if (!string.IsNullOrEmpty(tp.Name)) propName = tp.Name;
                if (tp.ForceNullable) propName = propName + "?";
            }
            if (string.IsNullOrEmpty(typeName)) typeName = resolver.ResolveTypeName(t);
            if (!propName.EndsWith("?") && t.IsNullable())
            {
                propName = propName + "?";
            }

            sw.Tab();
            sw.Indent();
            sw.Write("{0}: {1};",propName,typeName);
            sw.WriteLine();
            sw.UnTab();
        }

        /// <summary>
        /// Returns type of specified property. It is useful for overloads sometimes
        /// </summary>
        /// <param name="mi">Method Info</param>
        /// <returns>Property info type</returns>
        protected virtual Type GetType(MemberInfo mi)
        {
            PropertyInfo pi = (PropertyInfo) mi;
            return pi.PropertyType;
        }
    }
}
