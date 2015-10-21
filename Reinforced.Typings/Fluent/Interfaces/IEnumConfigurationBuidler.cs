using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Technical interface for enumeration configuration builder
    /// </summary>
    public interface IEnumConfigurationBuidler : IExportConfiguration<TsEnumAttribute>, IReferenceConfigurationBuilder
    {
        /// <summary>
        /// Type of enumeration
        /// </summary>
        Type EnumType { get; }
        
        /// <summary>
        /// Configurations for exported particular enumeration values
        /// </summary>
        Dictionary<FieldInfo, EnumValueExportConfiguration> ValueExportConfigurations { get; }
    }
}