using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
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
        internal readonly TypeBlueprint _blueprint;

        internal TypeConfigurationBuilder(ExportContext context)
        {
            _blueprint = context.Project.Blueprint(typeof(TType));
            Context = context;
        }

        /// <summary>
        /// Export context
        /// </summary>
        public ExportContext Context { get; private set; }

        Type ITypeConfigurationBuilder.Type
        {
            get { return typeof(TType); }
        }


        Dictionary<Type, RtTypeName> ITypeConfigurationBuilder.Substitutions
        {
            get { return _blueprint.Substitutions; }
        }

        /// <summary>
        /// Substitutions to be used only when in this type
        /// </summary>
        public Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GenericSubstitutions
        {
            get { return _blueprint.GenericSubstitutions; }
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
        /// Gets or sets whether type is third-party
        /// </summary>
        public bool ThirdParty
        {
            get { return _blueprint.IsThirdParty; }
            set { _blueprint.IsThirdParty = value; }
        }

        /// <summary>
        /// List of third-party imports
        /// </summary>
        public List<RtImport> ThirdPartyImports
        {
            get { return _blueprint.ThirdPartyImports; }
        }

        /// <summary>
        /// List of third-party references
        /// </summary>
        public List<RtReference> ThirdPartyReferences
        {
            get { return _blueprint.ThirdPartyReferences; }
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

        /// <inheritdoc />
        public abstract double MemberOrder { get; set; }
    }
}