using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Configuration builder for interface
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class InterfaceConfigurationBuilder<TInterface> : TypeConfigurationBuilder<TInterface>, IInterfaceConfigurationBuilder
    {
        private TsInterfaceAttribute AttributePrototype { get; set; }

        TsInterfaceAttribute IExportConfiguration<TsInterfaceAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal InterfaceConfigurationBuilder()
        {
            AttributePrototype = new TsInterfaceAttribute()
            {
                AutoExportProperties = false,
                AutoExportMethods = false
            };
        }
    }
}
