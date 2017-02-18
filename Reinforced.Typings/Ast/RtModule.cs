using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript module
    /// </summary>
    public class RtModule : RtNode
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtModule()
        {
            CompilationUnits = new List<RtNode>();
        }

        /// <summary>
        /// Identifies nameless module that only wraps CompilationUnits without module name
        /// </summary>
        public bool IsAmbientModule { get; set; }

        /// <summary>
        /// Module name
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Members of module - compilation units. Classes/enums/interfaces
        /// </summary>
        public List<RtNode> CompilationUnits { get; set; }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get { return CompilationUnits; }
        }

        /// <summary>
        /// Visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Typed visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}
