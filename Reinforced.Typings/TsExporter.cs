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
        private readonly ExportContext _context;
        private List<Type> _allTypes;
        private HashSet<Type> _allTypesHash;
        private ConfigurationRepository _configurationRepository;
        private bool _isInitialized;

        /// <summary>
        /// Global references extracted from current configuration
        /// </summary>
        public InspectedReferences GlobalReferences { get; private set; }

        /// <summary>
        /// Reference inspector instance
        /// </summary>
        public ReferenceInspector ReferenceInspector { get; private set; }

        /// <summary>
        /// Type resolver instance being used during export
        /// </summary>
        public TypeResolver TypeResolver { get; private set; }

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
                var configurationBuilder = new ConfigurationBuilder(_context.Global);
                _context.ConfigurationMethod(configurationBuilder);
                _configurationRepository = configurationBuilder.Build();
                ConfigurationRepository.Instance = _configurationRepository;

                foreach (var additionalDocumentationPath in _configurationRepository.AdditionalDocumentationPathes)
                {
                    _context.Documentation.CacheDocumentation(additionalDocumentationPath, _context.Warnings);
                }
            }

            ReferenceInspector = new ReferenceInspector(_context.TargetDirectory,
                _context.Global.ExportPureTypings,
                _context.Global.RootNamespace,
                _context.Global.UseModules,
                _context.Global.DiscardNamespacesWhenUsingModules);

            _context.Documentation =
                new DocumentationManager(_context.Global.GenerateDocumentation ? _context.DocumentationFilePath : null, _context.Warnings);


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

            GlobalReferences = ReferenceInspector.InspectGlobalReferences(_context.SourceAssemblies);
            TypeResolver = new TypeResolver(_context);
            _isInitialized = true;
        }

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="types">Types to export</param>
        private ExportedFile ExportTypes(IEnumerable<Type> types = null)
        {

            ExportedFile ef = new ExportedFile
            {
                GlobalReferences = GlobalReferences,
                References = InspectReferences()
            };
            TypeResolver.CurrentFile = ef;
            ef.Namespaces = ExportNamespaces(types ?? _allTypes);
            TypeResolver.CurrentFile = null;
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
                    var inspected = ReferenceInspector.GenerateInspectedReferences(type, _allTypesHash);
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
            Initialize();

            _context.Lock();

            if (!_context.Hierarchical)
            {
                var file = ExportTypes();
                _context.FileOperations.Export(_context.TargetFile, file);
            }
            else
            {
                var typeFilesMap = _allTypes
                    .GroupBy(c => ReferenceInspector.GetPathForType(c))
                    .ToDictionary(c => c.Key, c => c.AsEnumerable());

                foreach (var kv in typeFilesMap)
                {
                    var path = kv.Key;
                    var file = ExportTypes(kv.Value);
                    _context.FileOperations.Export(path, file);
                }
            }

            _context.Unlock();
            _context.FileOperations.DeployTempFiles();
            TypeResolver.PrintCacheInfo();
        }

        private RtNamespace[] ExportNamespaces(IEnumerable<Type> types)
        {
            var gen = TypeResolver.GeneratorForNamespace(_context);
            var grp = types.GroupBy(c => c.GetNamespace(true));
            var nsp = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            List<RtNamespace> result = new List<RtNamespace>(nsp.Count);
            foreach (var n in nsp)
            {
                var ns = n.Key;
                if (ns == "-") ns = string.Empty;
                var module = gen.Generate(n.Value, ns, TypeResolver);
                result.Add(module);
            }
            return result.ToArray();
        }
    }
}