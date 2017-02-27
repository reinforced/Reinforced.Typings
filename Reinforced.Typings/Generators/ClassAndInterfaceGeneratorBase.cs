using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Base code generator both for TypeScript class and interface
    /// </summary>
    /// <typeparam name="TNode">Resulting node type (RtClass or RtInterface)</typeparam>
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
            Context.Location.SetCurrentType(type);
            result.Name = type.GetName();
            result.Order = type.GetOrder();

            var doc = Context.Documentation.GetDocumentationMember(type);
            if (doc != null)
            {
                RtJsdocNode docNode = new RtJsdocNode();
                if (doc.HasSummary()) docNode.Description = doc.Summary.Text;
                result.Documentation = docNode;
            }
            
            var ifaces = type.GetInterfaces();
            var bs = type.BaseType;
            var baseClassIsExportedAsInterface = false;
            if (bs != null && bs != typeof(object))
            {
                TsDeclarationAttributeBase attr = null;
                bool baseAsInterface = false;
                if (bs.IsGenericType)
                {
                    var genericBase = bs.GetGenericTypeDefinition();
                    attr = ConfigurationRepository.Instance.ForType<TsDeclarationAttributeBase>(genericBase);
                    baseAsInterface = genericBase.IsExportingAsInterface();
                }
                else
                {
                    attr = ConfigurationRepository.Instance.ForType<TsDeclarationAttributeBase>(bs);
                    baseAsInterface = bs.IsExportingAsInterface();
                }
                
                if (attr != null)
                {
                    if (baseAsInterface) baseClassIsExportedAsInterface = true;
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
            Context.Location.ResetCurrentType();
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
            ExportConstructors(typeMember, element, resolver, swtch);
            ExportFields(typeMember, element, resolver, swtch);
            ExportProperties(typeMember, element, resolver, swtch);
            ExportMethods(typeMember, element, resolver, swtch);
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
                    ExportProperties(sw, element.BaseType, resolver, basExSwtch);
                    ExportMethods(sw, element.BaseType, resolver, basExSwtch);
                    Context.SpecialCase = false;
                    Context.Warnings.Add(ErrorMessages.RTW0005_BaseClassExportingAsInterface.Warn(element.BaseType.FullName,element.FullName));
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
                            .Where(c => ConfiguredTypesExtensions.TypeScriptMemberSearchPredicate(c));
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
                var generator = Context.Generators.GeneratorFor<T>(m, Context);
                var member = generator.Generate(m, resolver);
                typeMember.Members.Add(member);
            }
        }
    }
}
