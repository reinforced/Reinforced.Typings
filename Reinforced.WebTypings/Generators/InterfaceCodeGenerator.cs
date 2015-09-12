using System;
using System.Reflection;

namespace Reinforced.WebTypings.Generators
{
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
