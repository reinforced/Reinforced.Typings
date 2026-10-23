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
        private readonly List<RtReference> _referencesOrder = new List<RtReference>();
        private readonly List<RtImport> _importsOrder = new List<RtImport>();
        private readonly Dictionary<string, RtImport> _starImportsAs = new Dictionary<string, RtImport>();

        /// <summary>
        /// References exposed via &lt;reference path="..."&gt; tag
        /// </summary>
        public IEnumerable<RtReference> References { get { return _referencesOrder; } }

        /// <summary>
        /// References exposed via imports
        /// </summary>
        public IEnumerable<RtImport> Imports { get { return _importsOrder; } }

        /// <summary>
        /// Cache of starred imports. Key is "from", value is star import alias
        /// </summary>
        public IReadOnlyDictionary<string, RtImport> StarImports { get { return _starImportsAs; } }

        /// <summary>
        /// Constructs new inspected references set
        /// </summary>
        /// <param name="references">References</param>
        /// <param name="imports">Imports</param>
        public InspectedReferences(IEnumerable<RtReference> references, IEnumerable<RtImport> imports = null)
        {
            foreach (var rtReference in references)
            {
                AddReference(rtReference);
            }

            if (imports != null)
            {
                foreach (var rtImport in imports.Where(c => c.IsWildcard))
                {
                    AddImport(rtImport);
                }

                foreach (var rtImport in imports.Where(c => !c.IsWildcard))
                {
                    AddImport(rtImport);
                }
            }
        }

        /// <summary>
        /// Duplicates inspected references for further usage
        /// </summary>
        /// <returns></returns>
        public InspectedReferences Duplicate()
        {
            return new InspectedReferences(_referencesOrder, _importsOrder);
        }

        /// <summary>
        /// Attaches new reference to existing ones
        /// </summary>
        /// <param name="reference">New reference</param>
        public void AddReference(RtReference reference)
        {
            if (_references.Contains(reference)) return;
            _references.Add(reference);
            _referencesOrder.Add(reference);
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
                var insertIndex = _importsOrder.FindIndex(c => c.From == import.From);
                var removed = _imports.RemoveWhere(c => c.From == import.From);
                if (removed > 0) _importsOrder.RemoveAll(c => c.From == import.From);
                _imports.Add(import);
                if (insertIndex >= 0 && insertIndex <= _importsOrder.Count)
                {
                    _importsOrder.Insert(insertIndex, import);
                }
                else
                {
                    _importsOrder.Add(import);
                }
                _starImportsAs[import.From] = import;
            }
            else
            {
                if (_imports.Contains(import)) return;
                _imports.Add(import);
                _importsOrder.Add(import);
            }
        }
    }


}
