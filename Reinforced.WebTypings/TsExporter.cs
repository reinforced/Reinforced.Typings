using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reinforced.WebTypings
{
    public class TsExporter : MarshalByRefObject
    {
        private readonly Assembly[] _assemblies;
        private bool _isAnalyzed;
        private StringBuilder _referenceBuilder = new StringBuilder();

        private Dictionary<string, List<Type>> _namespace = new Dictionary<string, List<Type>>();

        public TsExporter(params string[] assemblyPaths)
        {
            _assemblies = assemblyPaths.Select(Assembly.LoadFrom).ToArray();
        }
        
        public TsExporter(Assembly assembly)
        {
            _assemblies = new Assembly[]{assembly};
        }

        public TsExporter(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies.ToArray();
        }

        public TsExporter(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public void Analyze()
        {
            var allTypes = _assemblies.SelectMany(c => c.GetTypes().Where(d => d.GetCustomAttribute<TsAttributeBase>() != null));
            _namespace = allTypes.GroupBy(c => c.GetNamespace()).ToDictionary(k => k.Key, v => v.ToList());
            
            _assemblies.Where(c => c.GetCustomAttribute<TsReference>() != null)
                    .SelectMany(c => c.GetCustomAttributes<TsReference>())
                    .Select(c => string.Format("/// <reference path=\"{0}\"/>",c.Path))
                    .ToList()
                    .ForEach(a=>_referenceBuilder.AppendLine(a));

            _isAnalyzed = true;
        }

        public void Export(TextWriter sw)
        {
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

        public string Export()
        {
            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                Export(sw);
            }
            return sb.ToString();
        }
    }
}
