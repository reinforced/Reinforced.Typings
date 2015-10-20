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
    /// Export configuration builder for class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassConfigurationBuilder<T> : TypeConfigurationBuilder<T>, IClassConfigurationBuilder
    {
        private TsClassAttribute AttributePrototype { get; set; }

        TsClassAttribute IExportConfiguration<TsClassAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal ClassConfigurationBuilder()
        {
            AttributePrototype = new TsClassAttribute()
            {
                AutoExportConstructors = false,
                AutoExportFields = false,
                AutoExportProperties = false,
                AutoExportMethods = false
            };
        }
    }
}
