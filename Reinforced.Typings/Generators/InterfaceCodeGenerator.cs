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
        public override void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var tc = element.GetCustomAttribute<TsInterfaceAttribute>();
            if (tc == null) throw new ArgumentException("TsInterfaceAttribute is not present", "element");
            Export("interface", element, resolver, sw, tc);
        }
    }
}
