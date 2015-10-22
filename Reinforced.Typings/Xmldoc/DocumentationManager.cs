using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
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

        internal DocumentationManager(string docFilePath)
        {
            CacheDocumentation(docFilePath);
        }

        internal DocumentationManager(string[] docFilePath)
        {
            foreach (var s in docFilePath)
            {
                CacheDocumentation(s);
            }
        }

        internal void CacheDocumentation(string docFilePath)
        {
            if (string.IsNullOrEmpty(docFilePath)) return;
            if (!File.Exists(docFilePath)) return;
            try
            {
                var ser = new XmlSerializer(typeof (Documentation));
                Documentation documentation;
                using (var fs = File.OpenRead(docFilePath))
                {
                    documentation = (Documentation) ser.Deserialize(fs);
                }
                foreach (var documentationMember in documentation.Members)
                {
                    _documentationCache[documentationMember.Name] = documentationMember;
                }
                _isDocumentationExists = true;
            }
            catch (Exception)
            {
                _isDocumentationExists = false;
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

            if (parameterType.IsGenericType)
            {
                var gen = parameterType.GetGenericTypeDefinition();
                var name = gen.FullName;
                var quote = name.IndexOf('`');
                name = name.Substring(0, quote);
                var genericParams = parameterType.GetGenericArguments()
                    .Select(c => GetDocFriendlyParameterName(c, typeGenericsDict, methodGenericArgsDict)).ToArray();
                name = string.Format("{0}{{{1}}}", name, string.Join(",", genericParams));
                return name;
            }
            return parameterType.FullName.Trim('&');
        }

        private string GetIdentifierForMethod(MethodBase method, string name)
        {
            var isCtor = name == "#ctor";
            var sb = new StringBuilder(string.Format("M:{0}.{1}", method.DeclaringType.FullName, name));
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
                    method.DeclaringType.GetGenericArguments()
                        .Select((a, i) => new {a, i})
                        .ToDictionary(c => c.a, c => c.i); // type -> `0, type -> `1


                var methodGenericArgsDict = isCtor
                    ? new Dictionary<Type, int>()
                    : method.GetGenericArguments()
                        .Select((a, i) => new {a, i})
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
            if (member is MethodInfo) return GetIdentifierForMethod((MethodBase) member, member.Name);
            var id = string.Format("{0}:{1}.{2}", GetPrefix(member.MemberType), member.DeclaringType.FullName,
                member.Name);
            return id;
        }

        private string GetIdentifierForType(Type type)
        {
            return string.Format("T:{0}", type.FullName);
        }

        private string GetIdentifierForConstructor(ConstructorInfo constructor)
        {
            return GetIdentifierForMethod(constructor, "#ctor");
        }

        /// <summary>
        ///     Outputs documentation for class member
        /// </summary>
        /// <param name="member">Class member</param>
        /// <param name="sw">Text writer</param>
        public void WriteDocumentation(MemberInfo member, WriterWrapper sw)
        {
            if (member == null) return;
            if (!_isDocumentationExists) return;
            var id = GetIdentifierForMember(member);
            if (!_documentationCache.ContainsKey(id)) return;
            var info = member as MethodInfo;
            if (info != null)
            {
                WriteDocumentation(info, sw);
                return;
            }
            var doc = _documentationCache[id];
            if (!doc.HasSummary()) return;
            Begin(sw);
            Summary(sw, doc.Summary.Text);
            End(sw);
        }

        /// <summary>
        ///     Outputs documentation for method
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="sw">Text writer</param>
        public void WriteDocumentation(MethodInfo method, WriterWrapper sw)
        {
            if (method == null) return;
            if (!_isDocumentationExists) return;
            var id = GetIdentifierForMethod(method, method.Name);
            if (!_documentationCache.ContainsKey(id)) return;
            var doc = _documentationCache[id];

            if ((!doc.HasSummary()) && (!doc.HasParameters()) && (!doc.HasReturns())) return;

            Begin(sw);
            if (doc.HasSummary()) Summary(sw, doc.Summary.Text);
            if (doc.HasParameters())
            {
                if (doc.HasSummary()) Line(sw);
                WriteParametersDoc(method.GetParameters(), doc, sw);
            }
            if (doc.HasReturns())
            {
                if (doc.HasSummary() || doc.HasParameters()) Line(sw);
                Line(sw, string.Format("@returns {0}", doc.Returns.Text));
            }
            End(sw);
        }

        /// <summary>
        ///     Outputs documentation for constructor
        /// </summary>
        /// <param name="constructor">Constructor</param>
        /// <param name="sw">Text writer</param>
        public void WriteDocumentation(ConstructorInfo constructor, WriterWrapper sw)
        {
            if (constructor == null) return;
            if (!_isDocumentationExists) return;
            var id = GetIdentifierForConstructor(constructor);
            if (!_documentationCache.ContainsKey(id)) return;
            var doc = _documentationCache[id];
            if ((!doc.HasSummary()) && (!doc.HasParameters())) return;

            Begin(sw);
            if (doc.HasSummary())
            {
                Summary(sw, doc.Summary.Text);
                Line(sw);
            }
            Line(sw, "@constructor");
            if (doc.HasParameters())
            {
                WriteParametersDoc(constructor.GetParameters(), doc, sw);
            }
            End(sw);
        }

        /// <summary>
        ///     Outputs documentation for type
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="sw">Text writer</param>
        public void WriteDocumentation(Type type, WriterWrapper sw)
        {
            if (type == null) return;
            if (!_isDocumentationExists) return;
            var id = GetIdentifierForType(type);
            if (!_documentationCache.ContainsKey(id)) return;
            var typeDoc = _documentationCache[id];
            if (!typeDoc.HasSummary()) return;

            Begin(sw);
            Summary(sw, typeDoc.Summary.Text);
            End(sw);
        }

        private void WriteParametersDoc(ParameterInfo[] parameters, DocumentationMember docMember, WriterWrapper sw)
        {
            foreach (var parameterInfo in parameters)
            {
                var doc = docMember.Parameters.SingleOrDefault(c => c.Name == parameterInfo.Name);
                if (doc == null) continue;
                var name = parameterInfo.GetName();
                Line(sw, string.Format("@param {0} {1}", name, doc.Description));
            }
        }

        private void Begin(WriterWrapper sw)
        {
            sw.WriteLine("/**");
        }

        private void Summary(WriterWrapper sw, string summary)
        {
            if (string.IsNullOrEmpty(summary)) return;
            var summaryLines = summary.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var summaryLine in summaryLines)
            {
                Line(sw, summaryLine);
            }
        }

        private void Line(WriterWrapper sw, string line = null)
        {
            if (string.IsNullOrEmpty(line)) sw.WriteLine("*");
            else sw.WriteLine("* {0}", line);
        }

        /// <summary>
        ///     Writes output comment with automatic multiline division
        /// </summary>
        /// <param name="sw">Output writer</param>
        /// <param name="comment">Comment (multiline allowed)</param>
        public void WriteComment(WriterWrapper sw, string comment)
        {
            sw.Br();
            sw.Indent();
            var lines = comment.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 1)
            {
                sw.WriteLine("// {0} ", lines[0]);
            }
            else
            {
                Begin(sw);
                foreach (var line in lines)
                {
                    Line(sw, line);
                }
                End(sw);
                sw.Br();
            }
        }

        private void End(WriterWrapper sw)
        {
            sw.WriteLine("*/");
        }
    }
}