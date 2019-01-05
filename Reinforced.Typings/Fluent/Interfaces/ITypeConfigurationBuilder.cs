using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    ///     Technical interface for type (class/interface) configuration builder
    /// </summary>
    public interface ITypeConfigurationBuilder : IReferenceConfigurationBuilder, IOrderableMember
    {

        /// <summary>
        /// Export context
        /// </summary>
        ExportContext Context { get; }

        /// <summary>
        ///     Type is being configured
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Substitutions to be used only when in this type
        /// </summary>
        Dictionary<Type, RtTypeName> Substitutions { get; }

        /// <summary>
        /// Substitutions to be used only when in this type
        /// </summary>
        Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GenericSubstitutions { get; }

        /// <summary>
        /// Gets whether type configuration is flatten
        /// </summary>
        bool IsHierarchyFlatten { get; }

        /// <summary>
        /// Flatten limiter
        /// </summary>
        Type FlattenLimiter { get; }

        /// <summary>
        /// Returns true, when hierarchy can be flatten. False otherwise
        /// </summary>
        /// <returns></returns>
        bool CanFlatten();

        /// <summary>
        /// Gets or sets whether type is third-party
        /// </summary>
        bool ThirdParty { get; set; }

        /// <summary>
        /// List of third-party imports
        /// </summary>
        List<RtImport> ThirdPartyImports { get; }

        /// <summary>
        /// List of third-party references
        /// </summary>
        List<RtReference> ThirdPartyReferences { get;  }
    }
}