using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings
{
    /// <summary>
    /// Resulting TS file model
    /// </summary>
    public class ExportedFile
    {
        private readonly ExportContext _context;

        internal ExportContext Context
        {
            get { return _context; }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal ExportedFile(HashSet<Type> typesToExport, string fileName, InspectedReferences references, ExportContext context)
        {
            _context = context;
            TypesToExport = typesToExport;
            FileName = fileName;
            AllTypesIsSingleFile = !_context.Hierarchical;
            References = references;
            TypeResolver = new TypeResolver(this);
            AddReferencesFromTypes();
        }

        /// <summary>
        /// File references and imports
        /// </summary>
        public InspectedReferences References { get; private set; }

        /// <summary>
        /// Namespaces ASTs
        /// </summary>
        public RtNamespace[] Namespaces { get; internal set; }

        /// <summary>
        /// Type Resolver object
        /// </summary>
        public TypeResolver TypeResolver { get; private set; }

        /// <summary>
        /// Gets or sets whether all exported types are stored in single file
        /// </summary>
        public bool AllTypesIsSingleFile { get; private set; }

        /// <summary>
        /// Set of types being exported within this file
        /// </summary>
        public HashSet<Type> TypesToExport { get; private set; }

        /// <summary>
        /// Absolute file path+name+extension
        /// </summary>
        public string FileName { get; private set; }

        private IEnumerable<RtReference> _refinedReferences;
        private IEnumerable<RtImport> _refinedImports;

        /// <summary>
        /// Gets final version of references (after conditional user processing)
        /// </summary>
        public IEnumerable<RtReference> FinalReferences
        {
            get { return _refinedReferences ?? References.References; }
        }

        /// <summary>
        /// Gets final version of references (after conditional user processing)
        /// </summary>
        public IEnumerable<RtImport> FinalImports
        {
            get { return _refinedImports ?? References.Imports; }
        }

        internal void ApplyReferenceProcessor(ReferenceProcessorBase refProcessor = null)
        {
            if (refProcessor == null) return;

            var references = References.References;
            references = refProcessor.FilterReferences(references, this);
            if (references == null) references = new RtReference[0];
            _refinedReferences = references;

            var imports = References.Imports;
            imports = refProcessor.FilterImports(imports, this);
            if (imports == null) imports = new RtImport[0];

            _refinedImports = imports;
        }

        /// <summary>
        /// Ensures that imports for specified type presents in specified file
        /// </summary>
        /// <param name="t">Type to import</param>
        /// <param name="typeName">Type name (probably overriden)</param>
        /// <returns>Import AST node or null if no import needed. Returns existing import in case if type is already imported</returns>
        internal RtImport EnsureImport(Type t, string typeName)
        {
            if (TypesToExport.Contains(t)) return null;

            var bp = _context.Project.Blueprint(t);
            if (bp.ThirdParty != null)
            {
                foreach (var tpi in bp.ThirdPartyImports)
                {
                    References.AddImport(tpi);
                }

                return null;
            }

            if (AllTypesIsSingleFile) return null;

            var relPath = GetRelativePathForType(t, FileName);
            if (string.IsNullOrEmpty(relPath)) return null;

            RtImport result = null;

            if (References.StarImports.ContainsKey(relPath))
            {
                return References.StarImports[relPath];
            }

            if (_context.Global.DiscardNamespacesWhenUsingModules)
            {
                var target = string.Format("{{ {0} }}", typeName);
                result = new RtImport() { From = relPath, Target = target };
                References.AddImport(result);
            }
            else
            {
                var alias = Path.GetFileNameWithoutExtension(relPath);
                var target = string.Format("* as {0}", alias);
                result = new RtImport() { From = relPath, Target = target };
                References.AddImport(result);
            }
            return result;
        }

        /// <summary>
        /// Ensures that reference for specified type presents in specified file
        /// </summary>
        /// <param name="t">Type to reference</param>
        /// <returns>Reference AST node or null if no reference needed. Returns existing reference in case if type is already referenced</returns>
        internal void EnsureReference(Type t)
        {
            if (TypesToExport.Contains(t)) return;
            var bp = _context.Project.Blueprint(t);
            if (bp.ThirdParty != null)
            {
                foreach (var tpi in bp.ThirdPartyReferences)
                {
                    References.AddReference(tpi);
                }
                return;
            }
            if (AllTypesIsSingleFile) return;
            var relPath = GetRelativePathForType(t, FileName);
            var result = new RtReference() { Path = relPath };
            References.AddReference(result);
        }

        private string GetRelativePathForType(Type typeToReference, string currentFile)
        {
            if (_context.Global.UseModules)
            {
                currentFile = Path.Combine(Path.GetDirectoryName(currentFile), Path.GetFileNameWithoutExtension(currentFile));
            }
            var desiredFile = _context.GetPathForType(typeToReference);
            if (currentFile == desiredFile) return String.Empty;

            var desiredFileName = Path.GetFileName(desiredFile);

            var relPath = GetRelativeNamespacePath(Path.GetDirectoryName(currentFile),
                Path.GetDirectoryName(desiredFile));

            relPath = Path.Combine(relPath, desiredFileName);
            if (_context.Global.UseModules)
            {
                if (!relPath.StartsWith(".")) relPath = "./" + relPath;
            }
            return relPath;
        }

        private string GetRelativeNamespacePath(string currentNamespace, string desiredNamespace)
        {
            if (currentNamespace == desiredNamespace) return string.Empty;
            if (string.IsNullOrEmpty(currentNamespace)) return desiredNamespace;

            var current = currentNamespace.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var desired = desiredNamespace.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();
            if (string.IsNullOrEmpty(desiredNamespace))
            {
                for (var i = 0; i < current.Length; i++) result.Append("../");
            }
            else
            {
                var level = current.Length;
                //var cr1 = current.I(level);
                //var ds1 = desired.I(level);
                while (level >= 0 && (!ArrayExtensions.PartialCompare(current, desired, level)))
                {
                    //var cr = current.I(level);
                    //var ds = desired.I(level);
                    result.Append("../");
                    level--;

                }
                //level++;
                for (; level < desired.Length; level++)
                {
                    result.AppendFormat("{0}/", desired[level]);
                }
            }
            return result.ToString().Trim(Path.DirectorySeparatorChar);
        }

        private void AddReferencesFromTypes()
        {
            foreach (var type in TypesToExport)
            {
                AddTypeSpecificReferences(type);
                if (_context.Global.UseModules) AddTypeSpecificImports(type);
            }
        }

        private void AddTypeSpecificReferences(Type t)
        {
            var references = _context.Project.Blueprint(t).References;

            foreach (var tsAddTypeReferenceAttribute in references)
            {
                if (tsAddTypeReferenceAttribute.Type != null)
                {
                    TypeResolver.ResolveTypeName(tsAddTypeReferenceAttribute.Type);
                }
                else
                {
                    References.AddReference(tsAddTypeReferenceAttribute.ToReference());
                }
            }

        }

        private void AddTypeSpecificImports(Type t)
        {
            var imports = _context.Project.Blueprint(t).Imports;
            foreach (var tsAddTypeImportAttribute in imports)
            {
                References.AddImport(tsAddTypeImportAttribute.ToImport());
            }
        }
    }
}