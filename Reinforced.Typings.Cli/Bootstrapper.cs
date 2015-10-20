using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Fluent;

namespace Reinforced.Typings.Cli
{
    /// <summary>
    /// Class for CLI typescript typings utility
    /// </summary>
    public static class Bootstrapper
    {
        private static ExporterConsoleParameters _parameters;
        private static Dictionary<string, string> _referencesCache = new Dictionary<string, string>();
        private static string _lastAssemblyLocalDir;
        private static int _totalLoadedAssemblies;


        /// <summary>
        /// Usage: rtcli.exe Assembly.dll [Assembly2.dll Assembly3.dll ... etc] file.ts
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Reinforced.Typings CLI generator (c) 2015 by Pavel B. Novikov");

            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }
            try
            {
                _parameters = ExtractParametersFromArgs(args);
                if (_parameters == null)
                {
                    Console.WriteLine("No valid parameters found. Exiting.");
                    Environment.Exit(0);
                }
                var settings = InstantiateExportSettings();
                ResolveFluentMethod(settings);
                TsExporter exporter = new TsExporter(settings);
                exporter.Export();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(1);
            }

            Console.WriteLine("Reinforced.Typings generation finished with total {0} assemblies loaded", _totalLoadedAssemblies);

            Console.WriteLine("Please build CompileTypeScript task to update javascript sources");

        }

        private static void ResolveFluentMethod(ExportSettings settings)
        {
            if (string.IsNullOrEmpty(_parameters.ConfigurationMethod)) return;
            var methodPath = _parameters.ConfigurationMethod;
            var path = new Stack<string>(methodPath.Split('.'));
            var method = path.Pop();
            var fullQualifiedType = string.Join(".", path.Reverse());

            foreach (var sourceAssembly in settings.SourceAssemblies)
            {
                var type = sourceAssembly.GetType(fullQualifiedType, false);
                if (type != null)
                {
                    var constrMethod = type.GetMethod(method);
                    if (constrMethod != null && constrMethod.IsStatic)
                    {

                        var pars = constrMethod.GetParameters();
                        if (pars.Length == 1 && pars[0].ParameterType == typeof(ConfigurationBuilder))
                        {
                            settings.ConfigurationMethod = builder => constrMethod.Invoke(null, new object[] { builder });
                            break;
                        }
                    }
                }
            }
        }

        public static ExportSettings InstantiateExportSettings()
        {
            ExportSettings settings = new ExportSettings
            {
                ExportPureTypings = _parameters.ExportPureTypings,
                Hierarchical = _parameters.Hierarchy,
                TargetDirectory = _parameters.TargetDirectory,
                TargetFile = _parameters.TargetFile,
                WriteWarningComment = _parameters.WriteWarningComment,
                SourceAssemblies = GetAssembliesFromArgs(),
                RootNamespace = _parameters.RootNamespace,
                CamelCaseForMethods = _parameters.CamelCaseForMethods,
                CamelCaseForProperties = _parameters.CamelCaseForProperties,
                DocumentationFilePath = _parameters.DocumentationFilePath,
                GenerateDocumentation = _parameters.GenerateDocumentation
            };
            return settings;
        }

        public static void BuildReferencesCache()
        {
            _referencesCache.Clear();
            foreach (var reference in _parameters.References)
            {
                _referencesCache.Add(Path.GetFileName(reference), reference);
            }
        }

        public static string LookupAssemblyPath(string assemblyNameOrFullPath, bool storeIfFullName = true)
        {
            if (!assemblyNameOrFullPath.EndsWith(".dll")) assemblyNameOrFullPath = assemblyNameOrFullPath + ".dll";
#if DEBUG
            Console.WriteLine("Looking up for assembly {0}", assemblyNameOrFullPath);
#endif

            if (Path.IsPathRooted(assemblyNameOrFullPath))
            {
                if (storeIfFullName)
                {
                    _lastAssemblyLocalDir = Path.GetDirectoryName(assemblyNameOrFullPath) + "\\";
                }
#if DEBUG
                  Console.WriteLine("Already have full path to assembly {0}",assemblyNameOrFullPath);
#endif
                return assemblyNameOrFullPath;
            }

            if (_referencesCache.ContainsKey(assemblyNameOrFullPath))
            {
                var rf = _referencesCache[assemblyNameOrFullPath];
#if DEBUG
                Console.WriteLine("Assembly {0} found at {1}", assemblyNameOrFullPath, rf);
#endif
                return rf;
            }
            var p = Path.Combine(_lastAssemblyLocalDir, assemblyNameOrFullPath);
            if (File.Exists(p))
            {
#if DEBUG
                Console.WriteLine("Assembly {0} found at {1}", assemblyNameOrFullPath, p);
#endif
                return p;
            }

            Console.WriteLine("Warning! Probably assembly {0} not found", assemblyNameOrFullPath, p);
            return assemblyNameOrFullPath;
        }

        public static Assembly[] GetAssembliesFromArgs()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            BuildReferencesCache();

            List<Assembly> assemblies = new List<Assembly>();

            for (int i = 0; i < _parameters.SourceAssemblies.Length; i++)
            {
                var assemblyPath = _parameters.SourceAssemblies[i];
                var path = LookupAssemblyPath(assemblyPath);
                var a = Assembly.LoadFrom(path);
                _totalLoadedAssemblies++;

                assemblies.Add(a);
            }

            return assemblies.ToArray();
        }

        public static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Reinforced.Typings.XmlSerializers")) return Assembly.GetExecutingAssembly();
            AssemblyName nm = new AssemblyName(args.Name);
            string path = LookupAssemblyPath(nm.Name + ".dll", false);
            Assembly a = Assembly.LoadFrom(path);
            _totalLoadedAssemblies++;
#if DEBUG
            Console.WriteLine("{0} additionally resolved", nm);
#endif
            return a;
        }

        public static void PrintHelp()
        {
            Console.WriteLine("Available parameters:");
            Console.WriteLine();

            var t = typeof(ExporterConsoleParameters);
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in props)
            {
                var attr = propertyInfo.GetCustomAttribute<ConsoleHelpAttribute>();
                if (attr != null)
                {
                    var req = attr.RequiredType;
                    string requiredText = null;
                    switch (req)
                    {
                        case Required.NotReuired:
                            requiredText = "(not requred)";
                            break;
                        case Required.Reuired:
                            requiredText = "(requred)";
                            break;
                        case Required.Partially:
                            requiredText = "(sometimes requred)";
                            break;
                    }
                    Console.WriteLine(propertyInfo.Name + " " + requiredText);

                    var lines = attr.HelpText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        Console.WriteLine("\t{0}", line);
                    }

                    Console.WriteLine();
                }
            }
        }

        public static ExporterConsoleParameters ExtractParametersFromArgs(string[] args)
        {
            var t = typeof(ExporterConsoleParameters);
            var instance = new ExporterConsoleParameters();
            foreach (var s in args)
            {
                var trimmed = s.TrimStart('-');
                var kv = trimmed.Split('=');
                if (kv.Length != 2)
                {
                    Console.WriteLine("Unrecognized parameter: {0}", s);
                    continue;
                }

                var key = kv[0].Trim();
                var value = kv[1].Trim().Trim('"');

                var prop = t.GetProperty(key);
                if (prop == null)
                {
                    Console.WriteLine("Unrecognized parameter: {0}", key);
                    continue;
                }

                if (prop.PropertyType == typeof(bool))
                {
                    bool parsedValue = Boolean.Parse(value);
                    prop.SetValue(instance, parsedValue);
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(instance, value);
                    continue;
                }

                if (prop.PropertyType == typeof(string[]))
                {
                    var parsedValue = value.Split(';');
                    prop.SetValue(instance, parsedValue);
                    continue;
                }

                Console.WriteLine("Cannot parse parameter for source property {0}", key);
            }

            try
            {
                instance.Validate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parameters validation error: {0}", ex.Message);
                PrintHelp();
                return null;
            }
            return instance;
        }
    }
}
