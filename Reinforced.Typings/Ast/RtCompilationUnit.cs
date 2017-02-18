using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Abstract AST node for class/interface/enum
    /// </summary>
    public abstract class RtCompilationUnit : RtNode
    {
        protected RtCompilationUnit()
        {
            Decorators = new List<RtDecorator>();
        }

        /// <summary>
        /// Denotes current class to be exported
        /// </summary>
        public bool Export { get; set; }

        /// <summary>
        /// Denotes that current class must be default export of module
        /// </summary>
        public bool DefaultExport { get; set; }

        /// <summary>
        /// Decorators for type
        /// </summary>
        public List<RtDecorator> Decorators { get; set; }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                if(Decorators==null) yield break;
                foreach (var rtDecorator in Decorators)
                {
                    yield return rtDecorator;
                }
            }
        }
    }
}
