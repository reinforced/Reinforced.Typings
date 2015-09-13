using System;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for enums
    /// </summary>
    public class EnumGenerator : ITsCodeGenerator<Type>
    {
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var values = Enum.GetValues(element);
            var name = element.GetName();
            var fmt = Settings.GetDeclarationFormat(element);

            sw.WriteLine(String.Format(fmt,"enum {0} {{ "), name);
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

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }
    }
}
