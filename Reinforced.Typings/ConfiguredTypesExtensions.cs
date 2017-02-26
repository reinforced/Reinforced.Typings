using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    public static class ConfiguredTypesExtensions
    {
        /// <summary>
        ///     Search predicate to exclude ignored and compiler-generated items
        /// </summary>
        public static readonly Func<MemberInfo, bool> TypeScriptMemberSearchPredicate =
            c =>
                (!ConfigurationRepository.Instance.IsIgnored(c)) &&
                (c.GetCustomAttribute<CompilerGeneratedAttribute>() == null);

        /// <summary>
        ///     Returns all properties that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Properties array</returns>
        public static PropertyInfo[] GetExportedProperties(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedProperties(t);
        }

        /// <summary>
        ///     Returns all fields that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Fields array</returns>
        public static FieldInfo[] GetExportedFields(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedFields(t);
        }

        /// <summary>
        ///     Returns all methods that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Methods array</returns>
        public static MethodInfo[] GetExportedMethods(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedMethods(t);
        }


        /// <summary>
        ///     Retrieves parameter name from corresponding attribute. If attribute not present then takes parameter name via
        ///     reflection
        /// </summary>
        /// <param name="element">Parameter info</param>
        /// <returns>Parameter name</returns>
        public static string GetName(this ParameterInfo element)
        {
            var fa = ConfigurationRepository.Instance.ForMember(element);
            if (fa != null && !string.IsNullOrEmpty(fa.Name)) return fa.Name;
            return element.Name;
        }

        /// <summary>
        ///     Retrieves type name from type itself or from corresponding Reinforced.Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="genericArguments">Generic arguments to be substituted to type</param>
        /// <returns>Type name</returns>
        public static RtSimpleTypeName GetName(this Type t, RtTypeName[] genericArguments = null)
        {
            if (t.IsEnum)
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                var ns = t.Name;
                if (te != null && !string.IsNullOrEmpty(te.Name))
                {
                    ns = te.Name;
                }
                return new RtSimpleTypeName(ns);
            }

            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            var nameFromAttr = tc != null ? tc.Name : ti != null ? ti.Name : null;
            var name = (!string.IsNullOrEmpty(nameFromAttr) ? nameFromAttr : t.CleanGenericName());
            if (genericArguments == null) genericArguments = t.SerializeGenericArguments();

            if (ti != null)
            {
                if (ti.AutoI && !name.StartsWith("I")) name = "I" + name;
            }
            return new RtSimpleTypeName(name, genericArguments);
        }

        /// <summary>
        ///     Retrieves type namespace from type itself or from corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="distinguishAutoTypes">
        ///     Forces GetNamespace to return "-" for interfaces with IncludeInterface = false and
        ///     null for anonymous types
        /// </param>
        /// <returns>Full-qualified namespace name</returns>
        public static string GetNamespace(this Type t, bool distinguishAutoTypes = false)
        {
            if (t.IsEnum)
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                var ns = t.Namespace;
                if (te != null && te.IncludeNamespace && !string.IsNullOrEmpty(te.Namespace))
                {
                    ns = te.Namespace;
                }
                return ns;
            }
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            if (tc == null && ti == null) return t.Namespace;
            var nameFromAttr = tc != null ? tc.Namespace : ti.Namespace;
            var includeNamespace = tc != null ? tc.IncludeNamespace : ti.IncludeNamespace;
            if (!includeNamespace) return distinguishAutoTypes ? "-" : string.Empty;
            if (!string.IsNullOrEmpty(nameFromAttr)) return nameFromAttr;
            return t.Namespace;
        }

        /// <summary>
        ///     Determines should type be exported as interface or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public static bool IsExportingAsInterface(this Type t)
        {
            return ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t) != null;
        }

        /// <summary>
        ///     Determines should type be exported as class or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public static bool IsExportingAsClass(this Type t)
        {
            return ConfigurationRepository.Instance.ForType<TsClassAttribute>(t) != null;
        }

        /// <summary>
        ///     Determines if type member should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type member info</param>
        /// <returns>True if type member should be ignored, false otherwise</returns>
        public static bool IsIgnored(this MemberInfo t)
        {
            return ConfigurationRepository.Instance.IsIgnored(t);
        }

        /// <summary>
        ///     Determines if parameter should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Parameter info</param>
        /// <returns>True if parameter should be ignored, false otherwise</returns>
        public static bool IsIgnored(this ParameterInfo t)
        {
            return ConfigurationRepository.Instance.IsIgnored(t);
        }
    }
}
