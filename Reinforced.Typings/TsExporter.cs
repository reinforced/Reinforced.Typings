using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Facade for final TypeScript export. This class supplies assemblies names or assemblies itself as parameter and
    ///     exports resulting TypeScript file to file or to string
    /// </summary>
    public class TsExporter : MarshalByRefObject
    {
        private InspectedReferences _globalReferences;
        private readonly ExportContext _context;
        private List<Type> _allTypes;
        private HashSet<Type> _allTypesHash;
        private ConfigurationRepository _configurationRepository;
        private bool _isAnalyzed;
        private readonly ReferenceInspector _referenceInspector;


        #region Constructors

        /// <summary>
        ///     Constructs new instance of TypeScript exporter
        /// </summary>
        /// <param name="context"></param>
        public TsExporter(ExportContext context)
        {
            _context = context;
            _referenceInspector = new ReferenceInspector(context.TargetDirectory, context.ExportPureTypings, context.RootNamespace);
        }

        #endregion

        private void Analyze()
        {
            if (_isAnalyzed) return;
            _context.Documentation =
                new DocumentationManager(_context.GenerateDocumentation ? _context.DocumentationFilePath : null, _context.Warnings);
            var fluentConfigurationPresents = _context.ConfigurationMethod != null;
            if (fluentConfigurationPresents)
            {
                var configurationBuilder = new ConfigurationBuilder();
                _context.ConfigurationMethod(configurationBuilder);
                _configurationRepository = configurationBuilder.Build();
                ConfigurationRepository.Instance = _configurationRepository;

                foreach (var additionalDocumentationPath in _configurationRepository.AdditionalDocumentationPathes)
                {
                    _context.Documentation.CacheDocumentation(additionalDocumentationPath, _context.Warnings);
                }
            }

            _allTypes = _context.SourceAssemblies
                .SelectMany(c => c.GetTypes().Where(d => d.GetCustomAttribute<TsAttributeBase>(false) != null))
                .Union(ConfigurationRepository.Instance.AttributesForType.Keys).Distinct()
                .ToList();

            _allTypesHash = new HashSet<Type>(_allTypes);

            if (_context.Hierarchical)
            {
                foreach (var type in _allTypesHash)
                {
                    ConfigurationRepository.Instance.AddFileSeparationSettings(type);
                }
            }

            _globalReferences = _referenceInspector.InspectGlobalReferences(_context.SourceAssemblies);

            _isAnalyzed = true;
        }

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="sw">TextWriter</param>
        /// <param name="tr">TypeResolver object</param>
        /// <param name="types">Types to export</param>
        private ExportedFile ExportTypes(TypeResolver tr, IEnumerable<Type> types = null)
        {

            ExportedFile ef = new ExportedFile
            {
                GlobalReferences = _globalReferences,
                References = InspectReferences()
            };
            ef.Namespaces = ExportNamespaces(types ?? _allTypes, tr);
            return ef;
        }

        private InspectedReferences InspectReferences(IEnumerable<Type> types = null)
        {
            if (types != null)
            {
                List<RtReference> refs = new List<RtReference>();
                List<RtImport> imports = new List<RtImport>();
                foreach (var type in types)
                {
                    var inspected = _referenceInspector.GenerateInspectedReferences(type, _allTypesHash);
                    refs.AddRange(inspected.References);
                    imports.AddRange(inspected.Imports);
                }
                return new InspectedReferences(refs, imports);
            }
            else
            {
                return new InspectedReferences();
            }

        }

        /// <summary>
        ///     Exports TypeScript source according to settings
        /// </summary>
        public void Export()
        {
            _context.FileOperations.ClearTempRegistry();
            Analyze();
            var tr = new TypeResolver(_context);
            _context.Lock();

            if (!_context.Hierarchical)
            {
                var file = ExportTypes(tr);
                _context.FileOperations.Export(_context.TargetFile, file);
            }
            else
            {
                var typeFilesMap = _allTypes
                    .GroupBy(c => _referenceInspector.GetPathForType(c))
                    .ToDictionary(c => c.Key, c => c.AsEnumerable());

                foreach (var kv in typeFilesMap)
                {
                    var path = kv.Key;
                    var file = ExportTypes(tr, kv.Value);
                    _context.FileOperations.Export(path, file);
                }
            }

            _context.Unlock();
            _context.FileOperations.DeployTempFiles();
            tr.PrintCacheInfo();
        }

        private RtNamespace[] ExportNamespaces(IEnumerable<Type> types, TypeResolver tr)
        {
            var gen = tr.GeneratorForNamespace(_context);
            var grp = types.GroupBy(c => c.GetNamespace(true));
            var nsp = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            List<RtNamespace> result = new List<RtNamespace>(nsp.Count);
            foreach (var n in nsp)
            {
                var ns = n.Key;
                if (ns == "-") ns = string.Empty;
                var module = gen.Generate(n.Value, ns, tr);
                result.Add(module);
            }
            return result.ToArray();
        }
    }
}