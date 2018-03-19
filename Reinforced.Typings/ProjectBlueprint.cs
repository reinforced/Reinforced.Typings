using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings
{
    class ProjectBlueprint
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ProjectBlueprint()
        {
            PathesToFiles = new Dictionary<Type, string>();
            TypesInFiles = new Dictionary<string, List<Type>>();
            AdditionalDocumentationPathes = new List<string>();
            References = new List<RtReference>();
            Imports = new List<RtImport>();
            GlobalGenericSubstitutions = new Dictionary<Type, Func<Type, TypeResolver, RtTypeName>>();
            GlobalSubstitutions = new Dictionary<Type, RtTypeName>();
        }

        private readonly Dictionary<Type, TypeBlueprint> _blueprints = new Dictionary<Type, TypeBlueprint>();
        public bool IsIgnored(Type t)
        {
            return Blueprint(t).IsIgnored();
        }
        internal IEnumerable<Type> BlueprintedTypes
        {
            get { return _blueprints.Keys; }
        }

        internal TypeBlueprint Blueprint(Type t, bool create = true)
        {
            if (t == null) return null;
            if (create) return _blueprints.GetOrCreate(t, () => new TypeBlueprint(t));
            if (!_blueprints.ContainsKey(t)) return null;
            return _blueprints[t];
        }

        public void AddFileSeparationSettings(Type type)
        {
            var bp = Blueprint(type);

            if (!string.IsNullOrEmpty(bp.PathToFile))
            {
                TrackTypeFile(type, bp.PathToFile);
            }
            else
            {
                var fileAttr = type.GetCustomAttribute<TsFileAttribute>();

                if (fileAttr != null)
                {
                    TrackTypeFile(type, fileAttr.FileName);
                }
            }

        }

        public void TrackTypeFile(Type t, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var typesPerFile = TypesInFiles.GetOrCreate(fileName);
            if (!typesPerFile.Contains(t)) typesPerFile.Add(t);
            PathesToFiles[t] = fileName;
        }

        public string GetPathForFile(Type t)
        {
            if (PathesToFiles.ContainsKey(t)) return PathesToFiles[t];
            return null;
        }

        public Dictionary<Type, string> PathesToFiles { get; private set; }
        public Dictionary<string, List<Type>> TypesInFiles { get; private set; }
        public List<string> AdditionalDocumentationPathes { get; private set; }
        public List<RtReference> References { get; private set; }
        public List<RtImport> Imports { get; private set; }
        public Dictionary<Type, RtTypeName> GlobalSubstitutions { get; private set; }
        public Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GlobalGenericSubstitutions { get; private set; }

        /// <summary>
        /// Obtains substitution for type
        /// </summary>
        /// <param name="t">Type to find substitution for</param>
        /// <param name="tr">Type resolver instance</param>
        /// <returns>Substitution AST</returns>
        public RtTypeName Substitute(Type t, TypeResolver tr)
        {
            Type genericDef = t._IsGenericType() ? t.GetGenericTypeDefinition() : null;

            if (GlobalSubstitutions.ContainsKey(t)) return GlobalSubstitutions[t];
            if (genericDef != null)
            {
                if (GlobalGenericSubstitutions.ContainsKey(genericDef))
                {
                    var ts = GlobalGenericSubstitutions[genericDef](t, tr);
                    if (ts != null) return ts;
                }
            }
            return null;
        }

        internal void AddReferencesFromTypes(ExportedFile file, bool useImports)
        {
            foreach (var type in file.TypesToExport)
            {
                AddTypeSpecificReferences(file, type);
                if (useImports) AddTypeSpecificImports(file, type);
            }
        }

        private void AddTypeSpecificReferences(ExportedFile file, Type t)
        {
            var fluentRefs = Blueprint(t).References;
            var typeRefs = t.GetCustomAttributes<TsAddTypeReferenceAttribute>();

            foreach (var tsAddTypeReferenceAttribute in typeRefs)
            {
                if (tsAddTypeReferenceAttribute.Type != null)
                {
                    file.TypeResolver.ResolveTypeName(tsAddTypeReferenceAttribute.Type);
                }
                else
                {
                    file.References.AddReference(new RtReference() { Path = tsAddTypeReferenceAttribute.RawPath });
                }
            }

            if (fluentRefs != null)
            {
                foreach (var tsAddTypeReferenceAttribute in fluentRefs)
                {
                    if (tsAddTypeReferenceAttribute.Type != null)
                    {
                        file.TypeResolver.ResolveTypeName(tsAddTypeReferenceAttribute.Type);
                    }
                    else
                    {
                        file.References.AddReference(new RtReference() { Path = tsAddTypeReferenceAttribute.RawPath });
                    }
                }
            }
        }

        private void AddTypeSpecificImports(ExportedFile file, Type t)
        {
            var fluentImports = Blueprint(t).Imports;
            var typeImports = t.GetCustomAttributes<TsAddTypeImportAttribute>();
            foreach (var tsAddTypeImportAttribute in typeImports)
            {
                file.References.AddImport(new RtImport() { From = tsAddTypeImportAttribute.ImportSource, Target = tsAddTypeImportAttribute.ImportTarget, IsRequire = tsAddTypeImportAttribute.ImportRequire });
            }
            if (fluentImports != null)
            {
                foreach (var tsAddTypeImportAttribute in fluentImports)
                {
                    file.References.AddImport(new RtImport() { From = tsAddTypeImportAttribute.ImportSource, Target = tsAddTypeImportAttribute.ImportTarget, IsRequire = tsAddTypeImportAttribute.ImportRequire });
                }
            }
        }

    }
}
