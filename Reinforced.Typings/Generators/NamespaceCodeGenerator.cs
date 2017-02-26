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
                var converter = resolver.GeneratorFor(type, Context);
                var member = converter.Generate(type, resolver);
                if (Context.Global.UseModules)
                {
                    var m = member as RtCompilationUnit;
                    if (m != null)
                    {
                        m.Export = true;
                    }
                }
                ns.CompilationUnits.Add(member);
                Console.WriteLine("Exported {0}", type);
            }

            Context.CurrentNamespace = null;
            Context.Location.ResetLocation(ns);
            return ns;
        }
    }
}