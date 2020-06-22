using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Xmldoc.Model;

// ReSharper disable PossibleNullReferenceException

namespace Reinforced.Typings.Xmldoc
{
    /// <summary>
    ///     XMLDOC documentation manager
    /// </summary>
    public class DocumentationManager
    {
        private readonly Dictionary<string, DocumentationMember> _documentationCache =
            new Dictionary<string, DocumentationMember>();

        private bool _isDocumentationExists;

        internal DocumentationManager(string docFilePath, List<RtWarning> warnings)
        {
            CacheDocumentation(docFilePath, warnings);
        }

        internal DocumentationManager(string[] docFilePath, List<RtWarning> warnings)
        {
            foreach (var s in docFilePath)
            {
                CacheDocumentation(s, warnings);
            }
        }

        internal void CacheDocumentation(string docFilePath, List<RtWarning> warnings)
        {
            if (string.IsNullOrEmpty(docFilePath))
            {
                return;
            }
            if (!File.Exists(docFilePath))
            {
                warnings.Add(ErrorMessages.RTW0002_DocumentationNotFound.Warn(docFilePath));
                return;
            }
            try
            {
                var ser = new XmlSerializer(typeof(Documentation));
                
                Documentation documentation;
                using (var fs = File.OpenRead(docFilePath))
                {
                    documentation = (Documentation)ser.Deserialize(fs);
                }
                foreach (var documentationMember in documentation.Members)
                {
                    _documentationCache[documentationMember.Name] = documentationMember;
                }
                _isDocumentationExists = true;
            }
            catch (Exception ex)
            {
                _isDocumentationExists = false;
                warnings.Add(ErrorMessages.RTW0006_DocumentationParseringError.Warn(docFilePath, ex.Message));
            }
        }

        private static string GetDocFriendlyParameterName(Type parameterType,
            Dictionary<Type, int> typeGenericsDict,
            Dictionary<Type, int> methodGenericArgsDict)
        {
            if (typeGenericsDict.ContainsKey(parameterType))
            {
                return ("`" + typeGenericsDict[parameterType]);
            }
            if (methodGenericArgsDict.ContainsKey(parameterType))
            {
                return ("``" + methodGenericArgsDict[parameterType]);
            }

            if (parameterType._IsGenericType())
            {
                var gen = parameterType.GetGenericTypeDefinition();
                var name = gen.FullName;
                var quote = name.IndexOf('`');
                name = name.Substring(0, quote);
                var genericParams = parameterType._GetGenericArguments()
                    .Select(c => GetDocFriendlyParameterName(c, typeGenericsDict, methodGenericArgsDict)).ToArray();
                name = string.Format("{0}{{{1}}}", name, string.Join(",", genericParams));
                return name;
            }
            return parameterType.FullName.Trim('&');
        }

        private string GetIdentifierForMethod(MethodBase method, string name)
        {
            var isCtor = name == "#ctor";
            var sb = new StringBuilder(string.Format("M:{0}.{1}", method.DeclaringType.FullName.Replace('+', '.'), name));
            if (!isCtor)
            {
                var cnt = method.GetGenericArguments().Length;
                if (cnt > 0) sb.AppendFormat("``{0}", cnt);
            }
            var prs = method.GetParameters();
            if (prs.Length > 0)
            {
                sb.Append('(');

                var typeGenericsDict =
                    method.DeclaringType._GetGenericArguments()
                        .Select((a, i) => new { a, i })
                        .ToDictionary(c => c.a, c => c.i); // type -> `0, type -> `1


                var methodGenericArgsDict = isCtor
                    ? new Dictionary<Type, int>()
                    : method.GetGenericArguments()
                        .Select((a, i) => new { a, i })
                        .ToDictionary(c => c.a, c => c.i); //type -> ``0, type -> ``1
                var names = new List<string>();
                foreach (var param in prs)
                {
                    var friendlyName = GetDocFriendlyParameterName(param.ParameterType, typeGenericsDict,
                        methodGenericArgsDict);
                    if (param.IsOut || param.ParameterType.IsByRef) friendlyName = friendlyName + "@";
                    names.Add(friendlyName);
                }
                sb.Append(string.Join(",", names));
                sb.Append(')');
            }
            return sb.ToString();
        }

        private string GetPrefix(MemberTypes mt)
        {
            switch (mt)
            {
                case MemberTypes.Property:
                    return "P";
                case MemberTypes.Field:
                    return "F";
                case MemberTypes.Method:
                    return "M";
                case MemberTypes.Event:
                    return "E";
            }
            return string.Empty;
        }

        private string GetIdentifierForMember(MemberInfo member)
        {

            if (member is MethodInfo) return GetIdentifierForMethod((MethodBase)member, member.Name);
            var path = member.DeclaringType.FullName;
            if (string.IsNullOrEmpty(path))
            {
                path = member.DeclaringType.Namespace + "." + member.DeclaringType.Name;
            }
            var id = string.Format("{0}:{1}.{2}", GetPrefix(member.MemberType),
                path.Replace('+', '.'),
                member.Name);
            return id;

        }

        private string GetIdentifierForType(Type type)
        {
            return string.Format("T:{0}", type.FullName.Replace('+', '.'));
        }

        private string GetIdentifierForConstructor(ConstructorInfo constructor)
        {
            return GetIdentifierForMethod(constructor, "#ctor");
        }

        /// <summary>
        ///     Returns documentation member for class member
        /// </summary>
        /// <param name="member">Class member</param>
        public DocumentationMember GetDocumentationMember(MemberInfo member)
        {
            if (member == null) return null;
            if (!_isDocumentationExists) return null;
            var id = GetIdentifierForMember(member);
            if (!_documentationCache.ContainsKey(id)) return null;
            var info = member as MethodInfo;
            if (info != null)
            {
                return GetDocumentationMember(info);
            }
            var doc = _documentationCache[id];
            if (!doc.HasInheritDoc() && !doc.HasSummary()) return null;
            return doc;
        }

        /// <summary>
        /// Returns documentation member for method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public DocumentationMember GetDocumentationMember(MethodInfo method)
        {
            if (method == null) return null;
            if (!_isDocumentationExists) return null;
            var id = GetIdentifierForMethod(method, method.Name);
            if (!_documentationCache.ContainsKey(id)) return null;
            var doc = _documentationCache[id];

            if (!doc.HasInheritDoc() && !doc.HasSummary() && !doc.HasParameters() && !doc.HasReturns()) return null;
            return doc;
        }

        /// <summary>
        ///     Returns documentation for constructor
        /// </summary>
        /// <param name="constructor">Constructor</param>
        public DocumentationMember GetDocumentationMember(ConstructorInfo constructor)
        {
            if (constructor == null) return null;
            if (!_isDocumentationExists) return null;
            var id = GetIdentifierForConstructor(constructor);
            if (!_documentationCache.ContainsKey(id)) return null;
            var doc = _documentationCache[id];
            if (!doc.HasInheritDoc() && !doc.HasSummary() && !doc.HasParameters()) return null;
            return doc;
        }


        /// <summary>
        ///     Returns documentation for type
        /// </summary>
        /// <param name="type">Type</param>
        public DocumentationMember GetDocumentationMember(Type type)
        {
            if (type == null) return null;
            if (!_isDocumentationExists) return null;
            var id = GetIdentifierForType(type);
            if (!_documentationCache.ContainsKey(id)) return null;
            var typeDoc = _documentationCache[id];
            if (!typeDoc.HasInheritDoc() && !typeDoc.HasSummary()) return null;
            return typeDoc;
        }
    }
}