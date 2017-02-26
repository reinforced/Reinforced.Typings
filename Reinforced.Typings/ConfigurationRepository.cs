using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings
{
    internal class ConfigurationRepository
    {
        public static ConfigurationRepository Instance
        {
            get { return _instance ?? (_instance = new ConfigurationRepository()); }
            set { _instance = value; }
        }

        
        #region private fields

        private readonly Dictionary<Type, TsDeclarationAttributeBase> _attributesForType =
            new Dictionary<Type, TsDeclarationAttributeBase>();

        private readonly Dictionary<MethodInfo, TsFunctionAttribute> _attributesForMethods =
            new Dictionary<MethodInfo, TsFunctionAttribute>();

        private readonly Dictionary<PropertyInfo, TsPropertyAttribute> _attributesForProperties =
            new Dictionary<PropertyInfo, TsPropertyAttribute>();

        private readonly Dictionary<FieldInfo, TsPropertyAttribute> _attributesForFields =
            new Dictionary<FieldInfo, TsPropertyAttribute>();

        private readonly Dictionary<FieldInfo, TsValueAttribute> _attributesForEnumValues =
            new Dictionary<FieldInfo, TsValueAttribute>();

        private readonly Dictionary<ParameterInfo, TsParameterAttribute> _attributesForParameters =
            new Dictionary<ParameterInfo, TsParameterAttribute>();

        private readonly Dictionary<Type, List<TsAddTypeReferenceAttribute>> _referenceAttributes =
            new Dictionary<Type, List<TsAddTypeReferenceAttribute>>();

        private readonly Dictionary<Type, string> _pathesToFiles = new Dictionary<Type, string>();
        private readonly Dictionary<string, List<Type>> _typesInFiles = new Dictionary<string, List<Type>>();

        private readonly HashSet<object> _ignored = new HashSet<object>();
        private static ConfigurationRepository _instance;
        private readonly List<RtReference> _references = new List<RtReference>();
        private readonly List<RtImport> _imports = new List<RtImport>();
        private readonly List<string> _additionalDocumentationPathes = new List<string>();

        #endregion

        #region Fileds frontend

        public Dictionary<Type, string> PathesToFiles
        {
            get { return _pathesToFiles; }
        }

        public Dictionary<string, List<Type>> TypesInFiles
        {
            get { return _typesInFiles; }
        }

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

        public List<RtReference> References
        {
            get { return _references; }
        }

        public List<RtImport> Imports
        {
            get { return _imports; }
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

        #endregion

        #region Division among files

        public void AddFileSeparationSettings(Type type, IReferenceConfigurationBuilder refs = null)
        {
            var refsList = ReferenceAttributes.GetOrCreate(type);
            var attrs = type.GetCustomAttributes<TsAddTypeReferenceAttribute>();
            if (attrs != null) refsList.AddRange(attrs);
            var fileAttr = type.GetCustomAttribute<TsFileAttribute>();
            if (fileAttr != null)
            {
                TrackTypeFile(type, fileAttr.FileName);
            }

            if (refs != null)
            {
                refsList.AddRange(refs.References);
                TrackTypeFile(type, refs.PathToFile);
            }
        }

        private void TrackTypeFile(Type t, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var typesPerFile = _typesInFiles.GetOrCreate(fileName);
            if (!typesPerFile.Contains(t)) typesPerFile.Add(t);
            _pathesToFiles[t] = fileName;
        }

        public string GetPathForFile(Type t)
        {
            if (_pathesToFiles.ContainsKey(t)) return _pathesToFiles[t];
            return null;
        }

        #endregion

        #region Attribute retrieve methods

        public TAttr ForType<TAttr>(Type t)
            where TAttr : TsDeclarationAttributeBase
        {
            return _attributesForType.GetOr(t, () => t.GetCustomAttribute<TAttr>(false)) as TAttr;
        }

        public TsDeclarationAttributeBase ForType(Type t)
        {
            return _attributesForType.GetOr(t, () => t.GetCustomAttribute<TsDeclarationAttributeBase>(false));
        }

        public TsFunctionAttribute ForMember(MethodInfo member)
        {
            return _attributesForMethods.GetOr(member, () => member.GetCustomAttribute<TsFunctionAttribute>(false));
        }

        public TsTypedMemberAttributeBase ForMember(MemberInfo member)
        {
            if (member is PropertyInfo) return ForMember((PropertyInfo) member);
            if (member is MethodInfo) return ForMember((MethodInfo) member);
            if (member is FieldInfo) return ForMember((FieldInfo) member);
            return null;
        }

        public T ForMember<T>(MemberInfo member) where T : TsTypedMemberAttributeBase
        {
            if (member is PropertyInfo) return (T) (object) ForMember((PropertyInfo) member);
            if (member is MethodInfo) return (T) (object) ForMember((MethodInfo) member);
            if (member is FieldInfo) return (T) (object) ForMember((FieldInfo) member);
            return null;
        }

        public TsParameterAttribute ForMember(ParameterInfo member)
        {
            return _attributesForParameters.GetOr(member, () => member.GetCustomAttribute<TsParameterAttribute>(false));
        }

        public TsPropertyAttribute ForMember(PropertyInfo member)
        {
            return _attributesForProperties.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
        }

        public TsPropertyAttribute ForMember(FieldInfo member)
        {
            return _attributesForFields.GetOr(member, () => member.GetCustomAttribute<TsPropertyAttribute>(false));
        }

        public TsBaseParamAttribute ForMember(ConstructorInfo member)
        {
            return member.GetCustomAttribute<TsBaseParamAttribute>(false);
        }

        public TsValueAttribute ForEnumValue(FieldInfo member)
        {
            return _attributesForEnumValues.GetOr(member, () => member.GetCustomAttribute<TsValueAttribute>(false));
        }

        #endregion

        #region Ignorance tracking methods

        public bool IsIgnored(Type member)
        {
            return ForType(member) == null;
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
            if (member is Type) return IsIgnored((Type) member);
            if (member is PropertyInfo) return IsIgnored((PropertyInfo) member);
            if (member is MethodInfo) return IsIgnored((MethodInfo) member);
            if (member is FieldInfo) return IsIgnored((FieldInfo) member);
            return false;
        }

        public bool IsIgnored(MethodInfo member)
        {
            return _ignored.Contains(member) || (member.GetCustomAttribute<TsIgnoreAttribute>(false) != null);
        }

        #endregion

        #region Determine what is exported

        public FieldInfo[] GetExportedFields(Type t)
        {
            if (IsIgnored(t)) return new FieldInfo[0];
            if (t.IsEnum) return new FieldInfo[0];

            var typeAttr = ForType(t);
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers =
                    t.GetFields(TypeExtensions.MembersFlags).Where(TypeExtensions.TypeScriptMemberSearchPredicate);
                if (!aexpSwith.AutoExportFields)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.OfType<FieldInfo>().ToArray();
            }
            return new FieldInfo[0];
        }

        public PropertyInfo[] GetExportedProperties(Type t)
        {
            if (IsIgnored(t)) return new PropertyInfo[0];
            if (t.IsEnum) return new PropertyInfo[0];

            var typeAttr = ForType(t);
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers =
                    t.GetProperties(TypeExtensions.MembersFlags).Where(TypeExtensions.TypeScriptMemberSearchPredicate);
                if (!aexpSwith.AutoExportProperties)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.OfType<PropertyInfo>().ToArray();
            }
            return new PropertyInfo[0];
        }

        public MethodInfo[] GetExportedMethods(Type t)
        {
            if (IsIgnored(t)) return new MethodInfo[0];
            if (t.IsEnum) return new MethodInfo[0];

            var typeAttr = ForType(t);
            var aexpSwith = typeAttr as IAutoexportSwitchAttribute;

            if (aexpSwith != null)
            {
                var allMembers =
                    t.GetMethods(TypeExtensions.MembersFlags).Where(TypeExtensions.TypeScriptMemberSearchPredicate);
                if (!aexpSwith.AutoExportMethods)
                {
                    allMembers = allMembers.Where(c => ForMember(c) != null);
                }
                return allMembers.OfType<MethodInfo>().Where(c => !c.IsSpecialName).ToArray();
            }
            return new MethodInfo[0];
        }

        #endregion

        
    }
}