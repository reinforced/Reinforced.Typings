using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for CLR type (class)
    /// </summary>
    public class ClassCodeGenerator :ClassAndInterfaceGeneratorBase, ITsCodeGenerator<Type, RtClass>
    {

        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        public virtual RtClass Generate(Type element, TypeResolver resolver)
        {
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(element);
            if (tc == null) throw new ArgumentException("TsClassAttribute is not present", "element");
            var result = new RtClass();
            Export(result, element, resolver, tc);
            return result;
        }
    }
}