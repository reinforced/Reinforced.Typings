using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Node containing decorators
    /// </summary>
    public interface IDecoratable
    {
        /// <summary>
        /// Set of decorators applied to node
        /// </summary>
        List<RtDecorator> Decorators { get; }
    }
}
