using System;
using System.Collections.Generic;
using System.Reflection;
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
        ///     Configurations for method parameters
        /// </summary>
        Dictionary<ParameterInfo, IAttributed<TsParameterAttribute>> ParametersConfiguration { get; }

        /// <summary>
        ///     Configurations for type members
        /// </summary>
        Dictionary<MemberInfo, IAttributed<TsAttributeBase>> MembersConfiguration { get; }

        /// <summary>
        ///     Type is being configured
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Substitutions to be used only when in this type
        /// </summary>
        Dictionary<Type, RtTypeName> Substitutions { get; }

        /// <summary>
        /// Gets whether type configuration is flattern
        /// </summary>
        bool IsHierarchyFlattern { get; }

        /// <summary>
        /// Flattern limiter
        /// </summary>
        Type FlatternLimiter { get; }
    }
}