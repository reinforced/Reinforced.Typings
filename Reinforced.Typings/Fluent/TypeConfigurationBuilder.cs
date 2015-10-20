using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Configuration builder for type
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public abstract class TypeConfigurationBuilder<TType> : ITypeConfigurationBuilder
    {
        Type ITypeConfigurationBuilder.Type { get { return typeof (TType); } }

        private readonly Dictionary<MemberInfo,IExportConfiguration<TsAttributeBase>> _membersConfiguration = new Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>>();
        private readonly Dictionary<ParameterInfo,IExportConfiguration<TsParameterAttribute>> _parametersConfiguration = new Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>>();


        Dictionary<ParameterInfo, IExportConfiguration<TsParameterAttribute>> ITypeConfigurationBuilder.ParametersConfiguration
        {
            get { return _parametersConfiguration; }
        }


        Dictionary<MemberInfo, IExportConfiguration<TsAttributeBase>> ITypeConfigurationBuilder.MembersConfiguration
        {
            get { return _membersConfiguration; }
        }
    }
}
