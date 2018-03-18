using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    /// <summary>
    /// Set of extension methods for RT configured types
    /// </summary>
    public static class ConfiguredTypesExtensions
    {
        /// <summary>
        ///     Search predicate to exclude ignored and compiler-generated items
        /// </summary>
        public static readonly Func<MemberInfo, bool> TypeScriptMemberSearchPredicate =
            c =>
                (!ConfigurationRepository.Instance.IsIgnored(c)) &&
                (c.GetCustomAttribute<CompilerGeneratedAttribute>() == null);

        #region Retrieval of exported members

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


        #endregion

        #region Orders

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public static double GetOrder(this MemberInfo element)
        {
            if (element is MethodInfo) return GetOrder((MethodInfo)element);
            if (element is PropertyInfo) return GetOrder((PropertyInfo)element);
            if (element is FieldInfo) return GetOrder((FieldInfo)element);
            return 0;
        }

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public static double GetOrder(this MethodInfo element)
        {
            var fa = ConfigurationRepository.Instance.ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public static double GetOrder(this PropertyInfo element)
        {
            var fa = ConfigurationRepository.Instance.ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public static double GetOrder(this FieldInfo element)
        {
            var fa = ConfigurationRepository.Instance.ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves type order to appear in results file
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Type name</returns>
        public static double GetOrder(this Type t)
        {
            if (t._IsEnum())
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                return te.Order;
            }

            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            var order = tc != null ? tc.Order : ti != null ? ti.Order : 0;
            return order;
        }
        #endregion

        #region Names

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
            if (t._IsEnum())
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
                if (ti.AutoI)
                {
                    if (t._IsClass()) name = "I" + name;
                    else if (!name.StartsWith("I")) name = "I" + name;
                }
            }
            return new RtSimpleTypeName(name, genericArguments);
        }

        /// <summary>
        /// Gets whether type configuration required flattering inheritance hierarchy
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, when hierarchy must be flatten, false otherwise</returns>
        public static bool IsFlatten(this Type t)
        {
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            return tc != null ? tc.FlattenHierarchy : ti != null ? ti.FlattenHierarchy : false;
        }



        #endregion

        #region Is exported (as)

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

        #endregion

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
            if (t._IsEnum())
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                if (te == null) return t.Namespace;
                var ns = te.Namespace;
                if (!te.IncludeNamespace) return distinguishAutoTypes ? "-" : string.Empty;
                if (!string.IsNullOrEmpty(ns)) return ns;
                return t.Namespace;
            }
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            if (tc == null && ti == null) return t.Namespace;
            var nsFromAttr = tc != null ? tc.Namespace : ti != null ? ti.Namespace : t.Namespace;
            var includeNamespace = tc != null ? tc.IncludeNamespace : ti != null ? ti.IncludeNamespace : true;

            if (!includeNamespace) return distinguishAutoTypes ? "-" : string.Empty;
            if (!string.IsNullOrEmpty(nsFromAttr)) return nsFromAttr;
            return t.Namespace;
        }

        /// <summary>
        /// Obtains substitution for type
        /// </summary>
        /// <param name="t">Type to find substitution for</param>
        /// <param name="currentlyExportingType">Currently exported type (to look up substitutions)</param>
        /// <param name="tr">Type resolver instance</param>
        /// <returns>Substitution AST</returns>
        public static RtTypeName Substitute(this Type t, Type currentlyExportingType,TypeResolver tr)
        {
            Type genericDef = t._IsGenericType() ? t.GetGenericTypeDefinition() : null;

            if (currentlyExportingType != null)
            {
                if (ConfigurationRepository.Instance.TypeSubstitutions.ContainsKey(currentlyExportingType))
                {
                    var ts = ConfigurationRepository.Instance.TypeSubstitutions[currentlyExportingType];
                    if (ts.ContainsKey(t)) return ts[t];
                }

                if (genericDef!=null)
                {
                    if (ConfigurationRepository.Instance.TypeGenericSubstitutions.ContainsKey(currentlyExportingType))
                    {
                        if (ConfigurationRepository.Instance.TypeGenericSubstitutions[currentlyExportingType].ContainsKey(genericDef))
                        {
                            var r = ConfigurationRepository.Instance.TypeGenericSubstitutions[currentlyExportingType][genericDef];
                            var ts = r(t, tr);
                            if (ts != null) return ts;
                        }
                    }
                }
            }
            if (ConfigurationRepository.Instance.GlobalSubstitutions.ContainsKey(t)) return ConfigurationRepository.Instance.GlobalSubstitutions[t];
            if (genericDef != null)
            {
                if (ConfigurationRepository.Instance.GlobalGenericSubstitutions.ContainsKey(genericDef))
                {
                    var ts = ConfigurationRepository.Instance.GlobalGenericSubstitutions[genericDef](t, tr);
                    if (ts != null) return ts;
                }
            }
            return null;
        }


        internal static void AddReferencesFromTypes(this ExportedFile file, bool useImports)
        {
            foreach (var type in file.TypesToExport)
            {
                file.AddTypeSpecificReferences(type);
                if (useImports) file.AddTypeSpecificImports(type);
            }
        }

        private static void AddTypeSpecificReferences(this ExportedFile file, Type t)
        {
            var frefs = ConfigurationRepository.Instance.ReferenceAttributes;

            var fluentRefs = frefs.ContainsKey(t) ? frefs[t] : null;
            var typeRefs = t.GetCustomAttributes<TsAddTypeReferenceAttribute>();

            foreach (var tsAddTypeReferenceAttribute in typeRefs)
            {
                if (tsAddTypeReferenceAttribute.Type != null)
                {
                    file.TypeResolver.ResolveTypeName(tsAddTypeReferenceAttribute.Type);
                }
                else
                {
                    file.References.AddReference(new RtReference() { Path = tsAddTypeReferenceAttribute.RawPath });
                }
            }

            if (fluentRefs != null)
            {
                foreach (var tsAddTypeReferenceAttribute in fluentRefs)
                {
                    if (tsAddTypeReferenceAttribute.Type != null)
                    {
                        file.TypeResolver.ResolveTypeName(tsAddTypeReferenceAttribute.Type);
                    }
                    else
                    {
                        file.References.AddReference(new RtReference() { Path = tsAddTypeReferenceAttribute.RawPath });
                    }
                }
            }
        }

        private static void AddTypeSpecificImports(this ExportedFile file, Type t)
        {
            var fimps = ConfigurationRepository.Instance.ImportAttributes;
            var fluentImports = fimps.ContainsKey(t) ? fimps[t] : null;
            var typeImports = t.GetCustomAttributes<TsAddTypeImportAttribute>();
            foreach (var tsAddTypeImportAttribute in typeImports)
            {
                file.References.AddImport(new RtImport() {From = tsAddTypeImportAttribute.ImportSource,Target = tsAddTypeImportAttribute.ImportTarget,IsRequire = tsAddTypeImportAttribute.ImportRequire});
            }
            if (fluentImports != null)
            {
                foreach (var tsAddTypeImportAttribute in fluentImports)
                {
                    file.References.AddImport(new RtImport() { From = tsAddTypeImportAttribute.ImportSource, Target = tsAddTypeImportAttribute.ImportTarget, IsRequire = tsAddTypeImportAttribute.ImportRequire });
                }
            }
        }

    }
}
