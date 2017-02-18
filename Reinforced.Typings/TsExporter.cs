using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Visitors;
using Reinforced.Typings.Visitors.TypeScript;
using Reinforced.Typings.Visitors.Typings;
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
        private ReferenceInspector _referenceInspector;

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

        private void ExtractReferences()
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

            _context.SourceAssemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                .Select(c => string.Format("/// <reference path=\"{0}\"/>", c.Path))
                .Union(
                    ConfigurationRepository.Instance.References.Select(
                        c => string.Format("/// <reference path=\"{0}\"/>", c)))
                .ToList()
                .ForEach(a => _referenceBuilder.AppendLine(a));

            _isAnalyzed = true;
        }

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="sw">TextWriter</param>
        /// <param name="tr">TypeResolver object</param>
        /// <param name="types">Types to export</param>
        private void ExportTypes(TextWriter sw, TypeResolver tr, IEnumerable<Type> types = null)
        {
            ExportReferences(sw, types);
            if (types == null) types = _allTypes;
            ExportNamespaces(types, tr, sw);
        }

        private void ExportReferences(TextWriter tw, IEnumerable<Type> types = null)
        {
            WriteWarning(tw);
            tw.WriteLine(_referenceBuilder.ToString());
            if (types != null)
            {
                HashSet<string> pathes = new HashSet<string>();
                foreach (var type in types)
                {
                    var inspected = _referenceInspector.GenerateInspectedReferences(type, _allTypesHash);
                    if (!string.IsNullOrEmpty(inspected) && !string.IsNullOrWhiteSpace(inspected))
                    {
                        pathes.AddIfNotExists(inspected);
                    }
                }
                foreach (var path in pathes)
                {
                    tw.WriteLine(path);
                }

            }
        }

        /// <summary>
        ///     Exports TypeScript source according to settings
        /// </summary>
        public void Export()
        {
            _context.FileOperations.ClearTempRegistry();
            ExtractReferences();
            var tr = new TypeResolver(_context);
            _context.Lock();

            if (!_context.Hierarchical)
            {
                using (var fs = _context.FileOperations.GetTmpFile(_context.TargetFile))
                {
                    using (var tw = new StreamWriter(fs))
                    {
                        ExportTypes(tw, tr);
                    }
                }
            }
            else
            {
                var typeFilesMap = _allTypes
                    .GroupBy(c => _referenceInspector.GetPathForType(c))
                    .ToDictionary(c => c.Key, c => c.AsEnumerable());

                foreach (var kv in typeFilesMap)
                {
                    var path = kv.Key;
                    using (var fs = _context.FileOperations.GetTmpFile(path))
                    {
                        using (var tw = new StreamWriter(fs))
                        {
                            ExportTypes(tw, tr, kv.Value);
                        }
                    }
                }
            }

            _context.Unlock();
            _context.FileOperations.DeployTempFiles();
            tr.PrintCacheInfo();
        }

        private void ExportNamespaces(IEnumerable<Type> types, TypeResolver tr, TextWriter tw)
        {
            var gen = tr.GeneratorForNamespace(_context);
            var grp = types.GroupBy(c => c.GetNamespace(true));
            var nsp = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            var visitor = _context.ExportPureTypings ? new TypingsExportVisitor(tw) : new TypeScriptExportVisitor(tw);

            foreach (var n in nsp)
            {
                var ns = n.Key;
                if (ns == "-") ns = string.Empty;
                var module = gen.Generate(n.Value, ns, tr);
                visitor.Visit(module);
            }
            tw.Flush();
        }

        private void WriteWarning(TextWriter tw)
        {
            if (_context.WriteWarningComment)
            {
                tw.WriteLine("//     This code was generated by a Reinforced.Typings tool. ");
                tw.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
                tw.WriteLine("//     the code is regenerated.");
                tw.WriteLine();
            }
        }

        /// <summary>
        ///     Exports TypeScript source to string
        /// </summary>
        /// <returns>String containig generated TypeScript source for specified assemblies</returns>
        //public string ExportAll()
        //{
        //    _context.Lock();
        //    ExtractReferences();

        //    var sb = new StringBuilder();
        //    var tr = new TypeResolver(_context);
        //    using (var sw = new StringWriter(sb))
        //    {
        //        ExportTypes(sw, tr);
        //    }
        //    _context.Unlock();
        //    return sb.ToString();
        //}
    }
}