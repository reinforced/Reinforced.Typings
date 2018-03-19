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
        internal readonly TypeBlueprint _blueprint;

        protected GenericConfigurationBuilderBase(TypeBlueprint blueprint, ExportContext context)
        {
            _blueprint = blueprint;
            Context = context;
        }

        /// <summary>
        /// Substitutions to be used only when in this type
        /// </summary>
        public Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GenericSubstitutions
        {
            get
            {
                return _blueprint.GenericSubstitutions;
            }
        }

        /// <summary>
        /// Gets whether type configuration is flatten
        /// </summary>
        public abstract bool IsHierarchyFlatten { get; }

        /// <summary>
        /// Flatten limiter
        /// </summary>
        public abstract Type FlattenLimiter { get; }

        /// <summary>
        /// Returns true, when hierarchy can be flatten. False otherwise
        /// </summary>
        /// <returns></returns>
        public bool CanFlatten()
        {
            return _blueprint.CanFlatten();
        }

        /// <summary>
        /// Export context
        /// </summary>
        public ExportContext Context { get; }

        Type ITypeConfigurationBuilder.Type
        {
            get { return _blueprint.Type; }
        }

        Dictionary<Type, RtTypeName> ITypeConfigurationBuilder.Substitutions
        {
            get { return _blueprint.Substitutions; }
        }

        ICollection<TsAddTypeReferenceAttribute> IReferenceConfigurationBuilder.References
        {
            get { return _blueprint.References; }
        }

        ICollection<TsAddTypeImportAttribute> IReferenceConfigurationBuilder.Imports
        {
            get { return _blueprint.Imports; }
        }

        string IReferenceConfigurationBuilder.PathToFile
        {
            get { return _blueprint.PathToFile; }
            set { _blueprint.PathToFile = value; }
        }
        public abstract double MemberOrder { get; set; }
    }
}