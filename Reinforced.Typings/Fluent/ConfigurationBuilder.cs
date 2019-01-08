using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder
    /// </summary>
    public class ConfigurationBuilder
    {
       
        private readonly Dictionary<Type, TypeExportBuilder> _typeExportBuilders = new Dictionary<Type, TypeExportBuilder>();
        private readonly Dictionary<Type, ThirdPartyExportBuilder> _thirdPartyBuilders = new Dictionary<Type, ThirdPartyExportBuilder>();

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

        internal Dictionary<Type, TypeExportBuilder> TypeExportBuilders
        {
            get { return _typeExportBuilders; }
        }

        internal Dictionary<Type, ThirdPartyExportBuilder> ThirdPartyBuilders
        {
            get { return _thirdPartyBuilders; }
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

        internal TypeBlueprint GetCheckedBlueprint<TAttr>(Type type)
        {
            var bp = Context.Project.Blueprint(type);
            if (bp.TypeAttribute != null && !typeof(TAttr).IsAssignableFrom(bp.TypeAttribute.GetType()))
            {
                var name = typeof(TAttr).Name.Substring(2).Replace("Attribute", string.Empty).ToLower();

                ErrorMessages.RTE0017_FluentContradict.Throw(type, name);
            }

            if (bp.ThirdParty != null)
            {
                var name = typeof(TAttr).Name.Substring(2).Replace("Attribute", string.Empty).ToLower();
                ErrorMessages.RTE0018_FluentThirdParty.Throw(type, name);
            }
            return bp;
        }

        internal TypeBlueprint GetCheckedThirdPartyBlueprint(Type type)
        {
            var bp = Context.Project.Blueprint(type);

            if (bp.TypeAttribute != null)
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(type, "third party");
            }

            return bp;
        }
    }
}