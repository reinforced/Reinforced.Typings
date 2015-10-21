using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    internal class ConfigurationRepository
    {
        public static ConfigurationRepository Instance
        {
            get { return _instance ?? (_instance = new ConfigurationRepository()); }
            set { _instance = value; }
        }

        private readonly Dictionary<Type, TsDeclarationAttributeBase> _attributesForType = new Dictionary<Type, TsDeclarationAttributeBase>();
        private readonly Dictionary<MethodInfo, TsFunctionAttribute> _attributesForMethods = new Dictionary<MethodInfo, TsFunctionAttribute>();
        private readonly Dictionary<PropertyInfo, TsPropertyAttribute> _attributesForProperties = new Dictionary<PropertyInfo, TsPropertyAttribute>();
        private readonly Dictionary<FieldInfo, TsPropertyAttribute> _attributesForFields = new Dictionary<FieldInfo, TsPropertyAttribute>();
        private readonly Dictionary<FieldInfo, TsValueAttribute> _attributesForEnumValues = new Dictionary<FieldInfo, TsValueAttribute>();
        private readonly Dictionary<ParameterInfo, TsParameterAttribute> _attributesForParameters = new Dictionary<ParameterInfo, TsParameterAttribute>();
        private readonly Dictionary<Type,List<TsAddTypeReferenceAttribute>> _referenceAttributes = new Dictionary<Type, List<TsAddTypeReferenceAttribute>>();

        private readonly HashSet<object> _ignored = new HashSet<object>();
        private static ConfigurationRepository _instance;
        private readonly List<string> _references = new List<string>();
        private readonly List<string> _additionalDocumentationPathes = new List<string>();

        public List<string> AdditionalDocumentationPathes
        {
            get { return _additionalDocumentationPathes; }
        }

        public List<TsAddTypeReferenceAttribute> ReferencesForType(Type t)
        {
            return _referenceAttributes.GetOr(t, () => new List<TsAddTypeReferenceAttribute>());
        }

        public Dictionary<Type, List<TsAddTypeReferenceAttribute>> ReferenceAttributes
        {
            get { return _referenceAttributes; }
        }

        public List<string> References
        {
            get { return _references; }
        }

        public Dictionary<ParameterInfo, TsParameterAttribute> AttributesForParameters
        {
            get { return _attributesForParameters; }
        }

        public HashSet<object> Ignored
        {
            get { return _ignored; }
        }

        public Dictionary<Type, TsDeclarationAttributeBase> AttributesForType
        {
            get { return _attributesForType; }
        }

        public Dictionary<MethodInfo, TsFunctionAttribute> AttributesForMethods
        {
            get { return _attributesForMethods; }
        }

        public Dictionary<PropertyInfo, TsPropertyAttribute> AttributesForProperties
        {
            get { return _attributesForProperties; }
        }

        public Dictionary<FieldInfo, TsPropertyAttribute> AttributesForFields
        {
            get { return _attributesForFields; }
        }

        public Dictionary<FieldInfo, TsValueAttribute> AttributesForEnumValues
        {
            get { return _attributesForEnumValues; }
        }

        public TAttr ForType<TAttr>(Type t)
            where TAttr : TsDeclarationAttributeBase
        {
            return _attributesForType.GetOr(t, ()=>t.GetCustomAttribute<TAttr>(false)) as TAttr;
        }

        public TsDeclarationAttributeBase ForType(Type t)
        {
            return _attributesForType.GetOr(t, () => t.GetCustomAttribute<TsDeclarationAttributeBase>(false));
        }

        public TsFunctionAttribute ForMember(MethodInfo member)
        {
            return _attributesForMethods.GetOr(member, () => member.GetCustomAttribute<TsFunctionAttribute>(false));
        }

        public T ForMember<T>(MemberInfo member) where T : TsTypedMemberAttributeBase
        {
            if (member is PropertyInfo) return (T)(object)ForMember((PropertyInfo)member);
            if (member is MethodInfo) return (T)(object)ForMember((MethodInfo)member);
            if (member is FieldInfo) return (T)(object)ForMember((FieldInfo)member);
            return null;
        }

        public bool IsIgnored(MemberInfo member)
        {
            if (member is PropertyInfo) return IsIgnored((PropertyInfo) member);
            if (member is MethodInfo) return IsIgnored((MethodInfo)member);
            if (member is FieldInfo) return IsIgnored((FieldInfo)member);
            return false;
        }
        public bool IsIgnored(MethodInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        public TsParameterAttribute ForMember(ParameterInfo member)
        {
            return _attributesForParameters.GetOr(member, () => member.GetCustomAttribute<TsParameterAttribute>(false));
        }

        public bool IsIgnored(ParameterInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public TsPropertyAttribute ForMember(PropertyInfo member)
        {
            return _attributesForProperties.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
        }

        public bool IsIgnored(PropertyInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public TsPropertyAttribute ForMember(FieldInfo member)
        {
            return _attributesForFields.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
        }

        public bool IsIgnored(FieldInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>() != null);
        }

        public TsBaseParamAttribute ForMember(ConstructorInfo member)
        {
            return member.GetCustomAttribute<TsBaseParamAttribute>(false);
        }

        public bool IsIgnored(ConstructorInfo member)
        {
            return (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        public TsValueAttribute ForEnumValue(FieldInfo member)
        {
            return _attributesForEnumValues.GetOr(member, () => member.GetCustomAttribute<TsValueAttribute>(false));
        }
    }
}
