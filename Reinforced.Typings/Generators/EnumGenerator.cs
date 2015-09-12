using System;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for enums
    /// </summary>
    public class EnumGenerator : ITsCodeGenerator<Type>
    {
        public void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var values = Enum.GetValues(element);
            var name = element.GetName();
            sw.WriteLine("export enum {0} {{ ", name);
            sw.Tab();
            for (int index = 0; index < values.Length; index++)
            {
                var v = values.GetValue(index);
                var n = Enum.GetName(element, v);
                sw.Indent();
                sw.Write("{0} = {1}", n, Convert.ToInt64(v));
                if (index < values.Length - 1) sw.Write(",");
                sw.WriteLine();
            }
            sw.UnTab();
            sw.WriteLine("}");
        }
    }
}
