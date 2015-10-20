using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    internal static class ReferenceInspector
    {
        internal static string GenerateInspectedReferences(this FilesOperations fileOps, Type element,
            HashSet<Type> alltypes, string currentNamespace)
        {
            var inspectedTypes = InspectReferences(element, alltypes);
            StringBuilder sb = new StringBuilder();
            var types = ConfigurationRepository.Instance.ReferencesForType(element);
            if (types != null)
            {
                foreach (var attr in types)
                {
                    var path = attr.Type != null ? fileOps.GetRelativePathForType(attr.Type, currentNamespace) : attr.RawPath;
                    sb.AppendLine(String.Format("/// <reference path=\"{0}\"/>", path));
                }
            }
            foreach (var inspectedType in inspectedTypes)
            {
                sb.AppendLine(String.Format("/// <reference path=\"{0}\"/>", fileOps.GetRelativePathForType(inspectedType, currentNamespace)));
            }

            return sb.ToString();
        }

        internal static HashSet<Type> InspectReferences(Type element, HashSet<Type> alltypes)
        {
            HashSet<Type> references = new HashSet<Type>();
            IAutoexportSwitchAttribute swtch = ConfigurationRepository.Instance.ForType<TsClassAttribute>(element) ??
                                               (IAutoexportSwitchAttribute)ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(element);

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            Func<MemberInfo, bool> predicate = c => ConfigurationRepository.Instance.IsIgnored(c);

            var fields = element.GetFields(flags).Where(predicate).OfType<FieldInfo>();

            if (!swtch.AutoExportFields) fields = fields.Where(c => ConfigurationRepository.Instance.ForMember(c) != null);

            foreach (var fi in fields) InspectArgumentReferences(fi.FieldType, alltypes, references);

            var properties = element.GetProperties(flags).Where(predicate).OfType<PropertyInfo>();
            if (!swtch.AutoExportProperties) properties = properties.Where(c => ConfigurationRepository.Instance.ForMember(c) != null);

            foreach (var pi in properties) InspectArgumentReferences(pi.PropertyType, alltypes, references);

            var methods = element.GetMethods(flags).Where(c => predicate(c) && !c.IsSpecialName);
            if (!swtch.AutoExportMethods) methods = methods.Where(c => ConfigurationRepository.Instance.ForMember(c) != null);

            foreach (var mi in methods)
            {
                InspectArgumentReferences(mi.ReturnType, alltypes, references);
                foreach (var parameterInfo in mi.GetParameters())
                    InspectArgumentReferences(parameterInfo.ParameterType, alltypes, references);
            }

            return references;
        }

        private static void InspectArgumentReferences(Type argument, HashSet<Type> alltypes, HashSet<Type> referenceContainer)
        {
            if (alltypes.Contains(argument)) referenceContainer.AddIfNotExists(argument);
            if (argument.IsGenericType)
            {
                var args = argument.GetGenericArguments();
                foreach (var type in args)
                {
                    if (alltypes.Contains(type)) referenceContainer.AddIfNotExists(type);
                }
            }
        }

        private static void AddIfNotExists<T>(this HashSet<T> hashSet, T val)
        {
            if (hashSet.Contains(val)) return;
            hashSet.Add(val);
        }

    }
}
