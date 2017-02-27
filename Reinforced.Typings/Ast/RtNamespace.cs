using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript module
    /// </summary>
    public class RtNamespace : RtNode
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtNamespace()
        {
            CompilationUnits = new List<RtNode>();
            Export = true;
        }

        /// <summary>
        /// Identifies nameless namespace that only wraps CompilationUnits without module name
        /// </summary>
        public bool IsAmbientNamespace { get; set; }

        /// <summary>
        /// Module name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Denotes whether namespace must be exported or not
        /// </summary>
        public bool Export { get; set; }

        /// <summary>
        /// Denotes namespace generation mode
        /// </summary>
        public NamespaceGenerationMode GenerationMode { get; internal set; }

        /// <summary>
        /// Members of module - compilation units. Classes/enums/interfaces
        /// </summary>
        public List<RtNode> CompilationUnits { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { return CompilationUnits; }
        }

        /// <inheritdoc />
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Switches mode for generating namespace.
    /// If UseModules set to true then namespace must be 
    /// exported as Namespaces.
    /// If modules are not used then namespaces must represent modules
    /// </summary>
    public enum NamespaceGenerationMode
    {
        /// <summary>
        /// Export namespace as module
        /// </summary>
        Module,

        /// <summary>
        /// Export namespace as namespace
        /// </summary>
        Namespace
    }
}
