using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    ///     Technical interface for enumeration configuration builder
    /// </summary>
    public interface IEnumConfigurationBuidler : IAttributed<TsEnumAttribute>, IReferenceConfigurationBuilder,
        IDecoratorsAggregator
    {
        /// <summary>
        ///     Type of enumeration
        /// </summary>
        Type EnumType { get; }

        /// <summary>
        /// Export context
        /// </summary>
        ExportContext Context { get; }
        
    }
}