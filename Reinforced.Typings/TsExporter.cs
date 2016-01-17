using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Visitors;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Facade for final TypeScript export. This class supplies assemblies names or assemblies itself as parameter and
    ///     exports resulting TypeScript file to file or to string
    /// </summary>
    public class TsExporter : MarshalByRefObject
    {
        private readonly FilesOperations _fileOps;
        private readonly StringBuilder _referenceBuilder = new StringBuilder();
        private readonly ExportSettings _settings;
        private List<Type> _allTypes;
        private HashSet<Type> _allTypesHash;
        private ConfigurationRepository _configurationRepository;
        private bool _isAnalyzed;
        private Dictionary<string, List<Type>> _namespace = new Dictionary<string, List<Type>>();

        #region Constructors

        /// <summary>
        ///     Constructs new instance of TypeScript exporter
        /// </summary>
        /// <param name="settings"></param>
        public TsExporter(ExportSettings settings)
        {
            _settings = settings;
            _fileOps = new FilesOperations(settings);
        }

        #endregion

        private void ExtractReferences()
        {
            if (_isAnalyzed) return;
            _settings.Documentation =
                new DocumentationManager(_settings.GenerateDocumentation ? _settings.DocumentationFilePath : null);
            var fluentConfigurationPresents = _settings.ConfigurationMethod != null;
            if (fluentConfigurationPresents)
            {
                var configurationBuilder = new ConfigurationBuilder();
                _settings.ConfigurationMethod(configurationBuilder);
                _configurationRepository = configurationBuilder.Build();
                ConfigurationRepository.Instance = _configurationRepository;

                foreach (var additionalDocumentationPath in _configurationRepository.AdditionalDocumentationPathes)
                {
                    _settings.Documentation.CacheDocumentation(additionalDocumentationPath);
                }
            }

            _allTypes = _settings.SourceAssemblies
                .SelectMany(c => c.GetTypes().Where(d => d.GetCustomAttribute<TsAttributeBase>(false) != null))
                .Union(ConfigurationRepository.Instance.AttributesForType.Keys).Distinct()
                .ToList();

            _allTypesHash = new HashSet<Type>(_allTypes);

            if (_settings.Hierarchical)
            {
                foreach (var type in _allTypesHash)
                {
                    ConfigurationRepository.Instance.AddFileSeparationSettings(type);
                }
            }

            var grp = _allTypes.GroupBy(c => c.GetNamespace(true));
            _namespace = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            _settings.SourceAssemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                .Select(c => string.Format("/// <reference path=\"{0}\"/>", c.Path))
                .Union(
                    ConfigurationRepository.Instance.References.Select(
                        c => string.Format("/// <reference path=\"{0}\"/>", c)))
                .ToList()
                .ForEach(a => _referenceBuilder.AppendLine(a));

            _settings.References = _referenceBuilder.ToString();


            _isAnalyzed = true;
        }

        /// <summary>
        ///     Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="sw">TextWriter</param>
        /// <param name="tr">TypeResolver object</param>
        /// <param name="types">Types to export</param>
        public void ExportTypes(TextWriter sw, TypeResolver tr, IEnumerable<Type> types = null)
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
                    var inspected = _fileOps.GenerateInspectedReferences(type, _allTypesHash);
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
            _fileOps.ClearTempRegistry();
            ExtractReferences();
            var tr = new TypeResolver(_settings);
            _settings.Lock();

            if (!_settings.Hierarchical)
            {
                var file = _fileOps.GetTmpFile(_settings.TargetFile);
                using (var fs = File.OpenWrite(file))
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
                    .GroupBy(c => _fileOps.GetPathForType(c))
                    .ToDictionary(c => c.Key, c => c.AsEnumerable());

                foreach (var kv in typeFilesMap)
                {
                    var path = kv.Key;
                    var tmpFile = _fileOps.GetTmpFile(path);
                    using (var fs = File.OpenWrite(tmpFile))
                    {
                        using (var tw = new StreamWriter(fs))
                        {
                            ExportTypes(tw, tr, kv.Value);
                        }
                    }
                }
            }

            _settings.Unlock();
            _fileOps.DeployTempFiles();
            tr.PrintCacheInfo();
        }

        private void ExportNamespaces(IEnumerable<Type> types, TypeResolver tr, TextWriter tw)
        {
            var gen = tr.GeneratorForNamespace(_settings);
            var grp = types.GroupBy(c => c.GetNamespace(true));
            var nsp = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            var visitor = new TypeScriptExportVisitor(tw);

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
            if (_settings.WriteWarningComment)
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
        public string ExportAll()
        {
            _settings.Lock();
            ExtractReferences();

            var sb = new StringBuilder();
            var tr = new TypeResolver(_settings);
            using (var sw = new StringWriter(sb))
            {
                ExportTypes(sw, tr);
            }
            _settings.Unlock();
            return sb.ToString();
        }
    }
}