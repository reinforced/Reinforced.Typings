using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings
{
    public class InspectedReferences
    {
        private readonly List<RtReference> _references = new List<RtReference>();
        private readonly List<RtImport> _imports = new List<RtImport>();

        public IReadOnlyCollection<RtReference> References { get { return _references; } }
        public IReadOnlyCollection<RtImport> Imports { get { return _imports; } }

        public InspectedReferences(IEnumerable<RtReference> references = null, IEnumerable<RtImport> imports = null)
        {
            if (references != null) _references.AddRange(references);
            if (imports != null) _imports.AddRange(imports);
        }

        public void Merge(InspectedReferences refs)
        {
            _references.AddRange(refs.References);
            _imports.AddRange(refs.Imports);
        }
    }
}