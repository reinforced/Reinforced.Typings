using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    class GenericInterfaceConfigurationBuilder : GenericConfigurationBuilderBase, IInterfaceConfigurationBuilder
    {
        public GenericInterfaceConfigurationBuilder(Type t) : base(t)
        {
            AttributePrototype = new TsInterfaceAttribute
            {
                AutoExportProperties = false,
                AutoExportMethods = false
            };
        }

        public TsInterfaceAttribute AttributePrototype { get; private set; }

        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}