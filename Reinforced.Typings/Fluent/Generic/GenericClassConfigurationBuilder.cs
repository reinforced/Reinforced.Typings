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
        }


        public TsClassAttribute AttributePrototype { get; private set; }
    }
}
