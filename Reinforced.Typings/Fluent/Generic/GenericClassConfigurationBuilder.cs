using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public TsClassAttribute AttributePrototype { get; private set; }
        public List<TsDecoratorAttribute> Decorators { get; }

        public override double MemberOrder { get { return AttributePrototype.Order; } set { AttributePrototype.Order = value; } }
    }
}
