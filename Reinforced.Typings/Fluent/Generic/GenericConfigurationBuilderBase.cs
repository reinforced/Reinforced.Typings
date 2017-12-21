using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    abstract class GenericConfigurationBuilderBase : ITypeConfigurationBuilder
    {
        private readonly ICollection<TsAddTypeImportAttribute> _imports = new List<TsAddTypeImportAttribute>();

        private readonly Dictionary<MemberInfo, IAttributed<TsAttributeBase>> _membersConfiguration =
            new Dictionary<MemberInfo, IAttributed<TsAttributeBase>>();

        private readonly Dictionary<ParameterInfo, IAttributed<TsParameterAttribute>> _parametersConfiguration
            = new Dictionary<ParameterInfo, IAttributed<TsParameterAttribute>>();

        private readonly ICollection<TsAddTypeReferenceAttribute> _references = new List<TsAddTypeReferenceAttribute>();

        private readonly Type _type;

        protected GenericConfigurationBuilderBase(Type t)
        {
            _type = t;
            Substitutions = new Dictionary<Type, RtTypeName>();
        }

        private Dictionary<Type, RtTypeName> Substitutions { get; set; }

        /// <summary>
        /// Gets whether type configuration is flattern
        /// </summary>
        public abstract bool IsHierarchyFlattern { get; }

        /// <summary>
        /// Flattern limiter
        /// </summary>
        public abstract Type FlatternLimiter { get; }

        Type ITypeConfigurationBuilder.Type
        {
            get { return _type; }
        }

        Dictionary<Type, RtTypeName> ITypeConfigurationBuilder.Substitutions
        {
            get { return Substitutions; }
        }

        Dictionary<ParameterInfo, IAttributed<TsParameterAttribute>> ITypeConfigurationBuilder.
            ParametersConfiguration
        {
            get { return _parametersConfiguration; }
        }

        Dictionary<MemberInfo, IAttributed<TsAttributeBase>> ITypeConfigurationBuilder.MembersConfiguration
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