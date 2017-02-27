using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript enumeration
    /// </summary>
    public class RtEnum : RtCompilationUnit, IDecoratable
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Enum name
        /// </summary>
        public RtSimpleTypeName EnumName { get; set; }

        /// <summary>
        /// Enum values
        /// </summary>
        public List<RtEnumValue> Values { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtEnum()
        {
            Values = new List<RtEnumValue>();
            Decorators = new List<RtDecorator>();
        }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtNode in Decorators)
                {
                    yield return rtNode;
                }
                if (Values != null)
                {
                    foreach (var rtEnumValue in Values)
                    {
                        yield return rtEnumValue;
                    }
                }
            }
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

        
        public List<RtDecorator> Decorators { get; private set; }
    }
}
