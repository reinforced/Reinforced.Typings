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
        private readonly Dictionary<Type,TsDeclarationAttributeBase> _attributesForType = new Dictionary<Type, TsDeclarationAttributeBase>();
        private readonly Dictionary<MethodInfo, TsFunctionAttribute> _attributesForMethods = new Dictionary<MethodInfo, TsFunctionAttribute>();
        private readonly Dictionary<PropertyInfo,TsPropertyAttribute> _attributesForProperties = new Dictionary<PropertyInfo, TsPropertyAttribute>();
        private readonly Dictionary<FieldInfo, TsPropertyAttribute> _attributesForFields = new Dictionary<FieldInfo, TsPropertyAttribute>();
        private readonly Dictionary<FieldInfo, TsValueAttribute> _attributesForEnumValues = new Dictionary<FieldInfo, TsValueAttribute>();
        private readonly Dictionary<ParameterInfo, TsParameterAttribute> _attributesForParameters = new Dictionary<ParameterInfo, TsParameterAttribute>();
        private readonly HashSet<object> _ignored = new HashSet<object>();

        internal Dictionary<ParameterInfo, TsParameterAttribute> AttributesForParameters
        {
            get { return _attributesForParameters; }
        }

        internal HashSet<object> Ignored
        {
            get { return _ignored; }
        }

        internal Dictionary<Type, TsDeclarationAttributeBase> AttributesForType
        {
            get { return _attributesForType; }
        }

        internal Dictionary<MethodInfo, TsFunctionAttribute> AttributesForMethods
        {
            get { return _attributesForMethods; }
        }

        internal Dictionary<PropertyInfo, TsPropertyAttribute> AttributesForProperties
        {
            get { return _attributesForProperties; }
        }

        internal Dictionary<FieldInfo, TsPropertyAttribute> AttributesForFields
        {
            get { return _attributesForFields; }
        }

        internal Dictionary<FieldInfo, TsValueAttribute> AttributesForEnumValues
        {
            get { return _attributesForEnumValues; }
        }

        public TAttr ForType<TAttr>(Type t)
            where TAttr : TsDeclarationAttributeBase
        {
            return _attributesForType.GetOr(t,t.GetCustomAttribute<TAttr>) as TAttr;
        }

        public TsDeclarationAttributeBase ForType(Type t)
        {
            return _attributesForType.GetOr(t,t.GetCustomAttribute<TsDeclarationAttributeBase>);
        }

        public TsFunctionAttribute ForMember(MethodInfo member)
        {
            return _attributesForMethods.GetOr(member,member.GetCustomAttribute<TsFunctionAttribute>);
        }

        public TsParameterAttribute ForMember(ParameterInfo member)
        {
            return _attributesForParameters.GetOr(member,member.GetCustomAttribute<TsParameterAttribute>);
        }

        public TsPropertyAttribute ForMember(PropertyInfo member)
        {
            return _attributesForProperties.GetOr(member,member.GetCustomAttribute<TsPropertyAttribute>);
        }

        public TsPropertyAttribute ForMember(FieldInfo member)
        {
            return _attributesForFields.GetOr(member, member.GetCustomAttribute<TsPropertyAttribute>);
        }

        public TsBaseParamAttribute ForMember(ConstructorInfo member)
        {
            return member.GetCustomAttribute<TsBaseParamAttribute>();
        }

        public TsValueAttribute ForEnumValue(FieldInfo member)
        {
            return _attributesForEnumValues.GetOr(member, member.GetCustomAttribute<TsValueAttribute>);
        }
    }
}
