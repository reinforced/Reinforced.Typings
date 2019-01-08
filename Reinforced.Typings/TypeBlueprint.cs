using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
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
        internal TypeBlueprint(Type t)
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
            ThirdPartyImports = new List<RtImport>();
            ThirdPartyReferences = new List<RtReference>();
            TypeAttribute = Type.GetCustomAttribute<TsEnumAttribute>(false)
                            ?? Type.GetCustomAttribute<TsInterfaceAttribute>(false)
                            ?? (TsDeclarationAttributeBase)Type.GetCustomAttribute<TsClassAttribute>(false);
            _thirdPartyAttribute = Type.GetCustomAttribute<TsThirdPartyAttribute>();
            InitFromAttributes();
        }

        private bool _flattenTouched = false;
        internal void NotifyFlattenTouched()
        {
            _flattenTouched = true;
        }

        /// <summary>
        /// Path to file that blueprinted type will be exported to
        /// </summary>
        public string PathToFile { get; internal set; }

        private void InitFromAttributes()
        {
            var typeRefs = Type.GetCustomAttributes<TsAddTypeReferenceAttribute>();
            References.AddRange(typeRefs);
            var typeImports = Type.GetCustomAttributes<TsAddTypeImportAttribute>();
            Imports.AddRange(typeImports);

            InitThirdPartyImports();
            
        }

        private void InitThirdPartyImports()
        {
            if (ThirdParty != null)
            {
                ThirdPartyReferences.Clear();
                ThirdPartyImports.Clear();

                var tpRefs = Type.GetCustomAttributes<TsThirdPartyReferenceAttribute>();
                foreach (var a in tpRefs)
                {
                    ThirdPartyReferences.Add(a.ToReference());
                }

                var tpImpots = Type.GetCustomAttributes<TsThirdPartyImportAttribute>();
                foreach (var a in tpImpots)
                {
                    ThirdPartyImports.Add(a.ToImport());
                }
            }
        }

        /// <summary>
        ///     Type is being exported
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Attribute for exporting class
        /// </summary>
        public TsDeclarationAttributeBase TypeAttribute
        {
            get { return _typeAttribute; }
            internal set
            {
                _typeAttribute = value;
                InitFromAttributes();
            }
        }

        #region Third-Party type handling
        private TsThirdPartyAttribute _thirdPartyAttribute;
        internal List<RtImport> ThirdPartyImports { get; private set; }
        internal List<RtReference> ThirdPartyReferences { get; private set; }

        /// <summary>
        /// Gets whether type is used as third-party type only without export being performed
        /// </summary>
        public TsThirdPartyAttribute ThirdParty
        {
            get { return _thirdPartyAttribute; }
            set
            {
                _thirdPartyAttribute = value;
                InitFromAttributes();
            }
        }
        #endregion

        private readonly HashSet<object> _ignored = new HashSet<object>();

        /// <summary>
        /// Set of ignored members
        /// </summary>
        internal HashSet<object> Ignored
        {
            get { return _ignored; }
            private set { }
        }

        private readonly Dictionary<PropertyInfo, TsPropertyAttribute> _attributesForProperties;
        private readonly Dictionary<MethodInfo, TsFunctionAttribute> _attributesForMethods;
        private readonly Dictionary<ParameterInfo, TsParameterAttribute> _attributesForParameters;
        private readonly Dictionary<FieldInfo, TsPropertyAttribute> _attributesForFields;
        private readonly Dictionary<FieldInfo, TsValueAttribute> _attributesForEnumValues;
        private TsDeclarationAttributeBase _typeAttribute;

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

        /// <summary>
        /// Retrieves configuration attribute for type member
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsTypedMemberAttributeBase ForMember(MemberInfo member, bool create = false)
        {
            if (member is PropertyInfo) return ForMember((PropertyInfo)member, create);
            if (member is MethodInfo) return ForMember((MethodInfo)member, create);
            if (member is FieldInfo) return ForMember((FieldInfo)member, create);
            return null;
        }

        /// <summary>
        /// Retrieves configuration attribute for type member
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public T ForMember<T>(MemberInfo member, bool create = false) where T : TsTypedMemberAttributeBase
        {
            if (member is PropertyInfo) return (T)(object)ForMember((PropertyInfo)member, create);
            if (member is MethodInfo) return (T)(object)ForMember((MethodInfo)member, create);
            if (member is FieldInfo) return (T)(object)ForMember((FieldInfo)member, create);
            return null;
        }


        /// <summary>
        /// Retrieves configuration attribute for method
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsFunctionAttribute ForMember(MethodInfo member, bool create = false)
        {
            var v = _attributesForMethods.GetOr(member, () => member.GetCustomAttribute<TsFunctionAttribute>(false));
            if (create && v == null)
            {
                return _attributesForMethods.GetOrCreate(member);
            }
            return v;
        }

        /// <summary>
        /// Retrieves configuration attribute for method parameter
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsParameterAttribute ForMember(ParameterInfo member, bool create = false)
        {
            var v = _attributesForParameters.GetOr(member, () => member.GetCustomAttribute<TsParameterAttribute>(false));
            if (create && v == null)
            {
                return _attributesForParameters.GetOrCreate(member);
            }
            return v;
        }

        /// <summary>
        /// Retrieves configuration attribute for property
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsPropertyAttribute ForMember(PropertyInfo member, bool create = false)
        {
            var v = _attributesForProperties.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
            if (create && v == null)
            {
                return _attributesForProperties.GetOrCreate(member);
            }
            return v;
        }

        /// <summary>
        /// Retrieves configuration attribute for field
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsPropertyAttribute ForMember(FieldInfo member, bool create = false)
        {
            var v = _attributesForFields.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
            if (create && v == null)
            {
                return _attributesForFields.GetOrCreate(member);
            }
            return v;
        }

        /// <summary>
        /// Retrieves configuration attribute for constructor
        /// </summary>
        /// <param name="member">Type member</param>
        /// <returns>Configuration attribute</returns>
        public TsBaseParamAttribute ForMember(ConstructorInfo member)
        {
            return member.GetCustomAttribute<TsBaseParamAttribute>(false);
        }

        /// <summary>
        /// Retrieves configuration attribute for enum value
        /// </summary>
        /// <param name="member">Type member</param>
        /// <param name="create">When true, attribute will be created if not exists</param>
        /// <returns>Configuration attribute</returns>
        public TsValueAttribute ForEnumValue(FieldInfo member, bool create = false)
        {
            var v = _attributesForEnumValues.GetOr(member, () => member.GetCustomAttribute<TsValueAttribute>(false));
            if (create && v == null)
            {
                return _attributesForEnumValues.GetOrCreate(member);
            }
            return v;
        }

        /// <summary>
        /// Retrieves type's exported fields
        /// </summary>
        /// <returns>Array of exported fields</returns>
        public FieldInfo[] GetExportedFields()
        {
            var t = Type;
            if (IsIgnored()) return new FieldInfo[0];
            if (t._IsEnum()) return new FieldInfo[0];


            var typeAttr = TypeAttribute;
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

        /// <summary>
        /// Retrieves type's exported properties
        /// </summary>
        /// <returns>Array of exported properties</returns>
        public PropertyInfo[] GetExportedProperties()
        {
            var t = Type;
            if (IsIgnored()) return new PropertyInfo[0];
            if (t._IsEnum()) return new PropertyInfo[0];

            var typeAttr = TypeAttribute;
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

        /// <summary>
        /// Retrieves type's exported methods
        /// </summary>
        /// <returns>Array of exported methods</returns>
        public MethodInfo[] GetExportedMethods()
        {
            var t = Type;
            if (IsIgnored()) return new MethodInfo[0];
            if (t._IsEnum()) return new MethodInfo[0];

            var typeAttr = TypeAttribute;
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

        /// <summary>
        /// Checks whether type is ignored during export
        /// </summary>
        /// <returns>True, when type is ignored. False otherwise</returns>
        private bool IsIgnored()
        {
            return TypeAttribute == null;
        }

        /// <summary>
        /// Checks whether constructor is ignored during export
        /// </summary>
        /// <returns>True, when constructor is ignored. False otherwise</returns>
        public bool IsIgnored(ConstructorInfo member)
        {
            return (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        /// <summary>
        /// Checks whether field is ignored during export
        /// </summary>
        /// <returns>True, when field is ignored. False otherwise</returns>
        public bool IsIgnored(FieldInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        /// <summary>
        /// Checks whether property is ignored during export
        /// </summary>
        /// <returns>True, when property is ignored. False otherwise</returns>
        public bool IsIgnored(PropertyInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        /// <summary>
        /// Checks whether parameter is ignored during export
        /// </summary>
        /// <returns>True, when parameter is ignored. False otherwise</returns>
        public bool IsIgnored(ParameterInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        /// <summary>
        /// Checks whether type member is ignored during export
        /// </summary>
        /// <returns>True, when type member is ignored. False otherwise</returns>
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

        /// <summary>
        /// Checks whether method is ignored during export
        /// </summary>
        /// <returns>True, when method is ignored. False otherwise</returns>
        public bool IsIgnored(MethodInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        #endregion


        internal List<TsDecoratorAttribute> DecoratorsListFor(MemberInfo t)
        {
            return DecoratorsForMembers.GetOrCreate(t);
        }

        internal List<TsDecoratorAttribute> DecoratorsListFor(ParameterInfo t)
        {
            return DecoratorsForParameters.GetOrCreate(t);
        }



        /// <summary>
        /// Retrieves decorators for type
        /// </summary>
        /// <returns>Set of type's decorators</returns>
        public IEnumerable<TsDecoratorAttribute> GetDecorators()
        {
            var inlineDecorators = Type.GetCustomAttributes<TsDecoratorAttribute>();

            return inlineDecorators.Union(Decorators);
        }

        /// <summary>
        /// Retrieves set of decorators for method parameter
        /// </summary>
        /// <param name="t">Method's parameter</param>
        /// <returns>Set of decorators</returns>
        public IEnumerable<TsDecoratorAttribute> DecoratorsFor(ParameterInfo t)
        {
            var inlineDecorators = t.GetCustomAttributes<TsDecoratorAttribute>();
            var fluentDecorators = DecoratorsForParameters.ContainsKey(t) ? DecoratorsForParameters[t] : null;

            if (fluentDecorators == null) return inlineDecorators;
            return inlineDecorators.Union(fluentDecorators);
        }

        /// <summary>
        /// Retrieves set of decorators for type's member
        /// </summary>
        /// <param name="t">Member's parameter</param>
        /// <returns>Set of decorators</returns>
        public IEnumerable<TsDecoratorAttribute> DecoratorsFor(MemberInfo t)
        {
            var inlineDecorators = t.GetCustomAttributes<TsDecoratorAttribute>();
            var fluentDecorators = DecoratorsForMembers.ContainsKey(t) ? DecoratorsForMembers[t] : null;

            if (fluentDecorators == null) return inlineDecorators;
            return inlineDecorators.Union(fluentDecorators);
        }

        /// <summary>
        /// Retrieves type's configuration attribute
        /// </summary>
        /// <typeparam name="T">Attribute type. Subclass of <see cref="TsDeclarationAttributeBase"/></typeparam>
        /// <returns>Configuration attribute. May be null</returns>
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

        /// <summary>
        /// Converts string to camelCase
        /// </summary>
        /// <param name="s">Source string in any case</param>
        /// <returns>Resulting string in camelCase</returns>
        public static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // ABCa -> abCa
                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
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
            string name = null;
            if (ThirdParty == null)
            {
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
                name = (!string.IsNullOrEmpty(nameFromAttr) ? nameFromAttr : t.CleanGenericName());
                if (ti != null)
                {
                    if (ti.AutoI)
                    {
                        if (t._IsClass()) name = "I" + name;
                        else if (!name.StartsWith("I")) name = "I" + name;
                    }
                }
            }
            else
            {
                name = ThirdParty.Name;
            }

            if (genericArguments == null) genericArguments = t.SerializeGenericArguments();

            
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
            var nsFromAttr = tc != null ? tc.Namespace : ti.Namespace;
            var includeNamespace = tc != null ? tc.IncludeNamespace : ti.IncludeNamespace;

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


        /// <summary>
        /// Gets whether type hierarchy can be flatten
        /// </summary>
        /// <returns></returns>
        public bool CanFlatten()
        {
            if (_flattenTouched) return false;
            if (_attributesForProperties.Count > 0) return false;
            if (_attributesForParameters.Count > 0) return false;
            if (_attributesForFields.Count > 0) return false;
            if (_attributesForMethods.Count > 0) return false;
            if (_attributesForEnumValues.Count > 0) return false;
            return true;
        }

    }
}
