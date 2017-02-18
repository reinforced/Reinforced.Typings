using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reinforced.Typings
{
    internal class ReferenceInspector
    {
        internal static string GenerateInspectedReferences(IFilesOperations fileOps, Type element,
            HashSet<Type> alltypes)
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
                        var path = attr.Type != null ? fileOps.GetRelativePathForType(attr.Type, element) : attr.RawPath;
                        if (!string.IsNullOrEmpty(path)) references.AddIfNotExists(path);
                    }
                }
            }
            foreach (var inspectedType in inspectedTypes)
            {
                if (inspectedType != element)
                {
                    var path = fileOps.GetRelativePathForType(inspectedType, element);
                    if (!string.IsNullOrEmpty(path)) references.AddIfNotExists(path);
                }
            }
            var sb = new StringBuilder();
            foreach (var reference in references)
            {
                if (!string.IsNullOrEmpty(reference))
                    sb.AppendLine(string.Format("/// <reference path=\"{0}\"/>", reference));
            }

            return sb.ToString().Trim();
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

    internal static class HashSetExtensions
    {
        internal static void AddIfNotExists<T>(this HashSet<T> hashSet, T val)
        {
            if (hashSet.Contains(val)) return;
            hashSet.Add(val);
        }
    }
}