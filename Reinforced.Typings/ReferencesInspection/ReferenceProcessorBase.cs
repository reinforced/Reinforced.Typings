using System.Collections.Generic;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.ReferencesInspection
{
    /// <summary>
    /// Base class for reference post-processor.
    /// </summary>
    public abstract class ReferenceProcessorBase
    {
        /// <summary>
        /// Returns refiltered and reordered import directives that must appear in resulting file
        /// </summary>
        /// <param name="imports">Set on initially computed imports</param>
        /// <param name="file">File that is being exported currently</param>
        /// <returns>Set of refiltered/reordered imports</returns>
        public abstract IEnumerable<RtImport> FilterImports(IEnumerable<RtImport> imports, ExportedFile file);

        /// <summary>
        /// Returns refiltered and reordered reference directives that must appear in resulting file
        /// </summary>
        /// <param name="references">Set on initially computed references</param>
        /// <param name="file">File that is being exported currently</param>
        /// <returns>Set of refiltered/reordered references</returns>
        public abstract IEnumerable<RtReference> FilterReferences(IEnumerable<RtReference> references, ExportedFile file);
    }
}
