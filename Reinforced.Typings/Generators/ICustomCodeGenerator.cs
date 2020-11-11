using System.Collections.Generic;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Generators
{
    public interface ICustomCodeGenerator
    {
        IEnumerable<RtNode> Generate();
    }
}