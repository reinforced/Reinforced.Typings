using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Abstraction for RtClass and RtInterface AST
    /// </summary>
    public interface ITypeMember
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Class/interface members
        /// </summary>
        List<RtNode> Members { get; }

        /// <summary>
        /// class/interface name
        /// </summary>
        RtSimpleTypeName Name { get; set; }

        /// <summary>
        /// Implementing types names
        /// </summary>
        List<RtSimpleTypeName> Implementees { get; }
    }
}
