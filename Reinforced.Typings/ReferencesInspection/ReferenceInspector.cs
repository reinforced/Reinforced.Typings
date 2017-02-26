using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.ReferencesInspection
{
    /// <summary>
    /// Class responsible for extracting direct dependencies from types
    /// </summary>
    public class ReferenceInspector
    {
        private ExportContext _context;
        private HashSet<Type> _allExportedTypes;

        public ReferenceInspector(ExportContext context, HashSet<Type> allExportedTypes)
        {
            _context = context;
            _allExportedTypes = allExportedTypes;
        }

        public InspectedReferences InspectGlobalReferences(Assembly[] assemblies)
        {
            var references = assemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                .Select(c => new RtReference() { Path = c.Path })
                .Union(ConfigurationRepository.Instance.References);

            if (_context.Global.UseModules)
            {
                var imports = assemblies.Where(c => c.GetCustomAttributes<TsImportAttribute>().Any())
                    .SelectMany(c => c.GetCustomAttributes<TsImportAttribute>())
                    .Select(c => new RtImport() { Target = c.ImportTarget, From = c.ImportSource, IsRequire = c.ImportRequire })
                    .Union(ConfigurationRepository.Instance.Imports);
                return new InspectedReferences(references, imports);
            }
            return new InspectedReferences(references);
        }

        public RtImport EnsureImport(Type t, string typeName, ExportedFile file)
        {
            if (file.TypesToExport.Contains(t)) return null;
            if (file.AllTypesIsSingleFile) return null;

            var relPath = GetRelativePathForType(t, file.FileName);
            if (string.IsNullOrEmpty(relPath)) return null;

            RtImport result = null;

            if (file.References.StarImports.ContainsKey(relPath))
            {
                return file.References.StarImports[relPath];
            }

            if (_context.Global.DiscardNamespacesWhenUsingModules)
            {
                var target = string.Format("{{ {0} }}", typeName);
                result = new RtImport() {From = relPath, Target = target};
                file.References.AddImport(result);
            }
            else
            {
                var alias = Path.GetFileNameWithoutExtension(relPath);
                var target = string.Format("* as {0}", alias);
                result = new RtImport() { From = relPath, Target = target };
                file.References.AddImport(result);
            }
            return result;
        }

        public RtReference EnsureReference(Type t, ExportedFile file)
        {
            var relPath = GetRelativePathForType(t, file.FileName);
            var result = new RtReference() {Path = relPath};
            file.References.AddReference(result);
            return result;
        }

        public string GetPathForType(Type t)
        {
            var fromConfiguration = ConfigurationRepository.Instance.GetPathForFile(t);
            if (!string.IsNullOrEmpty(fromConfiguration))
                return Path.Combine(_context.TargetDirectory, fromConfiguration).Replace("/", "\\");

            var ns = t.GetNamespace();
            var tn = t.GetName().ToString();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (!_context.Global.UseModules)
            {
                if (_context.Global.ExportPureTypings) tn = tn + ".d.ts";
                else tn = tn + ".ts";
            }

            if (string.IsNullOrEmpty(ns)) return Path.Combine(_context.TargetDirectory, tn);
            if (!string.IsNullOrEmpty(_context.Global.RootNamespace))
            {
                ns = ns.Replace(_context.Global.RootNamespace, string.Empty);
            }
            ns = ns.Trim('.').Replace('.', '\\');

            var pth =
                Path.Combine(
                    !string.IsNullOrEmpty(ns) ? Path.Combine(_context.TargetDirectory, ns) : _context.TargetDirectory,
                    tn);

            return pth;
        }

        private string GetRelativePathForType(Type typeToReference, string currentFile)
        {
            if (_context.Global.UseModules)
            {
                currentFile = Path.Combine(Path.GetDirectoryName(currentFile), Path.GetFileNameWithoutExtension(currentFile));
            }
            var desiredFile = GetPathForType(typeToReference);
            if (currentFile == desiredFile) return String.Empty;

            var desiredFileName = Path.GetFileName(desiredFile);

            var relPath = GetRelativeNamespacePath(Path.GetDirectoryName(currentFile),
                Path.GetDirectoryName(desiredFile));

            relPath = Path.Combine(relPath, desiredFileName);
            relPath = relPath.Replace('\\', '/');
            if (_context.Global.UseModules)
            {
                if (relPath.IndexOf("/") < 0) relPath = "./" + relPath;
            }
            return relPath;
        }


        private string GetRelativeNamespacePath(string currentNamespace, string desiredNamespace)
        {
            if (currentNamespace == desiredNamespace) return string.Empty;
            if (string.IsNullOrEmpty(currentNamespace)) return desiredNamespace;


            var current = currentNamespace.Split('\\');
            var desired = desiredNamespace.Split('\\');

            var result = new StringBuilder();
            if (string.IsNullOrEmpty(desiredNamespace))
            {
                for (var i = 0; i < current.Length; i++) result.Append("..\\");
            }
            else
            {
                var level = current.Length - 1;
                while (level >= 0 && (current.I(level) != desired.I(level)))
                {
                    result.Append("..\\");
                    level--;
                }
                level++;
                for (; level < desired.Length; level++)
                {
                    result.AppendFormat("{0}\\", desired[level]);
                }
            }
            return result.ToString().Trim('\\');
        }


        private static Type ClarifyType(MemberInfo info)
        {
            var attr = ConfigurationRepository.Instance.ForMember(info);
            if (attr != null && attr.StrongType != null) return attr.StrongType;
            if (info is PropertyInfo) return ((PropertyInfo)info).PropertyType;
            if (info is FieldInfo) return ((FieldInfo)info).FieldType;
            if (info is MethodInfo) return ((MethodInfo)info).ReturnType;
            return null;
        }

        private HashSet<Type> InspectReferences(Type element)
        {
            var alltypes = _allExportedTypes;

            var references = new HashSet<Type>();
            if (element.IsEnum) return references;

            foreach (var fi in element.GetExportedFields()) InspectTypeReferences(ClarifyType(fi), alltypes, references);
            foreach (var pi in element.GetExportedProperties()) InspectTypeReferences(ClarifyType(pi), alltypes, references);

            foreach (var mi in element.GetExportedMethods())
            {
                InspectTypeReferences(ClarifyType(mi), alltypes, references);

                foreach (var parameterInfo in mi.GetParameters())
                {
                    if (parameterInfo.IsIgnored()) continue;

                    var paramAttr = ConfigurationRepository.Instance.ForMember(parameterInfo);
                    if (paramAttr != null && paramAttr.StrongType != null)
                        InspectTypeReferences(paramAttr.StrongType, alltypes, references);
                    else InspectTypeReferences(parameterInfo.ParameterType, alltypes, references);
                }
            }
            if (element.BaseType != null) InspectTypeReferences(element.BaseType, alltypes, references);
            var interfaces = element.GetInterfaces();
            foreach (var iface in interfaces)
            {
                InspectTypeReferences(iface, alltypes, references);
            }

            return references;
        }

        private static void InspectTypeReferences(Type argument, HashSet<Type> alltypes, HashSet<Type> referenceContainer)
        {
            if (alltypes.Contains(argument)) referenceContainer.AddIfNotExists(argument);
            if (argument.IsGenericType)
            {
                var args = argument.GetGenericArguments();
                foreach (var type in args)
                {
                    InspectTypeReferences(type, alltypes, referenceContainer);
                }
            }
        }


    }

    internal static class HashSetExtensions
    {
        internal static void AddIfNotExists<T>(this HashSet<T> hashSet, T val)
        {
            if (hashSet.Contains(val)) return;
            hashSet.Add(val);
        }


    }
}