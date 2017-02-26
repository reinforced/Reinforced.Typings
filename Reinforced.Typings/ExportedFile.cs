using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings
{
    public class ExportedFile
    {
        public InspectedReferences References { get; internal set; }
        public RtNamespace[] Namespaces { get; internal set; }
        public TypeResolver TypeResolver { get; internal set; }
        public bool AllTypesIsSingleFile { get; internal set; }
        public HashSet<Type> TypesToExport { get; internal set; }
        public string FileName { get; internal set; }
    }
}