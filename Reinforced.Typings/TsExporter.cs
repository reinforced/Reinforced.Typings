using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.ReferencesInspection;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Facade for final TypeScript export. This class supplies assemblies names or assemblies itself as parameter and
    ///     exports resulting TypeScript file to file or to string
    /// </summary>
    public sealed class TsExporter //: MarshalByRefObject
    {
        private readonly ExportContext _context;
        private List<Type> _allTypes;
        private HashSet<Type> _allTypesHash;
        private bool _isInitialized;
        private Dictionary<string, IEnumerable<Type>> _typesToFilesMap;

        /// <summary>
        /// Global references extracted from current configuration
        /// </summary>
        public InspectedReferences GlobalReferences { get; private set; }

        /// <summary>
        /// Reference inspector instance
        /// </summary>
        public ReferenceInspector ReferenceInspector { get; private set; }

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

        private void ApplyTsGlobal(TsGlobalAttribute tsGlobal, GlobalParameters gp)
        {
            if (tsGlobal == null) return;
            gp.CamelCaseForMethods = tsGlobal.CamelCaseForMethods;
            gp.CamelCaseForProperties = tsGlobal.CamelCaseForProperties;
            gp.DiscardNamespacesWhenUsingModules = tsGlobal.DiscardNamespacesWhenUsingModules;
            gp.ExportPureTypings = tsGlobal.ExportPureTypings;
            gp.GenerateDocumentation = tsGlobal.GenerateDocumentation;
            gp.RootNamespace = tsGlobal.RootNamespace;
            gp.UseModules = tsGlobal.UseModules;
            gp.TabSymbol = tsGlobal.TabSymbol;
            gp.WriteWarningComment = tsGlobal.WriteWarningComment;
            //gp.StrictNullChecks = tsGlobal.StrictNullChecks;
        }

        /// <summary>
        /// Initializes TS exporter. Reads all types configuration, applies fluent configuration, resolves references
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            // 1st step - searching and processing [TsGlobal] attribute
            var tsGlobal = _context.SourceAssemblies.Select(c => c.GetCustomAttribute<TsGlobalAttribute>())
                .Where(c => c != null)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();
            ApplyTsGlobal(tsGlobal, _context.Global);

            // 2nd step - searching and processing fluent configuration
            var fluentConfigurationPresents = _context.ConfigurationMethod != null;
            if (fluentConfigurationPresents)
            {
                var configurationBuilder = new ConfigurationBuilder(_context);
                _context.ConfigurationMethod(configurationBuilder);
            }

            _context.Documentation =
                new DocumentationManager(_context.Global.GenerateDocumentation ? _context.DocumentationFilePath : null, _context.Warnings);
            foreach (var additionalDocumentationPath in _context.Project.AdditionalDocumentationPathes)
            {
                _context.Documentation.CacheDocumentation(additionalDocumentationPath, _context.Warnings);
            }

            _allTypes = _context.SourceAssemblies
                .SelectMany(c => c._GetTypes(_context.Warnings).Where(d => d.GetCustomAttribute<TsAttributeBase>(false) != null))
                .Union(_context.Project.BlueprintedTypes).Distinct()
                .ToList();

            _allTypesHash = new HashSet<Type>(_allTypes);
            ReferenceInspector = new ReferenceInspector(_context, _allTypesHash);

            if (_context.Hierarchical)
            {
                foreach (var type in _allTypesHash)
                {
                    _context.Project.AddFileSeparationSettings(type);
                }
            }

            GlobalReferences = ReferenceInspector.InspectGlobalReferences();
            _context.Generators = new GeneratorManager(_context);
            if (!_context.Hierarchical) _typesToFilesMap = new Dictionary<string, IEnumerable<Type>>();
            else _typesToFilesMap = _allTypes.GroupBy(c => ReferenceInspector.GetPathForType(c, stripExtension: false))
                .ToDictionary(c => c.Key, c => c.AsEnumerable());

            _isInitialized = true;
        }

        /// <summary>
        /// Sets up exported file dummy
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Exported file dummy</returns>
        public ExportedFile SetupExportedFile(string fileName = null)
        {
            if (!_context.Hierarchical && fileName == _context.TargetFile) fileName = null;
            IEnumerable<Type> types = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!_typesToFilesMap.ContainsKey(fileName))
                {
                    var allFiles = string.Join(", ", _typesToFilesMap.Keys);
                    throw new Exception("Current configuration does not contain file " + fileName + ", only " + allFiles);
                }

                types = _typesToFilesMap[fileName];
            }

            ExportedFile ef = new ExportedFile
            {
                References = GlobalReferences.Duplicate(),
                FileName = fileName,
                AllTypesIsSingleFile = !_context.Hierarchical,
                TypesToExport = _context.Hierarchical ? new HashSet<Type>(types) : _allTypesHash
            };
            ef.TypeResolver = new TypeResolver(_context, ef, ReferenceInspector);
            _context.Project.AddReferencesFromTypes(ef, _context.Global.UseModules);
            return ef;
        }

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="fileName">File name to export files to</param>
        private ExportedFile ExportTypes(string fileName = null)
        {
            var ef = SetupExportedFile(fileName);
            var gen = _context.Generators.GeneratorForNamespace(_context);
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

            if (!_context.Hierarchical)
            {
                var file = ExportTypes();
                _context.FileOperations.Export(_context.TargetFile, file);
            }
            else
            {
                foreach (var kv in _typesToFilesMap)
                {
                    var path = kv.Key;
                    var file = ExportTypes(kv.Key);
                    _context.FileOperations.Export(path, file);
                }
            }

            _context.Unlock();
            _context.FileOperations.DeployTempFiles();
        }


    }
}