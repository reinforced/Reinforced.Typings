using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Configuration interface for members supporting decorators aggregation
    /// </summary>
    public interface IDecoratorsAggregator
    {
        /// <summary>
        /// Aggregated decorators
        /// </summary>
        List<TsDecoratorAttribute> Decorators { get; }
    }
}