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
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(MemberInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;

            var t = GetType(element);
            string typeName = null;
            string propName = element.Name;
            var tp = ConfigurationRepository.Instance.ForMember<TsPropertyAttribute>(element);
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
                if (tp.ForceNullable && element.DeclaringType.IsExportingAsInterface() && !Settings.SpecialCase) propName = propName + "?";
            }
            if (string.IsNullOrEmpty(typeName)) typeName = resolver.ResolveTypeName(t);
            if (!propName.EndsWith("?") && t.IsNullable() && element.DeclaringType.IsExportingAsInterface() && !Settings.SpecialCase)
            {
                propName = propName + "?";
            }

            if (element is PropertyInfo)
            {
                propName = Settings.ConditionallyConvertPropertyNameToCamelCase(propName);
            }

            sw.Tab();
            Settings.Documentation.WriteDocumentation(element, sw);
            sw.Indent();
            if (!element.DeclaringType.IsExportingAsInterface() || Settings.SpecialCase)
            {
                var modifier = Settings.SpecialCase? AccessModifier.Public: element.GetModifier();
                sw.Write("{0} ",modifier.ToModifierText());
            }
            sw.Write("{0}: {1};", propName, typeName);
            sw.Br();
            sw.UnTab();
        }

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }

        /// <summary>
        /// Returns type of specified property. It is useful for overloads sometimes
        /// </summary>
        /// <param name="mi">Method Info</param>
        /// <returns>Property info type</returns>
        protected virtual Type GetType(MemberInfo mi)
        {
            PropertyInfo pi = (PropertyInfo)mi;
            return pi.PropertyType;
        }
    }
}
