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
        private readonly Assembly[] _assemblies;
        private bool _isAnalyzed;
        private readonly StringBuilder _referenceBuilder = new StringBuilder();
        private Dictionary<string, List<Type>> _namespace = new Dictionary<string, List<Type>>();

        #region Constructors
        /// <summary>
        /// Creates TsExporter instance using specified assembly
        /// </summary>
        /// <param name="assemblyPaths">Full pathes to assemblies to look up for types to export</param>
        public TsExporter(params string[] assemblyPaths)
        {
            _assemblies = assemblyPaths.Select(Assembly.LoadFrom).ToArray();
        }
        
        /// <summary>
        /// Creates TsExporter instance using specified assembly
        /// </summary>
        /// <param name="assembly">Assembly to look up for types to export</param>
        public TsExporter(Assembly assembly)
        {
            _assemblies = new Assembly[]{assembly};
        }

        /// <summary>
        /// Creates TsExporter instance using specified assembly
        /// </summary>
        /// <param name="assemblies">Assemblies to look up for types to export</param>
        public TsExporter(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies.ToArray();
        }

        /// <summary>
        /// Creates TsExporter instance using specified assembly
        /// </summary>
        /// <param name="assemblies">Assemblies to look up for types to export</param>
        public TsExporter(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }
        #endregion

        private void ExtractReferences()
        {
            var allTypes = _assemblies.SelectMany(c => c.GetTypes().Where(d => d.GetCustomAttribute<TsAttributeBase>() != null));
            _namespace = allTypes.GroupBy(c => c.GetNamespace()).ToDictionary(k => k.Key, v => v.ToList());
            
            _assemblies.Where(c => c.GetCustomAttribute<TsReferenceAttribute>() != null)
                    .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                    .Select(c => string.Format("/// <reference path=\"{0}\"/>",c.Path))
                    .ToList()
                    .ForEach(a=>_referenceBuilder.AppendLine(a));

            _isAnalyzed = true;
        }

        /// <summary>
        /// Exports TypeScript source for specified assemblies to specified TextWriter
        /// </summary>
        /// <param name="sw">TextWriter</param>
        public void Export(TextWriter sw)
        {
            if (!_isAnalyzed) ExtractReferences();
            
            sw.WriteLine(_referenceBuilder.ToString());
            TypeResolver tr = new TypeResolver();
            WriterWrapper ww = new WriterWrapper(sw);

            foreach (var n in _namespace)
            {
                ww.WriteLine("module {0} {{",n.Key);
                ww.Tab();
                foreach (var type in n.Value)
                {
                    var converter = tr.GeneratorFor(type);
                    converter.Generate(type,tr,ww);
                    Console.WriteLine("Exported {0}",type);
                }
                ww.UnTab();
                ww.WriteLine();
                ww.WriteLine("}");
            }
        }

        /// <summary>
        /// Exports TypeScript source to string
        /// </summary>
        /// <returns>String containig generated TypeScript source for specified assemblies</returns>
        public string Export()
        {
            if (!_isAnalyzed) ExtractReferences();
            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                Export(sw);
            }
            return sb.ToString();
        }
    }
}
