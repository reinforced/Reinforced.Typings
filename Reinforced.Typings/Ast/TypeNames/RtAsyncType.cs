using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for async return types of type "Promise".
    /// </summary>
    /// <remarks>With TypeScript, "Promise" use "generics" to define the resulting type of the "Promise". This is
    /// defined by a nested <see cref="TypeNameOfAsync"/></remarks>
    public sealed class RtAsyncType : RtTypeName
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtAsyncType(RtTypeName nestedType)
            : this()
        {
            TypeNameOfAsync = nestedType;
        }

        /// <summary>
        /// Type name
        /// </summary>
        public RtTypeName TypeNameOfAsync { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtAsyncType()
        {
        }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
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

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Promise<{TypeNameOfAsync?.ToString() ?? "void"}>";
        }
    }
}
