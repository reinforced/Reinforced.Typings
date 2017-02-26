using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported methods
    /// </summary>
    public class MethodConfigurationBuilder : IExportConfiguration<TsFunctionAttribute>, IIgnorable, IDecoratorsAggregator
    {
        internal MethodConfigurationBuilder()
        {
            AttributePrototype = new TsFunctionAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsFunctionAttribute AttributePrototype { get; set; }

        TsFunctionAttribute IExportConfiguration<TsFunctionAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
        public List<TsDecoratorAttribute> Decorators { get; }
    }
}