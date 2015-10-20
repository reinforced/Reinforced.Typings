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
    /// Configuration builder for Enum type
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumConfigurationBuilder<TEnum> : IEnumConfigurationBuidler
        where TEnum:struct
    {
        private readonly Dictionary<FieldInfo,EnumValueExportConfiguration> _valueExportConfigurations = new Dictionary<FieldInfo, EnumValueExportConfiguration>();

        Dictionary<FieldInfo, EnumValueExportConfiguration> IEnumConfigurationBuidler.ValueExportConfigurations
        {
            get { return _valueExportConfigurations; }
        }

        private TsEnumAttribute AttributePrototype { get; set; }

        TsEnumAttribute IExportConfiguration<TsEnumAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal EnumConfigurationBuilder()
        {
            AttributePrototype = new TsEnumAttribute();
        }

        Type IEnumConfigurationBuidler.EnumType { get { return typeof (TEnum); } }
    }
}
