using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.ReferencesInspection
{
    /// <summary>
    /// Represents inspected references for type or global references
    /// </summary>
    public class InspectedReferences
    {
        private readonly HashSet<RtReference> _references = new HashSet<RtReference>(ReferenceComparer.Instance);
        private readonly HashSet<RtImport> _imports = new HashSet<RtImport>(ImportComparer.Instance);
        private readonly Dictionary<string, string> _starImportsAs = new Dictionary<string, string>();

        /// <summary>
        /// References exposed via &lt;reference path="..."&gt; tag
        /// </summary>
        public IEnumerable<RtReference> References { get { return _references; } }

        /// <summary>
        /// References exposed via imports
        /// </summary>
        public IEnumerable<RtImport> Imports { get { return _imports; } }

        /// <summary>
        /// Cache of starred imports. Key is "from", value is star import alias
        /// </summary>
        public IReadOnlyDictionary<string, string> StarImports { get { return _starImportsAs; } }

        public InspectedReferences(IEnumerable<RtReference> references, IEnumerable<RtImport> imports = null)
        {
            foreach (var rtReference in references)
            {
                _references.AddIfNotExists(rtReference);
            }

            if (imports != null)
            {
                foreach (var rtImport in imports.Where(c => c.IsWildcard))
                {
                    _imports.AddIfNotExists(rtImport);
                    _starImportsAs[rtImport.From] = rtImport.WildcardAlias;
                }

                foreach (var rtImport in imports.Where(c => !c.IsWildcard))
                {
                    _imports.AddIfNotExists(rtImport);
                }
            }
        }

        /// <summary>
        /// Duplicates inspected references for further usage
        /// </summary>
        /// <returns></returns>
        public InspectedReferences Duplicate()
        {
            return new InspectedReferences(_references, _imports);
        }

        /// <summary>
        /// Attaches new reference to existing ones
        /// </summary>
        /// <param name="reference">New reference</param>
        public void AddReference(RtReference reference)
        {
            _references.AddIfNotExists(reference);
        }

        /// <summary>
        /// Attaches new import to existing ones
        /// </summary>
        /// <param name="import">Import</param>
        public void AddImport(RtImport import)
        {
            if (import.IsWildcard)
            {
                if (_starImportsAs.ContainsKey(import.From)) return;
                _imports.RemoveWhere(c => c.From == import.From);
                _imports.Add(import);
                _starImportsAs[import.From] = import.WildcardAlias;
            }
            else
            {
                _imports.AddIfNotExists(import);
            }
        }
    }


}