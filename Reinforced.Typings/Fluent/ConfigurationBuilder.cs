using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder
    /// </summary>
    public class ConfigurationBuilder
    {
        private readonly Dictionary<Type, IEnumConfigurationBuidler> _enumConfigurationBuilders =
            new Dictionary<Type, IEnumConfigurationBuidler>();
        
        private readonly Dictionary<Type, ITypeConfigurationBuilder> _typeConfigurationBuilders =
            new Dictionary<Type, ITypeConfigurationBuilder>();

        /// <summary>
        /// Export context
        /// </summary>
        public ExportContext Context { get; private set; }
        internal ConfigurationBuilder(ExportContext context)
        {
            Context = context;
            GlobalBuilder = new GlobalConfigurationBuilder(context.Global);
        }

        internal List<string> AdditionalDocumentationPathes
        {
            get { return Context.Project.AdditionalDocumentationPathes; }
        }

        internal List<RtReference> References
        {
            get { return Context.Project.References; }
        }

        internal List<RtImport> Imports
        {
            get { return Context.Project.Imports; }
        }

        internal Dictionary<Type, ITypeConfigurationBuilder> TypeConfigurationBuilders
        {
            get { return _typeConfigurationBuilders; }
        }

        internal Dictionary<Type, IEnumConfigurationBuidler> EnumConfigurationBuilders
        {
            get { return _enumConfigurationBuilders; }
        }

        internal Dictionary<Type, RtTypeName> GlobalSubstitutions
        {
            get { return Context.Project.GlobalSubstitutions; }
        }

        internal Dictionary<Type, Func<Type, TypeResolver, RtTypeName>> GenericSubstitutions
        {
            get { return Context.Project.GlobalGenericSubstitutions; }
        }

        internal GlobalConfigurationBuilder GlobalBuilder { get; private set; }
    }
}