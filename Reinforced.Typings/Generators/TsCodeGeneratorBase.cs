using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Base for strongly-typed code generator with automatical Context.Location handling
    /// </summary>
    /// <typeparam name="T">Source reflection [Something]Info type. Possible: Type, MethodInfo, PropertyInfo, ConstructorInfo, FieldInfo</typeparam>
    /// <typeparam name="TNode">Resulting node type</typeparam>
    public abstract class TsCodeGeneratorBase<T, TNode> : ITsCodeGenerator<T>
        where TNode : RtNode, new()
    {
        /// <summary>
        ///  Export settings
        /// </summary>
        public ExportContext Context { get; set; }

        /// <summary>
        /// Generate method implementation. 
        /// Calls GenerateNode inside, creates node dummy, sets and resets location
        /// </summary>
        /// <param name="element">Reflection element instance</param>
        /// <param name="resolver">Type resolver</param>
        /// <returns>Generated node or null</returns>
        public RtNode Generate(T element, TypeResolver resolver)
        {
            try
            {
                TNode currentNode = new TNode();
                Context.Location.SetLocation(currentNode);
                var result = GenerateNode(element, currentNode, resolver);
                Context.Location.ResetLocation(currentNode);
                return result;
            }
            catch (Exception ex)
            {
                ErrorMessages.RTE0004_GeneratorError.Throw(GetType().FullName, ex.Message);
                return null; // unreacheable
            }
            
        }

        /// <summary>
        /// Main entry point for resulting node generation.  
        /// </summary>
        /// <param name="element">Reflection element</param>
        /// <param name="node">Resulting node to be modified</param>
        /// <param name="resolver">Type resolver</param>
        /// <returns>Resulting node or null</returns>
        public abstract TNode GenerateNode(T element, TNode node, TypeResolver resolver);


        /// <summary>
        /// Appends decorators to decoratable node
        /// </summary>
        /// <param name="node">Decoratable syntax node</param>
        /// <param name="decorators">Set of decorator attributes</param>
        protected void AddDecorators(IDecoratable node, IEnumerable<TsDecoratorAttribute> decorators)
        {
            var decs = decorators.Select(c => new RtDecorator(c.Decorator, c.Order));
            node.Decorators.AddRange(decs);
        }
    }
}
