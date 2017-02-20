using Reinforced.Typings.Ast;

namespace Reinforced.Typings
{
    public class ExportedFile
    {
        public InspectedReferences GlobalReferences { get; internal set; }
        public InspectedReferences References { get; internal set; }
        public RtNamespace[] Namespaces { get; internal set; }
    }
}