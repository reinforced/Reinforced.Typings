using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for namespace
    /// </summary>
    public class NamespaceCodeGenerator
    {
        /// <summary>
        /// Generates namespace source code
        /// </summary>
        /// <param name="types">Types list</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(IEnumerable<Type> types, string namespaceName, TypeResolver resolver, WriterWrapper sw)
        {
            WriteNamespaceBegin(namespaceName,sw);
            Settings.CurrentNamespace = namespaceName;
            foreach (var type in types)
            {
                var converter = resolver.GeneratorFor(type,Settings);
                converter.Generate(type, resolver, sw);
                Console.WriteLine("Exported {0}", type);
            }
           
            WriteNamespaceEnd(namespaceName,sw);
        }

        /// <summary>
        /// Writes to output file opening namespace declaration
        /// </summary>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="sw">Output writer</param>
        public virtual void WriteNamespaceBegin(string namespaceName,WriterWrapper sw)
        {
            if (string.IsNullOrEmpty(namespaceName)) return;
            if (Settings.ExportPureTypings)
            {
                sw.WriteLine("declare module {0} {{", namespaceName);
            }
            else
            {
                sw.WriteLine("module {0} {{", namespaceName);
            }
            sw.Tab();
        }

        /// <summary>
        /// Writes to ouput file namespace closing
        /// </summary>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="sw">Output writer</param>
        public virtual void WriteNamespaceEnd(string namespaceName, WriterWrapper sw)
        {
            if (string.IsNullOrEmpty(namespaceName)) return;
            sw.UnTab();
            sw.Indent();
            sw.WriteLine("}");
            sw.Br();
        }

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }
    }
}
