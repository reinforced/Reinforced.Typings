using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
