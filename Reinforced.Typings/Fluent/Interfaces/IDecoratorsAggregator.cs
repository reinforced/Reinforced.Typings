using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IDecoratorsAggregator
    {
        List<TsDecoratorAttribute> Decorators { get; }
    }
}
