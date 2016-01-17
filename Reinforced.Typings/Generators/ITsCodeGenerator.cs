using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     TypeScript code generator interface
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public interface ITsCodeGenerator<in TElement,out TNode> where TNode : RtNode
    {
        /// <summary>
        ///     Export settings
        /// </summary>
        ExportSettings Settings { get; set; }

        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        TNode Generate(TElement element, TypeResolver resolver);
    }
}