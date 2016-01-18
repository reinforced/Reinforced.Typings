using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    public abstract class ClassAndInterfaceGeneratorBase<TNode> : TsCodeGeneratorBase<Type,TNode> where TNode : RtNode, new()
    {
        /// <summary>
        ///     Exports entire class to specified writer
        /// </summary>
        /// <param name="result">Exporting result</param>
        /// <param name="type">Exporting class type</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void Export(ITypeMember result, Type type, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            result.Name = type.GetName();

            var doc = Context.Documentation.GetDocumentationMember(type);
            if (doc != null)
            {
                RtJsdocNode docNode = new RtJsdocNode();
                if (doc.HasSummary()) docNode.Description = doc.Summary.Text;
                result.Documentation = docNode;
            }
            result.NeedsExports = !string.IsNullOrEmpty(type.GetNamespace());

            var ifaces = type.GetInterfaces();
            var bs = type.BaseType;
            var baseClassIsExportedAsInterface = false;
            if (bs != null && bs != typeof(object))
            {
                if (ConfigurationRepository.Instance.ForType<TsDeclarationAttributeBase>(bs) != null)
                {
                    if (bs.IsExportingAsInterface()) baseClassIsExportedAsInterface = true;
                    else
                    {
                        ((RtClass)result).Extendee = resolver.ResolveTypeName(bs);
                    }
                }
            }
            var implementees =
                ifaces.Where(c => ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(c) != null)
                    .Select(resolver.ResolveTypeName).ToList();
            if (baseClassIsExportedAsInterface)
            {
                implementees.Add(resolver.ResolveTypeName(bs));
            }
            result.Implementees.AddRange(implementees.OfType<RtSimpleTypeName>());
            ExportMembers(type, resolver, result, swtch);
        }

        /// <summary>
        ///     Exports all type members sequentially
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="typeMember">Placeholder for members</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMembers(Type element, TypeResolver resolver, ITypeMember typeMember,
            IAutoexportSwitchAttribute swtch)
        {
            ExportFields(typeMember, element, resolver, swtch);
            ExportProperties(typeMember, element, resolver, swtch);
            ExportMethods(typeMember, element, resolver, swtch);
            ExportConstructors(typeMember, element, resolver, swtch);
            HandleBaseClassExportingAsInterface(typeMember, element, resolver, swtch);
        }

        /// <summary>
        ///     Here you can customize what to export when base class is class but exporting as interface
        /// </summary>
        /// <param name="sw">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void HandleBaseClassExportingAsInterface(ITypeMember sw, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            if (element.BaseType != null)
            {
                if (
                    element.BaseType.IsExportingAsInterface() && !element.IsExportingAsInterface())
                {
                    // well.. bad but often case. 
                    // Here we should export members also for base class
                    // we do not export methods - just properties and fields
                    // but still. It is better thatn nothing

                    if (sw.Documentation == null) sw.Documentation = new RtJsdocNode();
                    sw.Documentation.TagToDescription.Add(new Tuple<DocTag, string>(DocTag.Todo,
                        string.Format("Automatically implemented from {0}", resolver.ResolveTypeName(element.BaseType))));

                    var basExSwtch = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(element.BaseType);
                    Context.SpecialCase = true;
                    ExportFields(sw, element.BaseType, resolver, basExSwtch);
                    ExportMethods(sw, element.BaseType, resolver, basExSwtch);
                    Context.SpecialCase = false;
                }
            }
        }

        /// <summary>
        ///     Exports type fields
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportFields(ITypeMember typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, typeMember, element.GetExportedFields());
        }

        /// <summary>
        ///     Exports type properties
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportProperties(ITypeMember typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, typeMember, element.GetExportedProperties());
        }

        /// <summary>
        ///     Exports type methods
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMethods(ITypeMember typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, typeMember, element.GetExportedMethods());
        }

        /// <summary>
        ///     Exports type constructors
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportConstructors(ITypeMember typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            if (swtch.AutoExportConstructors)
            {
                if (!element.IsExportingAsInterface()) // constructors are not allowed on interfaces
                {
                    var constructors =
                        element.GetConstructors(TypeExtensions.MembersFlags)
                            .Where(c => TypeExtensions.TypeScriptMemberSearchPredicate(c));
                    GenerateMembers(element, resolver, typeMember, constructors);
                }
            }
        }

        /// <summary>
        ///     Exports list of type members
        /// </summary>
        /// <typeparam name="T">Type member type</typeparam>
        /// <param name="element">Exporting class</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="typeMember">Output writer</param>
        /// <param name="members">Type members to export</param>
        protected virtual void GenerateMembers<T>(Type element, TypeResolver resolver, ITypeMember typeMember,
            IEnumerable<T> members) where T : MemberInfo
        {
            foreach (var m in members)
            {
                var generator = resolver.GeneratorFor<T>(m, Context);
                var member = generator.Generate(m, resolver);
                typeMember.Members.Add(member);
            }
        }
    }
}
