using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Import declaration
    /// </summary>
    public class RtImport : RtNode
    {
        /// <summary>
        /// Type list
        /// </summary>
        public string[] Types { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }

        public override IEnumerable<RtNode> Children { get {yield break;} }

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
