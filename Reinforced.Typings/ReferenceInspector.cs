using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings
{
    internal class ReferenceInspector
    {
        private readonly string _targetDirectory;
        private readonly bool _exportPureTypings;
        private readonly string _rootNamespace;

        public ReferenceInspector(string targetDirectory, bool exportPureTypings, string rootNamespace)
        {
            _targetDirectory = targetDirectory;
            _exportPureTypings = exportPureTypings;
            _rootNamespace = rootNamespace;
        }

        internal InspectedReferences InspectGlobalReferences(Assembly[] assemblies)
        {
            
        }

        internal InspectedReferences GenerateInspectedReferences(Type element, HashSet<Type> alltypes)
        {
            var inspectedTypes = InspectReferences(element, alltypes);
            var references = new HashSet<string>();
            var types = ConfigurationRepository.Instance.ReferencesForType(element);

            if (types != null)
            {
                foreach (var attr in types)
                {
                    if (attr.Type != element)
                    {
                        var path = attr.Type != null ? GetRelativePathForType(attr.Type, element) : attr.RawPath;
                        if (!string.IsNullOrEmpty(path)) references.AddIfNotExists(path);
                    }
                }
            }
            foreach (var inspectedType in inspectedTypes)
            {
                if (inspectedType != element)
                {
                    var path = GetRelativePathForType(inspectedType, element);
                    if (!string.IsNullOrEmpty(path)) references.AddIfNotExists(path);
                }
            }

            var referenceNodes = references.Select(c => new RtReference() { Path = c });

            return new InspectedReferences(referenceNodes);
        }

        public string GetPathForType(Type t)
        {
            var fromConfiguration = ConfigurationRepository.Instance.GetPathForFile(t);
            if (!string.IsNullOrEmpty(fromConfiguration))
                return Path.Combine(_targetDirectory, fromConfiguration).Replace("/", "\\");

            var ns = t.GetNamespace();
            var tn = t.GetName().ToString();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (_exportPureTypings) tn = tn + ".d.ts";
            else tn = tn + ".ts";

            if (string.IsNullOrEmpty(ns)) return Path.Combine(_targetDirectory, tn);
            if (!string.IsNullOrEmpty(_rootNamespace))
            {
                ns = ns.Replace(_rootNamespace, string.Empty);
            }
            ns = ns.Trim('.').Replace('.', '\\');

            var pth =
                Path.Combine(
                    !string.IsNullOrEmpty(ns) ? Path.Combine(_targetDirectory, ns) : _targetDirectory,
                    tn);

            return pth;
        }

        public string GetRelativePathForType(Type typeToReference, Type currentlyExportingType)
        {
            var currentFile = GetPathForType(currentlyExportingType);
            var desiredFile = GetPathForType(typeToReference);
            if (currentFile == desiredFile) return String.Empty;

            var desiredFileName = Path.GetFileName(desiredFile);

            var relPath = GetRelativeNamespacePath(Path.GetDirectoryName(currentFile),
                Path.GetDirectoryName(desiredFile));

            relPath = Path.Combine(relPath, desiredFileName);
            relPath = relPath.Replace('\\', '/');
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


        private static Type GetOverridenType(MemberInfo info)
        {
            var attr = ConfigurationRepository.Instance.ForMember(info);
            if (attr != null && attr.StrongType != null) return attr.StrongType;
            if (info is PropertyInfo) return ((PropertyInfo)info).PropertyType;
            if (info is FieldInfo) return ((FieldInfo)info).FieldType;
            if (info is MethodInfo) return ((MethodInfo)info).ReturnType;
            return null;
        }

        internal static HashSet<Type> InspectReferences(Type element, HashSet<Type> alltypes)
        {
            var references = new HashSet<Type>();
            if (element.IsEnum) return references;

            foreach (var fi in element.GetExportedFields())
                InspectTypeReferences(GetOverridenType(fi), alltypes, references);
            foreach (var pi in element.GetExportedProperties())
                InspectTypeReferences(GetOverridenType(pi), alltypes, references);
            foreach (var mi in element.GetExportedMethods())
            {
                InspectTypeReferences(GetOverridenType(mi), alltypes, references);

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

        private static void InspectTypeReferences(Type argument, HashSet<Type> alltypes,
            HashSet<Type> referenceContainer)
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

    internal class InspectedReferences
    {
        public RtReference[] References { get; private set; }

        public RtImport[] Imports { get; private set; }

        public InspectedReferences(IEnumerable<RtReference> references = null, IEnumerable<RtImport> imports = null)
        {
            References = references == null ? new RtReference[0] : references.ToArray();
            Imports = imports == null ? new RtImport[0] : imports.ToArray();
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