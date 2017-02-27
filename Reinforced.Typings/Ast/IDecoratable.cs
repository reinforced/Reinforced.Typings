using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
