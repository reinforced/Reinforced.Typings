using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for namespace
    /// </summary>
    public class NamespaceCodeGenerator
    {
        /// <summary>
        ///     Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }

        /// <summary>
        ///     Generates namespace source code
        /// </summary>
        /// <param name="types">Types list</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual RtModule Generate(IEnumerable<Type> types, string namespaceName, TypeResolver resolver)
        {
            RtModule module = new RtModule();
            if (string.IsNullOrEmpty(namespaceName)) module.IsAbstractModule = true;
            Settings.CurrentNamespace = namespaceName;
            foreach (var type in types)
            {
                var converter = resolver.GeneratorFor(type, Settings);
                converter.Generate(type, resolver);
                Console.WriteLine("Exported {0}", type);
            }

            Settings.CurrentNamespace = null;
            return module;
        }
    }
}