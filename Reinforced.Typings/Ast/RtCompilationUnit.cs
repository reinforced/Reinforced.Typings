using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Abstract AST node for class/interface/enum
    /// </summary>
    public abstract class RtCompilationUnit : RtNode
    {
        /// <summary>
        /// Denotes current class to be exported
        /// </summary>
        public bool Export { get; set; }

        /// <summary>
        /// Denotes that current class must be default export of module
        /// </summary>
        public bool DefaultExport { get; set; }

        /// <summary>
        /// Order of writing
        /// </summary>
        public double Order { get; set; }
    }
}
