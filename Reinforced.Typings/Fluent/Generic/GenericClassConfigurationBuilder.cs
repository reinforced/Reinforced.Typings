using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    class GenericClassConfigurationBuilder : GenericConfigurationBuilderBase, IClassConfigurationBuilder
    {
        public GenericClassConfigurationBuilder(Type t) : base(t)
        {
            AttributePrototype = new TsClassAttribute
            {
                AutoExportConstructors = false,
                AutoExportFields = false,
                AutoExportProperties = false,
                AutoExportMethods = false
            };
            Decorators = new List<TsDecoratorAttribute>();
        }


        private TsClassAttribute AttributePrototype { get; set; }

        private List<TsDecoratorAttribute> Decorators { get; set; }

        TsClassAttribute IAttributed<TsClassAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
        }

        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}