using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IEnumConfigurationBuidler : IExportConfiguration<TsEnumAttribute>, IReferenceConfiguration
    {
        Type EnumType { get; }
        Dictionary<FieldInfo, EnumValueExportConfiguration> ValueExportConfigurations { get; }
    }
}