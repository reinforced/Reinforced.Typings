using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent configuration builder
    /// </summary>
    public class ConfigurationBuilder
    {
        private readonly Dictionary<Type, ITypeConfigurationBuilder> _typeConfigurationBuilders = new Dictionary<Type, ITypeConfigurationBuilder>();
        private readonly Dictionary<Type, IEnumConfigurationBuidler> _enumConfigurationBuilders = new Dictionary<Type, IEnumConfigurationBuidler>();
        private readonly List<string> _references = new List<string>();

        internal List<string> References
        {
            get { return _references; }
        }

        internal Dictionary<Type, ITypeConfigurationBuilder> TypeConfigurationBuilders
        {
            get { return _typeConfigurationBuilders; }
        }

        internal Dictionary<Type, IEnumConfigurationBuidler> EnumConfigurationBuilders
        {
            get { return _enumConfigurationBuilders; }
        }

        internal ConfigurationRepository Build()
        {
            ConfigurationRepository repository = new ConfigurationRepository();
            foreach (var kv in _typeConfigurationBuilders)
            {
                var cls = kv.Value as IClassConfigurationBuilder;
                var intrf = kv.Value as IInterfaceConfigurationBuilder;
                if (cls != null)
                {
                    repository.AttributesForType[kv.Key] = cls.AttributePrototype;
                }

                if (intrf != null)
                {
                    repository.AttributesForType[kv.Key] = intrf.AttributePrototype;
                }

                foreach (var kvm in kv.Value.MembersConfiguration)
                {
                    if (kvm.Value.CheckIgnored())
                    {
                        repository.Ignored.Add(kvm.Key);
                        continue;
                    }
                    var prop = kvm.Key as PropertyInfo;
                    var field = kvm.Key as FieldInfo;
                    var method = kvm.Key as MethodInfo;
                    if (prop != null)
                    {
                        repository.AttributesForProperties[prop] = (TsPropertyAttribute)kvm.Value.AttributePrototype;
                    }
                    if (field != null)
                    {
                        repository.AttributesForFields[field] = (TsPropertyAttribute)kvm.Value.AttributePrototype;
                    }
                    if (method != null)
                    {
                        repository.AttributesForMethods[method] = (TsFunctionAttribute)kvm.Value.AttributePrototype;
                    }
                }
                foreach (var kvp in kv.Value.ParametersConfiguration)
                {
                    if (kvp.Value.CheckIgnored())
                    {
                        repository.Ignored.Add(kvp.Key);
                        continue;
                    }
                    repository.AttributesForParameters[kvp.Key] = kvp.Value.AttributePrototype;
                }
                AddReferences(repository, kv.Key, kv.Value);
            }
            foreach (var kv in _enumConfigurationBuilders)
            {
                repository.AttributesForType[kv.Key] = kv.Value.AttributePrototype;
                foreach (var enumValueExportConfiguration in kv.Value.ValueExportConfigurations)
                {
                    repository.AttributesForEnumValues[enumValueExportConfiguration.Key] =
                        enumValueExportConfiguration.Value.AttributePrototype;
                }
                AddReferences(repository,kv.Key,kv.Value);
            }
            repository.References.AddRange(_references);
            return repository;
        }

        private static void AddReferences(ConfigurationRepository repository,Type type,IReferenceConfigurationBuilder refs)
        {
            var refsList = repository.ReferenceAttributes.GetOrCreate(type);
            refsList.AddRange(refs.References);
            var attrs = type.GetCustomAttributes<TsAddTypeReferenceAttribute>();
            if (attrs != null) refsList.AddRange(attrs);
        }
    }
}
