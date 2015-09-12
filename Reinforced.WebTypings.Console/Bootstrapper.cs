using System;
using System.IO;
using System.Reflection;

namespace Reinforced.WebTypings.Console
{
    public static class Bootstrapper
    {
        private static string _assemblyLocalDir;
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            _assemblyLocalDir = Path.GetDirectoryName(args[0]) + "\\";

            var a = Assembly.LoadFrom(args[0]);
            TsExporter exporter = new TsExporter(a);
            exporter.Analyze();
            System.Console.WriteLine("Assembly {0} loaded and analyzed",a.FullName);
            string tmpFileName = string.Format("{0}.tmp", args[1]);
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
            System.Console.WriteLine("TypeScript sources generated");
            File.Delete(args[1]);
            File.Move(tmpFileName,args[1]);
            File.Delete(tmpFileName);
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName nm = new AssemblyName(args.Name);
            string path = Path.Combine(_assemblyLocalDir, nm.Name + ".dll");
            Assembly a = Assembly.LoadFrom(path);
            System.Console.WriteLine("{0} additionally resolved",nm);
            return a;
        }

        private static void PrintSplash()
        {
            System.Console.WriteLine(@"MVC Dynamic TypeScript Typings Generator");
        }

        private static void PrintHelp()
        {
            System.Console.WriteLine(@"Usage: {0} mvc_app.dll path_to_script.ts",Path.GetFileName(Assembly.GetEntryAssembly().CodeBase));
        }
    }
}
