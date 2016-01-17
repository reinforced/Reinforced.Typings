using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Generators
{
    public abstract class TsCodeGeneratorBase<T,TNode> : ITsCodeGenerator<T>
        where TNode : RtNode
    {
        /// <summary>
        ///  Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }

        public RtNode Generate(T element, TypeResolver resolver)
        {
            return GenerateNode(element, resolver);
        }

        public abstract TNode GenerateNode(T element, TypeResolver resolver);
    }
}
