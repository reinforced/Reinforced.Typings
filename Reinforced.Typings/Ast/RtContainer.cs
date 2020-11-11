using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for node collections
    /// </summary>
    public class RtContainer:RtNode
    {
        private readonly HashSet<RtNode> _children = new HashSet<RtNode>();

        public override IEnumerable<RtNode> Children => _children;

        public void Add(RtNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            _children.Add(node);
        }

        public void AddRange(IEnumerable<RtNode> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            foreach (RtNode node in nodes)
            {
                _children.Add(node);
            }
        }

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}