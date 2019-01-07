using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCORE1
using System.Runtime.Loader;
#endif

namespace Reinforced.Typings.Cli
{
    internal class AssemblyLocation
    {
        public AssemblyLocation(string assemblyName, string fileName)
        {
            AssemblyName = assemblyName;
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public string AssemblyName { get; private set; }
    }
    internal class AssemblyManager
    {
        private readonly string[] _sourceAssemblies;
        private int _totalLoadedAssemblies;

        public int TotalLoadedAssemblies
        {
            get { return _totalLoadedAssemblies; }
        }

        private readonly HashSet<string> _allAssembliesDirs = new HashSet<string>();
        private readonly string _referencesTmpFilePath;
        private readonly TextReader _profileReader;
        private readonly Dictionary<string, Assembly> _alreadyLoaded = new Dictionary<string, Assembly>();
        private readonly List<AssemblyLocation> _referencesCache = new List<AssemblyLocation>();
        private readonly Action<string, object[]> BuildWarn;

        public AssemblyManager(string[] sourceAssemblies, TextReader profileReader, string referencesTmpFilePath, Action<string, object[]> buildWarn)
        {
            _sourceAssemblies = sourceAssemblies;
            _profileReader = profileReader;
            _referencesTmpFilePath = referencesTmpFilePath;
            BuildWarn = buildWarn;
        }


        public Assembly[] GetAssembliesFromArgs()
        {
            BuildReferencesCache();

#if NETCORE1
            AssemblyLoadContext.Default.Resolving += CurrentDomainOnAssemblyResolve;
#else
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

#endif
            List<Assembly> assemblies = new List<Assembly>();

            foreach (var assemblyPath in _sourceAssemblies)
            {
                var pathes = LookupPossibleAssemblyPath(assemblyPath);
                foreach (var path in pathes)
                {
                    if (path == assemblyPath)
                    {
                        BuildWarn("Assembly {0} may be resolved incorrectly", new object[] { assemblyPath });
                    }
#if NETCORE1
                    var a = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#else
                    var a = Assembly.LoadFrom(path);
#endif
                    _totalLoadedAssemblies++;
                    assemblies.Add(a);
                }
            }

            return assemblies.ToArray();
        }

        private void BuildReferencesCache()
        {
            _referencesCache.Clear();

            if (string.IsNullOrEmpty(_referencesTmpFilePath) && _profileReader == null) return;
            TextReader tr = null;
            try
            {
                if (_profileReader == null)
                {
                    tr = File.OpenText(_referencesTmpFilePath);
                }
                else
                {
                    tr = _profileReader;
                }
                string reference;
                while ((reference = tr.ReadLine()) != null)
                {
                    _referencesCache.Add(new AssemblyLocation(Path.GetFileName(reference), reference));
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
        
#if NETCORE1
        private Assembly CurrentDomainOnAssemblyResolve(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            //AssemblyLoadContext.Default.Resolving -= CurrentDomainOnAssemblyResolve;
            if (assemblyName.Name.StartsWith("Reinforced.Typings.XmlSerializers")) return Assembly.GetEntryAssembly();
            if (_alreadyLoaded.ContainsKey(assemblyName.FullName)) return _alreadyLoaded[assemblyName.FullName];
            AssemblyName nm = new AssemblyName(assemblyName.Name);
            string paths = LookupAssemblyPath(nm.Name, false);
            Assembly a = null;
            foreach (var path in paths)
            {
                if (path != nm.Name) a = Assembly.LoadFrom(path);
                else BuildWarn("Assembly {0} may be resolved incorrectly", new object[] { nm.Name });

                if (a != null)
                {
                    _alreadyLoaded[args.Name] = a;
                    _totalLoadedAssemblies++;
                }
                
#if DEBUG
                Console.WriteLine("{0} additionally resolved", nm);
#endif
            }

            //AssemblyLoadContext.Default.Resolving += CurrentDomainOnAssemblyResolve;
            return a;
        }
#else
        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Reinforced.Typings.XmlSerializers")) return Assembly.GetExecutingAssembly();
            if (_alreadyLoaded.ContainsKey(args.Name)) return _alreadyLoaded[args.Name];
            AssemblyName nm = new AssemblyName(args.Name);
            var paths = LookupPossibleAssemblyPath(nm.Name, false);
            Assembly a = null;
            foreach (var path in paths)
            {
                if (path != nm.Name) a = Assembly.LoadFrom(path);
                else BuildWarn("Assembly {0} may be resolved incorrectly", new object[] { nm.Name });

                if (a != null)
                {
                    _alreadyLoaded[args.Name] = a;
                    _totalLoadedAssemblies++;
                }
                
#if DEBUG
                Console.WriteLine("{0} additionally resolved", nm);
#endif
            }

            return a;
        }
#endif


        private string[] LookupAssemblyPathInternal(string assemblyNameOrFullPath, bool storeIfFullName = true)
        {
#if DEBUG
            Console.WriteLine("Looking up for assembly {0}", assemblyNameOrFullPath);
#endif

            if (Path.IsPathRooted(assemblyNameOrFullPath) && File.Exists(assemblyNameOrFullPath))
            {
                if (storeIfFullName)
                {
                    var lastAssemblyLocalDir = Path.GetDirectoryName(assemblyNameOrFullPath) + Path.DirectorySeparatorChar;
                    if (!_allAssembliesDirs.Contains(lastAssemblyLocalDir)) _allAssembliesDirs.Add(lastAssemblyLocalDir);
                }
#if DEBUG
                Console.WriteLine("Already have full path to assembly {0}", assemblyNameOrFullPath);
#endif
                return new[] { assemblyNameOrFullPath };
            }

            var possiblePathes = _referencesCache.Where(d => d.AssemblyName == assemblyNameOrFullPath)
                .Select(d => d.FileName)
                .ToArray();


            if (possiblePathes.Length > 0)
            {

#if DEBUG
                Console.WriteLine("Assembly {0} could be found at:", assemblyNameOrFullPath);
                foreach (var assemblyLocation in possiblePathes)
                {
                    Console.WriteLine("\t{0}", assemblyLocation);
                }
#endif
                return possiblePathes;
            }

            foreach (var dir in _allAssembliesDirs)
            {
                var p = Path.Combine(dir, assemblyNameOrFullPath);
                if (File.Exists(p))
                {
#if DEBUG
                    Console.WriteLine("Assembly {0} found at {1}", assemblyNameOrFullPath, p);
#endif
                    return new[] { p };
                }
            }


            return null;
        }

        private IEnumerable<string> LookupPossibleAssemblyPath(string assemblyNameOrFullPath, bool storeIfFullName = true)
        {
            string[] checkResult;
            if (!assemblyNameOrFullPath.EndsWith(".dll") && !assemblyNameOrFullPath.EndsWith(".exe"))
            {
                var check = assemblyNameOrFullPath + ".dll";
                checkResult = LookupAssemblyPathInternal(check, storeIfFullName);

                if (checkResult.Length > 0 && checkResult.Any(d => !string.IsNullOrEmpty(d))) return checkResult.Where(d => !string.IsNullOrEmpty(d));

                check = assemblyNameOrFullPath + ".exe";
                checkResult = LookupAssemblyPathInternal(check, storeIfFullName);

                if (checkResult.Length > 0 && checkResult.Any(d => !string.IsNullOrEmpty(d))) return checkResult.Where(d => !string.IsNullOrEmpty(d));
            }

            var p = assemblyNameOrFullPath;
            checkResult = LookupAssemblyPathInternal(p, storeIfFullName);
            if (checkResult.Length > 0 && checkResult.Any(d => !string.IsNullOrEmpty(d))) return checkResult.Where(d => !string.IsNullOrEmpty(d));


            return new[] { assemblyNameOrFullPath };
        }
    }
}
