using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    /// <summary>
    /// Class that holds information of all exported types' parameters and helper methods for it
    /// </summary>
    public class ProjectBlueprint
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


        internal IEnumerable<Type> BlueprintedTypes
        {
            get { return _blueprints.Keys; }
        }

        /// <summary>
        /// Returns blueprint for type. Never returns null
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Type blueprint</returns>
        public TypeBlueprint Blueprint(Type t)
        {
            if (t == null) return null;
            return _blueprints.GetOrCreate(t, () => new TypeBlueprint(t));
        }

        /// <summary>
        /// Returns blueprint for type.
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="create">True to create blueprint if it does not exist</param>
        /// <returns>Type blueprint</returns>
        public TypeBlueprint Blueprint(Type t, bool create)
        {
            if (t == null) return null;
            if (create) return _blueprints.GetOrCreate(t, () => new TypeBlueprint(t));
            if (!_blueprints.ContainsKey(t)) return null;
            return _blueprints[t];
        }

        internal void AddFileSeparationSettings(Type type)
        {
            var bp = Blueprint(type);
            if (bp.ThirdParty != null) return;

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

        internal void TrackTypeFile(Type t, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var typesPerFile = TypesInFiles.GetOrCreate(fileName);
            if (!typesPerFile.Contains(t)) typesPerFile.Add(t);
            PathesToFiles[t] = fileName;
        }

        internal string GetPathForFile(Type t)
        {
            if (PathesToFiles.ContainsKey(t)) return PathesToFiles[t];
            return null;
        }

        /// <summary>
        /// Dictionary holds pathes to files
        /// </summary>
        public Dictionary<Type, string> PathesToFiles { get; private set; }

        /// <summary>
        /// Dictionary that holds types within each file
        /// </summary>
        public Dictionary<string, List<Type>> TypesInFiles { get; private set; }

        /// <summary>
        /// Additional pathes to look up documentation for
        /// </summary>
        public List<string> AdditionalDocumentationPathes { get; private set; }

        /// <summary>
        /// References that will be added to each exported file
        /// </summary>
        public List<RtReference> References { get; private set; }

        /// <summary>
        /// Imports that will be added to each exported file
        /// </summary>
        public List<RtImport> Imports { get; private set; }


        internal Dictionary<Type, RtTypeName> GlobalSubstitutions { get; private set; }
        internal Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GlobalGenericSubstitutions { get; private set; }

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



    }
}
