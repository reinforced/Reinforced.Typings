using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    /// <summary>
    /// Holds all information that is necessary to export particular type
    /// </summary>
    public class TypeBlueprint
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TypeBlueprint(Type t)
        {
            _attributesForParameters = new Dictionary<ParameterInfo, TsParameterAttribute>();
            _attributesForProperties = new Dictionary<PropertyInfo, TsPropertyAttribute>();
            _attributesForMethods = new Dictionary<MethodInfo, TsFunctionAttribute>();
            _attributesForFields = new Dictionary<FieldInfo, TsPropertyAttribute>();
            _attributesForEnumValues = new Dictionary<FieldInfo, TsValueAttribute>();

            Substitutions = new Dictionary<Type, RtTypeName>();
            GenericSubstitutions = new Dictionary<Type, Func<Type, TypeResolver, RtTypeName>>();
            References = new List<TsAddTypeReferenceAttribute>();
            Imports = new List<TsAddTypeImportAttribute>();
            DecoratorsForMembers = new Dictionary<MemberInfo, List<TsDecoratorAttribute>>();
            DecoratorsForParameters = new Dictionary<ParameterInfo, List<TsDecoratorAttribute>>();
            Decorators = new List<TsDecoratorAttribute>();
            Type = t;
            InitFromAttributes();
        }

        /// <summary>
        /// Path to file that blueprinted type will be exported to
        /// </summary>
        public string PathToFile { get; set; }

        private void InitFromAttributes()
        {
            TypeAttribute = Type.GetCustomAttribute<TsEnumAttribute>(false)
                            ?? (TsDeclarationAttributeBase)Type.GetCustomAttribute<TsInterfaceAttribute>(false)
                            ?? (TsDeclarationAttributeBase)Type.GetCustomAttribute<TsClassAttribute>(false);
        }

        /// <summary>
        ///     Type is being exported
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Attribute for exporting class
        /// </summary>
        public TsDeclarationAttributeBase TypeAttribute { get; set; }

        private readonly HashSet<object> _ignored = new HashSet<object>();

        public HashSet<object> Ignored
        {
            get { return _ignored; }
            private set { }
        }

        private readonly Dictionary<PropertyInfo, TsPropertyAttribute> _attributesForProperties;
        private readonly Dictionary<MethodInfo, TsFunctionAttribute> _attributesForMethods;
        private readonly Dictionary<ParameterInfo, TsParameterAttribute> _attributesForParameters;
        private readonly Dictionary<FieldInfo, TsPropertyAttribute> _attributesForFields;
        private readonly Dictionary<FieldInfo, TsValueAttribute> _attributesForEnumValues;

        /// <summary>
        /// Substitutions
        /// </summary>
        public Dictionary<Type, RtTypeName> Substitutions { get; private set; }

        /// <summary>
        /// Generic substitutions
        /// </summary>
        public Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GenericSubstitutions { get; private set; }

        /// <summary>
        /// References
        /// </summary>
        public List<TsAddTypeReferenceAttribute> References { get; private set; }

        /// <summary>
        /// Imports
        /// </summary>
        public List<TsAddTypeImportAttribute> Imports { get; private set; }


        /// <summary>
        /// Decorators
        /// </summary>
        public List<TsDecoratorAttribute> Decorators { get; set; }

        /// <summary>
        /// Decorators for type members
        /// </summary>
        public Dictionary<MemberInfo, List<TsDecoratorAttribute>> DecoratorsForMembers { get; private set; }

        /// <summary>
        /// Decorators for parameters
        /// </summary>
        public Dictionary<ParameterInfo, List<TsDecoratorAttribute>> DecoratorsForParameters { get; private set; }

        public TsTypedMemberAttributeBase ForMember(MemberInfo member, bool create = false)
        {
            if (member is PropertyInfo) return ForMember((PropertyInfo)member, create);
            if (member is MethodInfo) return ForMember((MethodInfo)member, create);
            if (member is FieldInfo) return ForMember((FieldInfo)member, create);
            return null;
        }

        public T ForMember<T>(MemberInfo member, bool create = false) where T : TsTypedMemberAttributeBase
        {
            if (member is PropertyInfo) return (T)(object)ForMember((PropertyInfo)member, create);
            if (member is MethodInfo) return (T)(object)ForMember((MethodInfo)member, create);
            if (member is FieldInfo) return (T)(object)ForMember((FieldInfo)member, create);
            return null;
        }


        public TsFunctionAttribute ForMember(MethodInfo member, bool create = false)
        {
            var v = _attributesForMethods.GetOr(member, () => member.GetCustomAttribute<TsFunctionAttribute>(false));
            if (create && v == null)
            {
                return _attributesForMethods.GetOrCreate(member);
            }
            return v;
        }

        
        public TsParameterAttribute ForMember(ParameterInfo member, bool create = false)
        {
            var v = _attributesForParameters.GetOr(member, () => member.GetCustomAttribute<TsParameterAttribute>(false));
            if (create && v == null)
            {
                return _attributesForParameters.GetOrCreate(member);
            }
            return v;
        }

        public TsPropertyAttribute ForMember(PropertyInfo member, bool create = false)
        {
            var v = _attributesForProperties.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
            if (create && v == null)
            {
                return _attributesForProperties.GetOrCreate(member);
            }
            return v;
        }

        public TsPropertyAttribute ForMember(FieldInfo member, bool create = false)
        {
            var v = _attributesForFields.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
            if (create && v == null)
            {
                return _attributesForFields.GetOrCreate(member);
            }
            return v;
        }

        public TsBaseParamAttribute ForMember(ConstructorInfo member)
        {
            return member.GetCustomAttribute<TsBaseParamAttribute>(false);
        }

        public TsValueAttribute ForEnumValue(FieldInfo member, bool create = false)
        {
            var v = _attributesForEnumValues.GetOr(member, () => member.GetCustomAttribute<TsValueAttribute>(false));
            if (create && v == null)
            {
                return _attributesForEnumValues.GetOrCreate(member);
            }
            return v;
        }

        public FieldInfo[] GetExportedFields()
        {
            var t = Type;
            if (IsIgnored(t)) return new FieldInfo[0];
            if (t._IsEnum()) return new FieldInfo[0];


            var typeAttr = this.TypeAttribute;
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers = this.GetExportingMembers(typeAttr.FlattenHierarchy, (tp, b) => tp._GetFields(b), typeAttr.FlattenLimiter);

                if (!aexpSwith.AutoExportFields)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.ToArray();
            }
            return new FieldInfo[0];
        }

        public PropertyInfo[] GetExportedProperties()
        {
            var t = this.Type;
            if (IsIgnored(t)) return new PropertyInfo[0];
            if (t._IsEnum()) return new PropertyInfo[0];

            var typeAttr = this.TypeAttribute;
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers = this.GetExportingMembers(typeAttr.FlattenHierarchy, (tp, b) => tp._GetProperties(b), typeAttr.FlattenLimiter);

                if (!aexpSwith.AutoExportProperties)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.ToArray();
            }
            return new PropertyInfo[0];
        }

        public MethodInfo[] GetExportedMethods()
        {
            var t = Type;
            if (IsIgnored(t)) return new MethodInfo[0];
            if (t._IsEnum()) return new MethodInfo[0];

            var typeAttr = this.TypeAttribute;
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers = this.GetExportingMembers(typeAttr.FlattenHierarchy, (tp, b) => tp._GetMethods(b).Where(x => !x.IsSpecialName).ToArray(), typeAttr.FlattenLimiter);

                if (!aexpSwith.AutoExportMethods)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.Where(c => !c.IsSpecialName).ToArray();
            }
            return new MethodInfo[0];
        }

        #region Ignorance tracking methods

        public bool IsIgnored()
        {
            return TypeAttribute == null;
        }

        public bool IsIgnored(ConstructorInfo member)
        {
            return (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        public bool IsIgnored(FieldInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public bool IsIgnored(PropertyInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public bool IsIgnored(ParameterInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public bool IsIgnored(MemberInfo member)
        {
#if NETCORE1
#else
            //if (member is Type) return IsIgnored((Type)member);
#endif
            if (member is PropertyInfo) return IsIgnored((PropertyInfo)member);
            if (member is MethodInfo) return IsIgnored((MethodInfo)member);
            if (member is FieldInfo) return IsIgnored((FieldInfo)member);
            return false;
        }

        public bool IsIgnored(MethodInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        #endregion

        public IEnumerable<TsDecoratorAttribute> GetDecorators()
        {
            var inlineDecorators = Type.GetCustomAttributes<TsDecoratorAttribute>();

            return inlineDecorators.Union(Decorators);
        }

        public List<TsDecoratorAttribute> DecoratorsListFor(MemberInfo t)
        {
            return DecoratorsForMembers.GetOrCreate(t);
        }

        public List<TsDecoratorAttribute> DecoratorsListFor(ParameterInfo t)
        {
            return DecoratorsForParameters.GetOrCreate(t);
        }

        public IEnumerable<TsDecoratorAttribute> DecoratorsFor(MemberInfo t)
        {
            var inlineDecorators = t.GetCustomAttributes<TsDecoratorAttribute>();
            var fluentDecorators = DecoratorsForMembers.ContainsKey(t) ? DecoratorsForMembers[t] : null;

            if (fluentDecorators == null) return inlineDecorators;
            return inlineDecorators.Union(fluentDecorators);
        }

        public IEnumerable<TsDecoratorAttribute> DecoratorsFor(ParameterInfo t)
        {
            var inlineDecorators = t.GetCustomAttributes<TsDecoratorAttribute>();
            var fluentDecorators = DecoratorsForParameters.ContainsKey(t) ? DecoratorsForParameters[t] : null;

            if (fluentDecorators == null) return inlineDecorators;
            return inlineDecorators.Union(fluentDecorators);
        }

        public T Attr<T>() where T : TsDeclarationAttributeBase
        {
            return TypeAttribute as T;
        }




        /// <summary>
        ///     Conditionally (based on attribute setting) turns member name to camelCase
        /// </summary>
        /// <param name="member">Member</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in camelCase if camelCasing enabled, initial string otherwise</returns>
        public string CamelCaseFromAttribute(MemberInfo member, string regularName)
        {
            var attr = ForMember<TsTypedMemberAttributeBase>(member);
            if (attr == null) return regularName;
            if (attr.ShouldBeCamelCased) return ConvertToCamelCase(regularName);
            return regularName;
        }

        /// <summary>
        ///     Conditionally (based on attribute setting) turns member name to PascalCase
        /// </summary>
        /// <param name="member">Member</param>
        /// <param name="regularName">Regular property name</param>
        /// <returns>Property name in PascalCase if pascalCasing enabled, initial string otherwise</returns>
        public string PascalCaseFromAttribute(MemberInfo member, string regularName)
        {
            var attr = ForMember<TsTypedMemberAttributeBase>(member);
            if (attr == null) return regularName;
            if (attr.ShouldBePascalCased) return ConvertToPascalCase(regularName);
            return regularName;
        }

        public static string ConvertToCamelCase(string s)
        {
            if (!char.IsLetter(s[0])) return s;
            StringBuilder result = new StringBuilder();
            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (i < s.Length - 1 && char.IsLower(s[i + 1])) break;
                if (char.IsUpper(s[i])) result.Append(char.ToLowerInvariant(s[i]));
            }

            if (i < s.Length - 1)
            {
                if (i == 0)
                {
                    result.Append(char.ToLowerInvariant(s[0]));
                    result.Append(s.Substring(1));
                }
                else
                {
                    result.Append(s.Substring(i));
                }
            }
            return result.ToString();
        }

        private string ConvertToPascalCase(string s)
        {
            if (!char.IsLetter(s[0])) return s;
            if (char.IsLower(s[0]))
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            return s;
        }

        #region Order
        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public double GetOrder(MemberInfo element)
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
        public double GetOrder(MethodInfo element)
        {
            var fa = ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public double GetOrder(PropertyInfo element)
        {
            var fa = ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves member order
        /// </summary>
        /// <param name="element">Method info</param>
        /// <returns>Method order</returns>
        public double GetOrder(FieldInfo element)
        {
            var fa = ForMember(element);
            if (fa != null) return fa.Order;
            return 0;
        }

        /// <summary>
        ///     Retrieves type order to appear in results file
        /// </summary>

        /// <returns>Type name</returns>
        public double GetOrder()
        {
            if (Type._IsEnum())
            {
                var te = Attr<TsEnumAttribute>();
                return te.Order;
            }

            var tc = Attr<TsClassAttribute>();
            var ti = Attr<TsInterfaceAttribute>();
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
        public string GetName(ParameterInfo element)
        {
            var fa = ForMember(element);
            if (fa != null && !string.IsNullOrEmpty(fa.Name)) return fa.Name;
            return element.Name;
        }

        /// <summary>
        ///     Retrieves type name from type itself or from corresponding Reinforced.Typings attribute
        /// </summary>

        /// <param name="genericArguments">Generic arguments to be substituted to type</param>
        /// <returns>Type name</returns>
        public RtSimpleTypeName GetName(RtTypeName[] genericArguments = null)
        {
            var t = Type;
            if (t._IsEnum())
            {
                var te = Attr<TsEnumAttribute>();
                var ns = t.Name;
                if (te != null && !string.IsNullOrEmpty(te.Name))
                {
                    ns = te.Name;
                }
                return new RtSimpleTypeName(ns);
            }

            var tc = Attr<TsClassAttribute>();
            var ti = Attr<TsInterfaceAttribute>();
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
        #endregion

        /// <summary>
        /// Gets whether type configuration required flattering inheritance hierarchy
        /// </summary>

        /// <returns>True, when hierarchy must be flatten, false otherwise</returns>
        public bool IsFlatten()
        {
            var tc = Attr<TsClassAttribute>();
            var ti = Attr<TsInterfaceAttribute>();
            return tc != null ? tc.FlattenHierarchy : ti != null ? ti.FlattenHierarchy : false;
        }

        #region Is exported (as)

        /// <summary>
        ///     Determines should type be exported as interface or not
        /// </summary>

        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public bool IsExportingAsInterface()
        {
            return Attr<TsInterfaceAttribute>() != null;
        }

        /// <summary>
        ///     Determines should type be exported as class or not
        /// </summary>

        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public bool IsExportingAsClass()
        {
            return Attr<TsClassAttribute>() != null;
        }

        #endregion

        /// <summary>
        ///     Retrieves type namespace from type itself or from corresponding Typings attribute
        /// </summary>

        /// <param name="distinguishAutoTypes">
        ///     Forces GetNamespace to return "-" for interfaces with IncludeInterface = false and
        ///     null for anonymous types
        /// </param>
        /// <returns>Full-qualified namespace name</returns>
        internal string GetNamespace(bool distinguishAutoTypes = false)
        {
            var t = Type;
            if (t._IsEnum())
            {
                var te = Attr<TsEnumAttribute>();
                if (te == null) return t.Namespace;
                var ns = te.Namespace;
                if (!te.IncludeNamespace) return distinguishAutoTypes ? "-" : string.Empty;
                if (!string.IsNullOrEmpty(ns)) return ns;
                return t.Namespace;
            }
            var tc = Attr<TsClassAttribute>();
            var ti = Attr<TsInterfaceAttribute>();
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
        /// <param name="tr">Type resolver instance</param>
        /// <returns>Substitution AST</returns>
        public RtTypeName Substitute(Type t, TypeResolver tr)
        {
            Type genericDef = t._IsGenericType() ? t.GetGenericTypeDefinition() : null;
            if (Substitutions.ContainsKey(t)) return Substitutions[t];
            if (genericDef != null)
            {
                if (GenericSubstitutions.ContainsKey(genericDef))
                {
                    var r = GenericSubstitutions[genericDef];
                    var ts = r(t, tr);
                    if (ts != null) return ts;
                }
            }


            return null;
        }

        
    }
}
