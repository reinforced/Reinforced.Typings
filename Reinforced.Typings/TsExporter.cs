using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;

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
        private List<string> _tmpFiles = new List<string>();

        #region Constructors

        /// <summary>
        /// Constructs new instance of TypeScript exporter
        /// </summary>
        /// <param name="settings"></param>
        public TsExporter(ExportSettings settings)
        {
            _settings = settings;
        }

        #endregion

        private void ExtractReferences()
        {
            if (_isAnalyzed) return;
            _allTypes = _settings.SourceAssemblies.SelectMany(c => c.GetTypes().Where(d => d.GetCustomAttribute<TsAttributeBase>() != null)).ToList();

            _namespace = _allTypes.GroupBy(c => c.GetNamespace()).ToDictionary(k => k.Key, v => v.ToList());

            _settings.SourceAssemblies.Where(c => c.GetCustomAttribute<TsReferenceAttribute>() != null)
                    .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                    .Select(c => string.Format("/// <reference path=\"{0}\"/>", c.Path))
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
                gen.Generate(n.Value, n.Key, tr, ww);
            }

            sw.Flush();
            _settings.Unlock();
        }

        private void ExportType(Type type, TextWriter tw, TypeResolver resolver)
        {
            tw.WriteLine(_referenceBuilder.ToString());
            WriterWrapper sw = new WriterWrapper(tw);
            var gen = resolver.GeneratorForNamespace(_settings);
            var n = type.GetNamespace();
            gen.WriteNamespaceBegin(n, sw);
            var converter = resolver.GeneratorFor(type, _settings);
            converter.Generate(type, resolver, sw);
            Console.WriteLine("Exported {0}", type);
            gen.WriteNamespaceEnd(sw);
            tw.Flush();
        }
        /// <summary>
        /// Exports TypeScript source according to settings
        /// </summary>
        public void Export()
        {
            _tmpFiles.Clear();
            ExtractReferences();
            if (!_settings.Hierarchical)
            {
                var file = GetTmpFile(_settings.TargetFile);
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
                    var path = GetPathForType(t);
                    var tmpFile = GetTmpFile(path);
                    using (var fs = File.OpenWrite(tmpFile))
                    {
                        using (var tw = new StreamWriter(fs))
                        {
                            WriteWarning(tw);
                            ExportType(t, tw, tr);
                        }
                    }
                }
            }
            DeployTempFiles();
        }

        private void DeployTempFiles()
        {
            foreach (var tmpFile in _tmpFiles)
            {
                var origFile = Path.GetFileNameWithoutExtension(tmpFile);
                var origDir = Path.GetDirectoryName(tmpFile);

                origFile = Path.Combine(origDir, origFile);

                if (File.Exists(origFile)) File.Delete(origFile);
                File.Move(tmpFile, origFile);
                Console.WriteLine("File replaced: {0} -> {1}", tmpFile, origFile);
            }
        }

        private string GetTmpFile(string fileName)
        {
            fileName = fileName + ".tmp";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            _tmpFiles.Add(fileName);
            return fileName;
        }

        private string GetPathForType(Type t)
        {
            var ns = t.GetNamespace();
            var tn = t.GetName();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (_settings.ExportPureTypings) tn = tn + ".d.ts";
            else tn = tn + ".ts";

            if (string.IsNullOrEmpty(ns)) return Path.Combine(_settings.TargetDirectory, tn);
            if (!string.IsNullOrEmpty(_settings.RootNamespace))
            {
                ns = ns.Replace(_settings.RootNamespace, String.Empty);
            }
            ns = ns.Replace('.', '\\');
            return Path.Combine(_settings.TargetDirectory, ns, tn);
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
