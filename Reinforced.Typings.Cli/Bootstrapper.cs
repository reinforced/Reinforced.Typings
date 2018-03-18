using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCORE1
using System.Runtime.Loader;
#endif
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent;

namespace Reinforced.Typings.Cli
{
    internal static class CoreTypeExtensions
    {

        internal static PropertyInfo[] _GetProperties(this Type t, BindingFlags flags)
        {
#if NETSTANDARD15
            return t.GetTypeInfo().GetProperties(flags);
#else
            return t.GetProperties(flags);
#endif
        }
        internal static PropertyInfo _GetProperty(this Type t, string name)
        {
#if NETSTANDARD15
            return t.GetTypeInfo().GetProperty(name);
#else
            return t.GetProperty(name);
#endif
        }

        internal static MethodInfo _GetMethod(this Type t, string name)
        {
#if NETSTANDARD15
            return t.GetTypeInfo().GetMethod(name);
#else
            return t.GetMethod(name);
#endif
        }

    }
    /// <summary>
    /// Class for CLI typescript typings utility
    /// </summary>
    public static class Bootstrapper
    {
        private static ExporterConsoleParameters _parameters;
        private static readonly Dictionary<string, string> _referencesCache = new Dictionary<string, string>();
        private static readonly HashSet<string> _allAssembliesDirs = new HashSet<string>();
        private static readonly Dictionary<string, Assembly> _alreadyLoaded = new Dictionary<string, Assembly>();
        private static int _totalLoadedAssemblies;
        private static TextReader _profileReader;
        private static string _profilePath;

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
                if (string.Compare(args[0], "profile",
#if NETCORE1
                StringComparison.CurrentCultureIgnoreCase
#else
                StringComparison.InvariantCultureIgnoreCase
#endif
                    ) == 0)
                {
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine("Cannot find profile {0}, exiting", args[1]);
                        return;
                    }
                    _parameters = ExtractParametersFromFile(args[1]);
                }
                else
                {
                    _parameters = ExtractParametersFromArgs(args);
                }
                if (_parameters == null)
                {
                    Console.WriteLine("No valid parameters found. Exiting.");
                    return;
                }
                var settings = InstantiateExportContext();
                ResolveFluentMethod(settings);
                TsExporter exporter = new TsExporter(settings);
                exporter.Export();
                foreach (var rtWarning in settings.Warnings)
                {
                    var msg = VisualStudioFriendlyErrorMessage.Create(rtWarning);
                    Console.WriteLine(msg.ToString());
                }
                ReleaseReferencesTempFile();
            }
            catch (RtException rtException)
            {
                var error = VisualStudioFriendlyErrorMessage.Create(rtException);
                Console.WriteLine(error.ToString());
                Console.WriteLine(rtException.StackTrace);
                ReleaseReferencesTempFile();
                Environment.Exit(1);
            }
            catch (TargetInvocationException ex)
            {
                var e = ex.InnerException;
                BuildError(e.Message);
                Console.WriteLine(e.StackTrace);
                Environment.Exit(1);
            }
            catch (ReflectionTypeLoadException ex)
            {
                BuildError(ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (ex.LoaderExceptions != null)
                {
                    foreach (var elo in ex.LoaderExceptions)
                    {
                        BuildError(elo.Message);
                        Console.WriteLine(elo.StackTrace);
                    }
                }
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                BuildError(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }

            
            Console.WriteLine("Reinforced.Typings generation finished with total {0} assemblies loaded", _totalLoadedAssemblies);

            Console.WriteLine("Please build CompileTypeScript task to update javascript sources");
        }

        private static void ReleaseReferencesTempFile()
        {
            if (_profileReader != null) _profileReader.Dispose();
            if (!string.IsNullOrEmpty(_profilePath)) File.Delete(_profilePath);
            if (_parameters == null) return;
            if (!string.IsNullOrEmpty(_parameters.ReferencesTmpFilePath)) File.Delete(_parameters.ReferencesTmpFilePath);
        }

        private static void ResolveFluentMethod(ExportContext context)
        {
            if (string.IsNullOrEmpty(_parameters.ConfigurationMethod)) return;
            var methodPath = _parameters.ConfigurationMethod;
            var path = new Stack<string>(methodPath.Split('.'));
            var method = path.Pop();
            var fullQualifiedType = string.Join(".", path.Reverse());
            bool isFound = false;

            foreach (var sourceAssembly in context.SourceAssemblies)
            {
                var type = sourceAssembly.GetType(fullQualifiedType, false);
                if (type != null)
                {
                    var constrMethod = type._GetMethod(method);
                    if (constrMethod != null && constrMethod.IsStatic)
                    {

                        var pars = constrMethod.GetParameters();
                        if (pars.Length == 1 && pars[0].ParameterType == typeof(ConfigurationBuilder))
                        {
                            isFound = true;
                            context.ConfigurationMethod = builder => constrMethod.Invoke(null, new object[] { builder });
                            break;
                        }
                    }
                }
            }
            if (!isFound) BuildWarn("Cannot find configured fluent method '{0}'", methodPath);
        }

        public static ExportContext InstantiateExportContext()
        {
            ExportContext context = new ExportContext
            {
                Hierarchical = _parameters.Hierarchy,
                TargetDirectory = _parameters.TargetDirectory,
                TargetFile = _parameters.TargetFile,
                SourceAssemblies = GetAssembliesFromArgs(),
                DocumentationFilePath = _parameters.DocumentationFilePath
            };
            return context;
        }

        public static void BuildReferencesCache()
        {
            _referencesCache.Clear();

            if (string.IsNullOrEmpty(_parameters.ReferencesTmpFilePath) && _profileReader == null) return;
            TextReader tr = null;
            try
            {
                if (_profileReader == null)
                {
                    tr = File.OpenText(_parameters.ReferencesTmpFilePath);
                }
                else
                {
                    tr = _profileReader;
                }
                string reference;
                while ((reference = tr.ReadLine()) != null)
                {
                    _referencesCache.Add(Path.GetFileName(reference), reference);
                }
            }
            finally
            {
                if (tr != null)
                {
                    if (_profileReader == null) tr.Dispose();
                }
            }
        }

        private static string LookupAssemblyPathInternal(string assemblyNameOrFullPath, bool storeIfFullName = true)
        {
#if DEBUG
            Console.WriteLine("Looking up for assembly {0}", assemblyNameOrFullPath);
#endif

            if (Path.IsPathRooted(assemblyNameOrFullPath) && File.Exists(assemblyNameOrFullPath))
            {
                if (storeIfFullName)
                {
                    var lastAssemblyLocalDir = Path.GetDirectoryName(assemblyNameOrFullPath) + "\\";
                    if (!_allAssembliesDirs.Contains(lastAssemblyLocalDir)) _allAssembliesDirs.Add(lastAssemblyLocalDir);
                }
#if DEBUG
                Console.WriteLine("Already have full path to assembly {0}", assemblyNameOrFullPath);
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

            foreach (var dir in _allAssembliesDirs)
            {
                var p = Path.Combine(dir, assemblyNameOrFullPath);
                if (File.Exists(p))
                {
#if DEBUG
                    Console.WriteLine("Assembly {0} found at {1}", assemblyNameOrFullPath, p);
#endif
                    return p;
                }
            }


            return null;
        }

        public static string LookupAssemblyPath(string assemblyNameOrFullPath, bool storeIfFullName = true)
        {
            string checkResult;
            if (!assemblyNameOrFullPath.EndsWith(".dll") && !assemblyNameOrFullPath.EndsWith(".exe"))
            {
                var check = assemblyNameOrFullPath + ".dll";
                checkResult = LookupAssemblyPathInternal(check, storeIfFullName);

                if (!string.IsNullOrEmpty(checkResult)) return checkResult;

                check = assemblyNameOrFullPath + ".exe";
                checkResult = LookupAssemblyPathInternal(check, storeIfFullName);

                if (!string.IsNullOrEmpty(checkResult)) return checkResult;
            }

            var p = assemblyNameOrFullPath;
            checkResult = LookupAssemblyPathInternal(p, storeIfFullName);
            if (!string.IsNullOrEmpty(checkResult)) return checkResult;


            return assemblyNameOrFullPath;
        }

        public static Assembly[] GetAssembliesFromArgs()
        {
#if NETCORE1
            AssemblyLoadContext.Default.Resolving += CurrentDomainOnAssemblyResolve;
#else
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
#endif
            BuildReferencesCache();

            List<Assembly> assemblies = new List<Assembly>();

            for (int i = 0; i < _parameters.SourceAssemblies.Length; i++)
            {
                var assemblyPath = _parameters.SourceAssemblies[i];
                var path = LookupAssemblyPath(assemblyPath);
                if (path == assemblyPath)
                {
                    BuildWarn("Assembly {0} may be resolved incorrectly", assemblyPath);
                }
#if NETCORE1
                var a = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#else
                var a = Assembly.LoadFrom(path);
#endif

                _totalLoadedAssemblies++;

                assemblies.Add(a);
            }

            return assemblies.ToArray();
        }

#if NETCORE1
        private static Assembly CurrentDomainOnAssemblyResolve(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            //AssemblyLoadContext.Default.Resolving -= CurrentDomainOnAssemblyResolve;
            if (assemblyName.Name.StartsWith("Reinforced.Typings.XmlSerializers")) return Assembly.GetEntryAssembly();
            if (_alreadyLoaded.ContainsKey(assemblyName.FullName)) return _alreadyLoaded[assemblyName.FullName];
            AssemblyName nm = new AssemblyName(assemblyName.Name);
            string path = LookupAssemblyPath(nm.Name, false);
            Assembly a = null;
            if (path != nm.Name) //else - lookup failed, return null
            {
                a = context.LoadFromAssemblyPath(path);
            }
            else
            {
                BuildWarn("Assembly {0} may be resolved incorrectly", assemblyName.FullName);
            }

            if (a != null) _alreadyLoaded[nm.FullName] = a;
            _totalLoadedAssemblies++;
#if DEBUG
            Console.WriteLine("{0} additionally resolved", nm);
#endif

            //AssemblyLoadContext.Default.Resolving += CurrentDomainOnAssemblyResolve;
            return a;
        }
#else
        public static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Reinforced.Typings.XmlSerializers")) return Assembly.GetExecutingAssembly();
            if (_alreadyLoaded.ContainsKey(args.Name)) return _alreadyLoaded[args.Name];
            AssemblyName nm = new AssemblyName(args.Name);
            string path = LookupAssemblyPath(nm.Name, false);
            Assembly a = null;
            if (path != nm.Name) a = Assembly.LoadFrom(path);
            else BuildWarn("Assembly {0} may be resolved incorrectly", nm.Name);

            if (a != null) _alreadyLoaded[args.Name] = a;
            _totalLoadedAssemblies++;
#if DEBUG
            Console.WriteLine("{0} additionally resolved", nm);
#endif
            return a;
        }
#endif
        public static void PrintHelp()
        {
            Console.WriteLine("Available parameters:");
            Console.WriteLine();

            var t = typeof(ExporterConsoleParameters);
            var props = t._GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

        private static void BuildWarn(string message, params object[] args)
        {
            var warningMessage = string.Format(message, args);
            VisualStudioFriendlyErrorMessage vsm = new VisualStudioFriendlyErrorMessage(99, warningMessage, VisualStudioFriendlyMessageType.Warning, "Build");
            Console.WriteLine(vsm.ToString());
        }

        private static void BuildError(string message, params object[] args)
        {
            var errorMessage = string.Format(message, args);
            VisualStudioFriendlyErrorMessage vsm = new VisualStudioFriendlyErrorMessage(999, errorMessage, VisualStudioFriendlyMessageType.Error, "Unexpected");
            Console.WriteLine(vsm.ToString());
        }

        private static ExporterConsoleParameters ExtractParametersFromFile(string fileName)
        {
            _profilePath = fileName;
            _profileReader = File.OpenText(fileName);
            return ExporterConsoleParameters.FromFile(_profileReader);
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
                    BuildWarn("Unrecognized parameter: {0}", s);
                    continue;
                }

                var key = kv[0].Trim();
                var value = kv[1].Trim().Trim('"');

                var prop = t._GetProperty(key);
                if (prop == null)
                {
                    BuildWarn("Unrecognized parameter: {0}", key);
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

                BuildWarn("Cannot parse parameter for source property {0}", key);
            }

            try
            {
                instance.Validate();
            }
            catch (Exception ex)
            {
                BuildError("Parameters validation error: {0}", ex.Message);
                PrintHelp();
                return null;
            }
            return instance;
        }
    }
}
