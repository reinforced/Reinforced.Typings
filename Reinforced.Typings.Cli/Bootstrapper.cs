using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Reinforced.Typings.Cli
{
    /// <summary>
    /// Class for CLI typescript typings utility
    /// </summary>
    public static class Bootstrapper
    {
        private static string _assemblyLocalDir;
        /// <summary>
        /// Usage: rtcli.exe Assembly.dll [Assembly2.dll Assembly3.dll ... etc] file.ts
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }
            Console.WriteLine("Reinforced.Typings CLI generator is starting now");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            
            var assemblies = GetAssembliesFromArgs(args);
            TsExporter exporter = new TsExporter(assemblies);
            Console.WriteLine("{0} assemblies loaded",assemblies.Length);
            var targetFileName = args[args.Length - 1];
            string tmpFileName = string.Format("{0}.tmp", targetFileName);
            using (var fs = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write))
            {
                using (var tr = new StreamWriter(fs))
                {
                    tr.WriteLine("// THIS FILE IS AUTO-GENERATED");
                    tr.WriteLine("// DO NOT MODIFY IT MANUALLY");
                    exporter.Export(tr);
                    tr.Flush();
                    fs.Flush(true);
                }
            }
            Console.WriteLine("TypeScript sources generated");
            File.Delete(targetFileName);
            File.Move(tmpFileName, targetFileName);
            File.Delete(tmpFileName);
        }

        private static Assembly[] GetAssembliesFromArgs(string[] args)
        {
            List<Assembly> assemblies = new List<Assembly>();

            for (int i = 0; i < args.Length-1; i++)
            {
                var assemblyPath = args[i];
                if (Path.IsPathRooted(assemblyPath))
                {
                    _assemblyLocalDir = Path.GetDirectoryName(assemblyPath) + "\\";
                    var a = Assembly.LoadFrom(assemblyPath);
                    assemblies.Add(a);
                }
                else
                {
                    var a = Assembly.LoadFrom(Path.Combine(_assemblyLocalDir,assemblyPath));
                    assemblies.Add(a);
                }
            }

            return assemblies.ToArray();
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName nm = new AssemblyName(args.Name);
            string path = Path.Combine(_assemblyLocalDir, nm.Name + ".dll");
            Assembly a = Assembly.LoadFrom(path);
            Console.WriteLine("{0} additionally resolved",nm);
            return a;
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"Usage: {0} Assembly.dll [Assembly2.dll Assembly3.dll ... etc] target_file.ts", Path.GetFileName(Assembly.GetEntryAssembly().CodeBase));
        }
    }
}
