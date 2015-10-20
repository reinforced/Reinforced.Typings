using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface ITypeConfigurationBuilder : IReferenceConfiguration
    {
        Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>> ParametersConfiguration { get; }
        Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>> MembersConfiguration { get; }
        Type Type { get; } 
    }
}