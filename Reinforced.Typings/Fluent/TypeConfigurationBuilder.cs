using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration builder for type
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public abstract class TypeConfigurationBuilder<TType> : ITypeConfigurationBuilder
    {
        private readonly Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>> _membersConfiguration =
            new Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>>();

        private readonly Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>> _parametersConfiguration
            = new Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>>();

        private readonly ICollection<TsAddTypeReferenceAttribute> _references = new List<TsAddTypeReferenceAttribute>();
        private readonly Dictionary<Type, RtTypeName> _substitutions = new Dictionary<Type, RtTypeName>();
        private readonly ICollection<TsAddTypeImportAttribute> _imports = new List<TsAddTypeImportAttribute>();

        Type ITypeConfigurationBuilder.Type
        {
            get { return typeof (TType); }
        }

        public Dictionary<Type, RtTypeName> Substitutions { get { return _substitutions; } }

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

        ICollection<TsAddTypeImportAttribute> IReferenceConfigurationBuilder.Imports
        {
            get { return _imports; }
        }

        string IReferenceConfigurationBuilder.PathToFile { get; set; }

        public abstract double MemberOrder { get; set; }
    }
}