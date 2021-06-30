using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCORE
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
#if NETCORE
            return t.GetTypeInfo().GetProperties(flags);
#else
            return t.GetProperties(flags);
#endif
        }
        internal static PropertyInfo _GetProperty(this Type t, string name)
        {
#if NETCORE
            return t.GetTypeInfo().GetProperty(name);
#else
            return t.GetProperty(name);
#endif
        }

        internal static MethodInfo _GetMethod(this Type t, string name)
        {
#if NETCORE
            return t.GetTypeInfo().GetMethod(name);
#else
            return t.GetMethod(name);
#endif
        }

    }

    internal class ReferenceCacheEntry
    {

    }
    /// <summary>
    /// Class for CLI typescript typings utility
    /// </summary>
    public static class Bootstrapper
    {
        private static ExporterConsoleParameters _parameters;
        private static TextReader _profileReader;
        private static string _profilePath;
        private static AssemblyManager _assemblyManager;
        private static HashSet<int> _suppressedWarnings = new HashSet<int>();
        
        /// <summary>
        /// Usage: rtcli.exe Assembly.dll [Assembly2.dll Assembly3.dll ... etc] file.ts
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Reinforced.Typings CLI generator (c) 2015-2018 by Pavel B. Novikov");

            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }
            try
            {
                if (string.Compare(args[0], "profile",
#if NETCORE
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
                _suppressedWarnings = new HashSet<int>(settings.SuppressedWarningCodes);
                ResolveFluentMethod(settings);
                TsExporter exporter = new TsExporter(settings);
                exporter.Export();
                _assemblyManager.TurnOffAdditionalResolvation();
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
                // ReSharper disable once PossibleNullReferenceException
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

            if (_assemblyManager != null)
            {
                Console.WriteLine("Reinforced.Typings generation finished with total {0} assemblies loaded",
                    _assemblyManager.TotalLoadedAssemblies);
            }

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
                        if (pars.Length == 1/* && pars[0].ParameterType == typeof(ConfigurationBuilder)*/)
                        {
                            isFound = true;
                            context.ConfigurationMethod = builder => constrMethod.Invoke(null, new object[] { builder });
                            break;
                        }
                    }
                }
            }
            if (!isFound) BuildWarn(ErrorMessages.RTW0009_CannotFindFluentMethod, methodPath);
        }

        public static ExportContext InstantiateExportContext()
        {
            _assemblyManager = new AssemblyManager(_parameters.SourceAssemblies,_profileReader,_parameters.ReferencesTmpFilePath,BuildWarn);

            var srcAssemblies = _assemblyManager.GetAssembliesFromArgs();
            ExportContext context = new ExportContext(srcAssemblies)
            {
                Hierarchical = _parameters.Hierarchy,
                TargetDirectory = _parameters.TargetDirectory,
                TargetFile = _parameters.TargetFile,
                DocumentationFilePath = _parameters.DocumentationFilePath
            };
            return context;
        }

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
                        case Required.Not:
                            requiredText = "(not required)";
                            break;
                        case Required.Is:
                            requiredText = "(required)";
                            break;
                        case Required.Partially:
                            requiredText = "(sometimes required)";
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

        private static void BuildWarn(ErrorMessage msg, params object[] args)
        {
            
            if (_suppressedWarnings.Contains(msg.Code)) return;
            VisualStudioFriendlyErrorMessage vsm = VisualStudioFriendlyErrorMessage.Create(msg.Warn(args));
            Console.WriteLine(vsm.ToString());
        }

        private static void BuildError(string message, params object[] args)
        {
            var errorMessage = string.Format(message, args);
            VisualStudioFriendlyErrorMessage vsm = new VisualStudioFriendlyErrorMessage(999, errorMessage, VisualStudioFriendlyMessageType.Error, "Unexpected");
            Console.WriteLine(vsm.ToString());
        }

        public static HashSet<int> ParseSuppressedWarnings(string input)
        {
            var result = new HashSet<int>();
            if (string.IsNullOrEmpty(input)) return result;
            var values = input.Split(';');
            foreach (var warningCode in values)
            {
                //for some reason there is no StringSplitOptions for netcoreapp1.0
                if (string.IsNullOrEmpty(warningCode)) continue;
                var filtered = new string(warningCode.Where(char.IsDigit).ToArray());
                bool parsed = int.TryParse(filtered, out int intWarningCode);
                if (parsed) result.Add(intWarningCode);
                else
                {
                    BuildWarn(ErrorMessages.RTW0010_CannotParseWarningCode,warningCode);
                }
            }

            return result;
        }

        private static ExporterConsoleParameters ExtractParametersFromFile(string fileName)
        {
            _profilePath = fileName;
            _profileReader = File.OpenText(fileName);
            var result = ExporterConsoleParameters.FromFile(_profileReader);
            
            _suppressedWarnings = ParseSuppressedWarnings(result.SuppressedWarnings);
            return result;
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
                    BuildWarn(ErrorMessages.RTW0011_UnrecognizedConfigurationParameter, s);
                    continue;
                }

                var key = kv[0].Trim();
                var value = kv[1].Trim().Trim('"');

                var prop = t._GetProperty(key);
                if (prop == null)
                {
                    BuildWarn(ErrorMessages.RTW0011_UnrecognizedConfigurationParameter, key);
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

                BuildWarn(ErrorMessages.RTW0012_UnrecognizedConfigurationParameterValue, key);
            }

            try
            {
                instance.Validate();
            }
            catch (Exception ex)
            {
                BuildError("Parameter validation error: {0}", ex.Message);
                PrintHelp();
                return null;
            }

            _suppressedWarnings = ParseSuppressedWarnings(instance.SuppressedWarnings);
            return instance;
        }
    }
}
