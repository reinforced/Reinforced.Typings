using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for CLR type (class)
    /// </summary>
    public class ClassCodeGenerator : ClassAndInterfaceGeneratorBase<RtClass>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtClass GenerateNode(Type element, RtClass result, TypeResolver resolver)
        {

            var clsbp = Context.Project.Blueprint(element);
            var tc = Context.Project.Blueprint(element).Attr<TsClassAttribute>();
            if (tc == null) throw new ArgumentException("TsClassAttribute is not present", "element");
            Export(result, element, resolver, tc);
            AddDecorators(result, clsbp.GetDecorators());
            if (!tc.IsAbstract.HasValue)
            {
                result.Abstract = element._IsAbstract() && !element._IsInterface();
            }
            else
            {
                result.Abstract = tc.IsAbstract.Value;
            }
            return result;
        }
    }
}