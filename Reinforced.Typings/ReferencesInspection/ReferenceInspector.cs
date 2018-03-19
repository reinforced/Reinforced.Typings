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
    public sealed class ReferenceInspector
    {
        private readonly ExportContext _context;
        private readonly HashSet<Type> _allExportedTypes;

        internal ReferenceInspector(ExportContext context, HashSet<Type> allExportedTypes)
        {
            _context = context;
            _allExportedTypes = allExportedTypes;
        }

        /// <summary>
        /// Inspects global assemblies defined with assembly attributes or fluent methods
        /// </summary>
        /// <returns>Set of inspected references</returns>
        public InspectedReferences InspectGlobalReferences()
        {
            var assemblies = _context.SourceAssemblies;
            var references = assemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                .Select(c => new RtReference() { Path = c.Path })
                .Union(_context.Project.References);

            if (_context.Global.UseModules)
            {
                var imports = assemblies.Where(c => c.GetCustomAttributes<TsImportAttribute>().Any())
                    .SelectMany(c => c.GetCustomAttributes<TsImportAttribute>())
                    .Select(c => new RtImport() { Target = c.ImportTarget, From = c.ImportSource, IsRequire = c.ImportRequire })
                    .Union(_context.Project.Imports);
                return new InspectedReferences(references, imports);
            }
            return new InspectedReferences(references);
        }

        /// <summary>
        /// Ensures that imports for specified type presents in specified file
        /// </summary>
        /// <param name="t">Type to import</param>
        /// <param name="typeName">Type name (probably overriden)</param>
        /// <param name="file">Exported file</param>
        /// <returns>Import AST node or null if no import needed. Returns existing import in case if type is already imported</returns>
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
                result = new RtImport() { From = relPath, Target = target };
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

        /// <summary>
        /// Ensures that reference for specified type presents in specified file
        /// </summary>
        /// <param name="t">Type to reference</param>
        /// <param name="file">Exported file</param>
        /// <returns>Reference AST node or null if no reference needed. Returns existing reference in case if type is already referenced</returns>
        public RtReference EnsureReference(Type t, ExportedFile file)
        {
            if (file.TypesToExport.Contains(t)) return null;
            if (file.AllTypesIsSingleFile) return null;
            var relPath = GetRelativePathForType(t, file.FileName);
            var result = new RtReference() { Path = relPath };
            file.References.AddReference(result);
            return result;
        }

        /// <summary>
        /// Retrieves full path to file where specified type will be exported to
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="stripExtension">Remove file extension. Set to false if you still want to get path with extension in case of module export</param>
        /// <returns>Full path to file containing exporting type</returns>
        public string GetPathForType(Type t, bool stripExtension = true)
        {
            var fromConfiguration = _context.Project.GetPathForFile(t);
            if (!string.IsNullOrEmpty(fromConfiguration))
            {
                if (_context.Global.UseModules && stripExtension)
                {
                    if (fromConfiguration.EndsWith(".d.ts"))
                        fromConfiguration = fromConfiguration.Substring(0, fromConfiguration.Length - 5);
                    if (fromConfiguration.EndsWith(".ts"))
                        fromConfiguration = fromConfiguration.Substring(0, fromConfiguration.Length - 3);
                }
                var r = Path.Combine(_context.TargetDirectory, fromConfiguration.Replace("/", "\\")).Replace("/", "\\");
                return r;
            }

            var ns = _context.Project.Blueprint(t).GetNamespace();
            var tn = _context.Project.Blueprint(t).GetName().ToString();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (!_context.Global.UseModules || !stripExtension)
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
                if (!relPath.StartsWith(".")) relPath = "./" + relPath;
            }
            return relPath;
        }


        private string GetRelativeNamespacePath(string currentNamespace, string desiredNamespace)
        {
            if (currentNamespace == desiredNamespace) return string.Empty;
            if (string.IsNullOrEmpty(currentNamespace)) return desiredNamespace;


            var current = currentNamespace.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var desired = desiredNamespace.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();
            if (string.IsNullOrEmpty(desiredNamespace))
            {
                for (var i = 0; i < current.Length; i++) result.Append("..\\");
            }
            else
            {
                var level = current.Length;
                //var cr1 = current.I(level);
                //var ds1 = desired.I(level);
                while (level >= 0 && (!ArrayExtensions.PartialCompare(current, desired, level)))
                {
                    //var cr = current.I(level);
                    //var ds = desired.I(level);
                    result.Append("../");
                    level--;

                }
                //level++;
                for (; level < desired.Length; level++)
                {
                    result.AppendFormat("{0}/", desired[level]);
                }
            }
            return result.ToString().Trim('/');
        }


        private Type ClarifyType(MemberInfo info)
        {
            
            var attr = _context.Project.Blueprint(info.DeclaringType).ForMember(info);
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
            if (element._IsEnum()) return references;
            var bp = _context.Project.Blueprint(element);
            foreach (var fi in bp.GetExportedFields()) InspectTypeReferences(ClarifyType(fi), alltypes, references);
            foreach (var pi in bp.GetExportedProperties()) InspectTypeReferences(ClarifyType(pi), alltypes, references);

            foreach (var mi in bp.GetExportedMethods())
            {
                InspectTypeReferences(ClarifyType(mi), alltypes, references);

                foreach (var parameterInfo in mi.GetParameters())
                {
                    if (bp.IsIgnored(parameterInfo)) continue;

                    var paramAttr = _context.Project.Blueprint(element).ForMember(parameterInfo);
                    if (paramAttr != null && paramAttr.StrongType != null)
                        InspectTypeReferences(paramAttr.StrongType, alltypes, references);
                    else InspectTypeReferences(parameterInfo.ParameterType, alltypes, references);
                }
            }
            if (element._BaseType() != null) InspectTypeReferences(element._BaseType(), alltypes, references);
            var interfaces = element._GetInterfaces();
            foreach (var iface in interfaces)
            {
                InspectTypeReferences(iface, alltypes, references);
            }

            return references;
        }

        private static void InspectTypeReferences(Type argument, HashSet<Type> alltypes, HashSet<Type> referenceContainer)
        {
            if (alltypes.Contains(argument)) referenceContainer.AddIfNotExists(argument);
            if (argument._IsGenericType())
            {
                var args = argument._GetGenericArguments();
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