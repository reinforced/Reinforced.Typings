using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings
{
    /// <summary>
    /// Represents inspected references for type or global references
    /// </summary>
    public class InspectedReferences
    {
        private readonly List<RtReference> _references = new List<RtReference>();
        private readonly List<RtImport> _imports = new List<RtImport>();

        /// <summary>
        /// References exposed via &lt;reference path="..."&gt; tag
        /// </summary>
        public IReadOnlyCollection<RtReference> References { get { return _references; } }

        /// <summary>
        /// References exposed via imports
        /// </summary>
        public IReadOnlyCollection<RtImport> Imports { get { return _imports; } }

        internal InspectedReferences(IEnumerable<RtReference> references = null, IEnumerable<RtImport> imports = null)
        {
            if (references != null) _references.AddRange(references);
            if (imports != null) _imports.AddRange(imports);
        }

        internal void Merge(InspectedReferences refs)
        {
            _references.AddRange(refs.References);
            _imports.AddRange(refs.Imports);
        }
    }
}