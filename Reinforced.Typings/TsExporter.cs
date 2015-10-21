using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Xmldoc;

namespace Reinforced.Typings
{
    /// <summary>
    /// Facade for final TypeScript export. This class supplies assemblies names or assemblies itself as parameter and exports resulting TypeScript file to file or to string
    /// </summary>
    public class TsExporter : MarshalByRefObject
    {
        private readonly ExportSettings _settings;
        private bool _isAnalyzed;
        private readonly StringBuilder _referenceBuilder = new StringBuilder();
        private Dictionary<string, List<Type>> _namespace = new Dictionary<string, List<Type>>();
        private List<Type> _allTypes;
        private HashSet<Type> _allTypesHash;
        private readonly FilesOperations _fileOps;
        private ConfigurationRepository _configurationRepository;

        #region Constructors

        /// <summary>
        /// Constructs new instance of TypeScript exporter
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
            _settings.Documentation = new DocumentationManager(_settings.GenerateDocumentation ? _settings.DocumentationFilePath : null);
            if (_settings.ConfigurationMethod != null)
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
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

            var grp = _allTypes.GroupBy(c => c.GetNamespace(true));
            _namespace = grp.Where(g => !string.IsNullOrEmpty(g.Key)) // avoid anonymous types
                .ToDictionary(k => k.Key, v => v.ToList());

            _settings.SourceAssemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                    .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                    .Select(c => string.Format("/// <reference path=\"{0}\"/>", c.Path))
                    .Union(ConfigurationRepository.Instance.References.Select(c => string.Format("/// <reference path=\"{0}\"/>", c)))
                    .ToList()
                    .ForEach(a => _referenceBuilder.AppendLine(a));

            _settings.References = _referenceBuilder.ToString();

            

            _isAnalyzed = true;
        }

        /// <summary>
        /// Exports TypeScript source to specified TextWriter according to settings
        /// </summary>
        /// <param name="sw">TextWriter</param>
        public void ExportAll(TextWriter sw)
        {
            _settings.Lock();

            ExtractReferences();

            sw.WriteLine(_referenceBuilder.ToString());
            TypeResolver tr = new TypeResolver(_settings);
            WriterWrapper ww = new WriterWrapper(sw);

            var gen = tr.GeneratorForNamespace(_settings);

            foreach (var n in _namespace)
            {
                var ns = n.Key;
                if (ns == "-") ns = String.Empty;
                gen.Generate(n.Value, ns, tr, ww);
            }

            sw.Flush();
            _settings.Unlock();
            tr.PrintCacheInfo();
        }

        private void ExportType(Type type, TextWriter tw, TypeResolver resolver)
        {
            tw.WriteLine(_referenceBuilder.ToString());
            var n = type.GetNamespace();
            tw.WriteLine(_fileOps.GenerateInspectedReferences(type, _allTypesHash, n));

            WriterWrapper sw = new WriterWrapper(tw);
            var gen = resolver.GeneratorForNamespace(_settings);

            gen.WriteNamespaceBegin(n, sw);
            var converter = resolver.GeneratorFor(type, _settings);
            converter.Generate(type, resolver, sw);
            Console.WriteLine("Exported {0}", type);
            gen.WriteNamespaceEnd(n, sw);
            tw.Flush();
        }
        /// <summary>
        /// Exports TypeScript source according to settings
        /// </summary>
        public void Export()
        {
            _fileOps.ClearTempRegistry();
            ExtractReferences();
            if (!_settings.Hierarchical)
            {
                var file = _fileOps.GetTmpFile(_settings.TargetFile);
                using (var fs = File.OpenWrite(file))
                {
                    using (var tw = new StreamWriter(fs))
                    {
                        WriteWarning(tw);
                        ExportAll(tw);
                    }
                }
            }
            else
            {
                TypeResolver tr = new TypeResolver(_settings);
                foreach (var t in _allTypes)
                {
                    var path = _fileOps.GetPathForType(t);
                    var tmpFile = _fileOps.GetTmpFile(path);
                    using (var fs = File.OpenWrite(tmpFile))
                    {
                        using (var tw = new StreamWriter(fs))
                        {
                            WriteWarning(tw);
                            ExportType(t, tw, tr);
                        }
                    }
                }
                tr.PrintCacheInfo();
            }
            _fileOps.DeployTempFiles();
        }


        private void WriteWarning(TextWriter tw)
        {
            if (_settings.WriteWarningComment)
            {
                tw.WriteLine("//     This code was generated by a Reinforced.Typings tool. ");
                tw.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
                tw.WriteLine("//     the code is regenerated.");
            }
        }

        /// <summary>
        /// Exports TypeScript source to string
        /// </summary>
        /// <returns>String containig generated TypeScript source for specified assemblies</returns>
        public string ExportAll()
        {
            _settings.Lock();
            ExtractReferences();

            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                ExportAll(sw);
            }
            _settings.Unlock();
            return sb.ToString();

        }
    }
}
