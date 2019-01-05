using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Abstraction for RtClass and RtInterface AST
    /// </summary>
    public interface ITypeMember
    {
        /// <summary>
        /// Denotes current class to be exported
        /// </summary>
        bool Export { get; set; }

        /// <summary>
        /// Denotes that current class must be default export of module
        /// </summary>
        bool DefaultExport { get; set; }

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

        /// <summary>
        /// Order of writing
        /// </summary>
        double Order { get; set; }
    }
}
