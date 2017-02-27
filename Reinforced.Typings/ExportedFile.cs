using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings
{
    /// <summary>
    /// Resulting TS file model
    /// </summary>
    public class ExportedFile
    {
        /// <summary>
        /// File references and imports
        /// </summary>
        public InspectedReferences References { get; internal set; }

        /// <summary>
        /// Namespaces ASTs
        /// </summary>
        public RtNamespace[] Namespaces { get; internal set; }

        /// <summary>
        /// Type Resolver object
        /// </summary>
        public TypeResolver TypeResolver { get; internal set; }

        /// <summary>
        /// Gets or sets whether all exported types are stored in single file
        /// </summary>
        public bool AllTypesIsSingleFile { get; internal set; }

        /// <summary>
        /// Set of types being exported within this file
        /// </summary>
        public HashSet<Type> TypesToExport { get; internal set; }

        /// <summary>
        /// Absolute file path+name+extension
        /// </summary>
        public string FileName { get; internal set; }
    }
}