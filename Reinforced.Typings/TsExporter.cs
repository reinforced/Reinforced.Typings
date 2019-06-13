using System;
using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Facade for final TypeScript export. This class supplies assemblies names or assemblies itself as parameter and
    ///     exports resulting TypeScript file to file or to string
    /// </summary>
    public sealed class TsExporter //: MarshalByRefObject
    {
        private readonly ExportContext _context;

        /// <summary>
        /// Obtains export context
        /// </summary>
        public ExportContext Context
        {
            get { return _context; }
        }

        private bool _isInitialized;
        
       
        #region Constructors

        /// <summary>
        ///     Constructs new instance of TypeScript exporter
        /// </summary>
        /// <param name="context"></param>
        public TsExporter(ExportContext context)
        {
            _context = context;
        }

        #endregion

        /// <summary>
        /// Initializes TS exporter. Reads all types configuration, applies fluent configuration, resolves references
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            _context.Initialize();
            
            _isInitialized = true;
        }

        

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="fileName">File name to export files to</param>
        private ExportedFile ExportTypes(string fileName = null)
        {
            var ef = _context.CreateExportedFile(fileName);
            var gen = _context.Generators.GeneratorForNamespace();
            var grp = ef.TypesToExport.GroupBy(c => _context.Project.Blueprint(c).GetNamespace(true));
            var nsp = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            List<RtNamespace> result = new List<RtNamespace>(nsp.Count);
            foreach (var n in nsp)
            {
                var ns = n.Key;
                if (ns == "-") ns = string.Empty;
                var module = gen.Generate(n.Value, ns, ef.TypeResolver);
                result.Add(module);
            }
            ef.Namespaces = result.ToArray();
            return ef;
        }

        /// <summary>
        ///     Exports TypeScript source according to settings
        /// </summary>
        public void Export()
        {
            _context.FileOperations.ClearTempRegistry();
            Initialize();

            _context.Lock();
            ReferenceProcessorBase refProc = null;
            if (_context.Global.ReferencesProcessorType != null)
            {
                refProc = (ReferenceProcessorBase)Activator.CreateInstance(_context.Global.ReferencesProcessorType);
            }
            if (!_context.Hierarchical)
            {
                var file = ExportTypes();
                file.ApplyReferenceProcessor(refProc);
                _context.FileOperations.Export(_context.TargetFile, file);
            }
            else
            {
                foreach (var fileName in _context.TypesToFilesMap.Keys.Union(_context.CustomGeneratorsToFilesMap.Keys).Distinct())
                {
                    var file = ExportTypes(fileName);
                    file.ApplyReferenceProcessor(refProc);
                    _context.FileOperations.Export(fileName, file);
                }
            }

            _context.Unlock();
            _context.FileOperations.DeployTempFiles();
        }


    }
}