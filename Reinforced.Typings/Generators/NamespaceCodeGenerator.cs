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
        public virtual RtNamespace Generate(IEnumerable<Type> types, string namespaceName, TypeResolver resolver)
        {
            RtNamespace ns = new RtNamespace();
            var needToDiscard = Context.Global.UseModules && Context.Global.DiscardNamespacesWhenUsingModules;

            if (string.IsNullOrEmpty(namespaceName) || needToDiscard) ns.IsAmbientNamespace = true;
            ns.Name = namespaceName;

            Context.CurrentNamespace = namespaceName;
            Context.Location.SetLocation(ns);
            foreach (var type in types)
            {
                var typeGenerator = Context.Generators.GeneratorFor(type, Context);
                if (typeGenerator == null) continue;
                var member = typeGenerator.Generate(type, resolver);
                var m = member as RtCompilationUnit;
                if (m != null)
                {
                    if (Context.Global.UseModules)
                    {
                        m.Export = true;
                    }
                    else
                    {
                        m.Export = !ns.IsAmbientNamespace;
                    }
                }


                ns.CompilationUnits.Add(member);
#if DEBUG
                Console.WriteLine("Exported {0}", type);
#endif
            }

            if (Context.Global.UseModules) ns.GenerationMode = NamespaceGenerationMode.Namespace;

            Context.CurrentNamespace = null;
            Context.Location.ResetLocation(ns);
            return ns;
        }
    }
}