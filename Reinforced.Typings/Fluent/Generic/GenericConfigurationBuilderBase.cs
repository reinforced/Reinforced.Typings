using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    abstract class GenericConfigurationBuilderBase : ITypeConfigurationBuilder
    {
        private readonly Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>> _membersConfiguration =
            new Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>>();

        private readonly Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>> _parametersConfiguration
            = new Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>>();

        private readonly ICollection<TsAddTypeReferenceAttribute> _references = new List<TsAddTypeReferenceAttribute>();

        private readonly Type _type;

        protected GenericConfigurationBuilderBase(Type t)
        {
            _type = t;
        }

        Type ITypeConfigurationBuilder.Type
        {
            get { return _type; }
        }

        Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>> ITypeConfigurationBuilder.
            ParametersConfiguration
        {
            get { return _parametersConfiguration; }
        }

        Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>> ITypeConfigurationBuilder.MembersConfiguration
        {
            get { return _membersConfiguration; }
        }

        ICollection<TsAddTypeReferenceAttribute> IReferenceConfigurationBuilder.References
        {
            get { return _references; }
        }

        string IReferenceConfigurationBuilder.PathToFile { get; set; }
        public abstract double MemberOrder { get; set; }
    }
}
