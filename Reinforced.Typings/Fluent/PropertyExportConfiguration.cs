using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported properties
    /// </summary>
    public class PropertyExportConfiguration : IExportConfiguration<TsPropertyAttribute>, IIgnorable, IDecoratorsAggregator
    {
        internal PropertyExportConfiguration()
        {
            AttributePrototype = new TsPropertyAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsPropertyAttribute AttributePrototype { get; set; }

        TsPropertyAttribute IExportConfiguration<TsPropertyAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
        public List<TsDecoratorAttribute> Decorators { get; }
    }
}