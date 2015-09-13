using System;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for interfaces. Derived from class generator since interfaces are very similar to classes in TypeScript
    /// </summary>
    public class InterfaceCodeGenerator : ClassCodeGenerator
    {
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public override void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var tc = element.GetCustomAttribute<TsInterfaceAttribute>();
            if (tc == null) throw new ArgumentException("TsInterfaceAttribute is not present", "element");
            Export("interface", element, resolver, sw, tc);
        }
    }
}
