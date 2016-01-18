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
        public ExportContext Context { get; set; }

        /// <summary>
        ///     Generates namespace source code
        /// </summary>
        /// <param name="types">Types list</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="resolver">Type resolver</param>
        public virtual RtModule Generate(IEnumerable<Type> types, string namespaceName, TypeResolver resolver)
        {
            RtModule module = new RtModule();
            if (string.IsNullOrEmpty(namespaceName)) module.IsAbstractModule = true;
            module.ModuleName = namespaceName;

            Context.CurrentNamespace = namespaceName;
            Context.Location.SetLocation(module);
            foreach (var type in types)
            {
                var converter = resolver.GeneratorFor(type, Context);
                var member = converter.Generate(type, resolver);
                module.CompilationUnits.Add(member);
                Console.WriteLine("Exported {0}", type);
            }

            Context.CurrentNamespace = null;
            Context.Location.ResetLocation(module);
            return module;
        }
    }
}