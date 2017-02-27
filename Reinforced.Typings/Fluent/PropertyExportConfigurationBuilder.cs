using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported properties
    /// </summary>
    public class PropertyExportConfigurationBuilder : IExportConfiguration<TsPropertyAttribute>, IIgnorable,
        IDecoratorsAggregator, IOrderableMember
    {
        internal PropertyExportConfigurationBuilder()
        {
            AttributePrototype = new TsPropertyAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsPropertyAttribute AttributePrototype { get; set; }
        private List<TsDecoratorAttribute> Decorators { get; set; }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
        }

        TsPropertyAttribute IExportConfiguration<TsPropertyAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }

        double IOrderableMember.MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}