using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    public abstract class ClassAndInterfaceGeneratorBase<TNode> : TsCodeGeneratorBase<Type, TNode> where TNode : RtNode, new()
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

            var bp = Context.Project.Blueprint(type);
            result.Name = bp.GetName();
            result.Order = bp.GetOrder();

            var doc = Context.Documentation.GetDocumentationMember(type);
            if (doc != null)
            {
                RtJsdocNode docNode = new RtJsdocNode();
                if (doc.HasInheritDoc()) docNode.AddTag(DocTag.Inheritdoc);
                if (doc.HasSummary()) docNode.Description = doc.Summary.Text;
                result.Documentation = docNode;
            }

            var materializedGenericParameters = type._GetGenericArguments()
                .Where(c => c.GetCustomAttribute<TsGenericAttribute>() != null)
                .ToDictionary(c => c.Name, resolver.ResolveTypeName);

            if (materializedGenericParameters.Count == 0) materializedGenericParameters = null;

            if (!bp.IsFlatten())
            {
                var baseType = type._BaseType();
                var implementees = ExtractImplementees(type, resolver, materializedGenericParameters).ToList();

                if (baseType != null && baseType != typeof(object))
                {
                    bool baseAsInterface = false;
                    RtTypeName inferredBaseType = null;
                    if (baseType._IsGenericType())
                    {
                        var genericBase = baseType.GetGenericTypeDefinition();
                        var genericBaseBp = Context.Project.Blueprint(genericBase);
                        if (genericBaseBp.TypeAttribute != null || genericBaseBp.ThirdParty != null)
                        {
                            inferredBaseType = resolver.ResolveTypeName(baseType,
                                MergeMaterializedGenerics(baseType, resolver, materializedGenericParameters));
                            baseAsInterface = Context.Project.Blueprint(genericBase).IsExportingAsInterface();
                        }
                    }
                    if (inferredBaseType == null || !baseType._IsGenericType())
                    {
                        var bsBp = Context.Project.Blueprint(baseType);
                        if (bsBp.TypeAttribute != null || bsBp.ThirdParty != null)
                        {
                            baseAsInterface = Context.Project.Blueprint(baseType).IsExportingAsInterface();
                            inferredBaseType = resolver.ResolveTypeName(baseType,
                                MergeMaterializedGenerics(baseType, resolver, materializedGenericParameters));
                        }
                    }

                    if (inferredBaseType != null)
                    {
                        if (baseAsInterface || result is RtInterface)
                        {
                            implementees.Add(inferredBaseType);
                        }
                        else
                        {
                            ((RtClass)result).Extendee = inferredBaseType;
                        }
                    }
                }

                result.Implementees.AddRange(implementees.OfType<RtSimpleTypeName>());
            }

            ExportMembers(type, resolver, result, swtch);
        }

        private Dictionary<string, RtTypeName> MergeMaterializedGenerics(Type t, TypeResolver resovler, Dictionary<string, RtTypeName> existing)
        {
            if (!t._IsGenericType()) return existing;
            var args = t._GetGenericArguments();
            if (args.All(c => c.IsGenericParameter)) return existing;
            var genDef = t.GetGenericTypeDefinition()._GetGenericArguments();
            Dictionary<string, RtTypeName> result = new Dictionary<string, RtTypeName>();
            if (existing != null)
            {
                foreach (var rtTypeName in existing)
                {
                    result[rtTypeName.Key] = rtTypeName.Value;
                }
            }
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].IsGenericParameter)
                {
                    result[genDef[i].Name] = resovler.ResolveTypeName(args[i],
                        MergeMaterializedGenerics(args[i], resovler, existing));
                }
                else
                {
                    if (args[i].Name != genDef[i].Name)
                    {
                        result[genDef[i].Name] = new RtSimpleTypeName(args[i].Name);
                    }
                }

            }
            if (result.Count == 0) return null;
            return result;

        }

        private IEnumerable<RtTypeName> ExtractImplementees(Type type, TypeResolver resolver, Dictionary<string, RtTypeName> materializedGenericParameters)
        {
            var ifaces = type._GetInterfaces();
            foreach (var iface in ifaces)
            {

                RtTypeName inferredBaseType = null;
                if (iface._IsGenericType())
                {
                    var genericBase = iface.GetGenericTypeDefinition();
                    var genericBaseBp = Context.Project.Blueprint(genericBase);
                    if (genericBaseBp.TypeAttribute != null || genericBaseBp.ThirdParty != null)
                    {
                        inferredBaseType = resolver.ResolveTypeName(iface,
                            MergeMaterializedGenerics(iface, resolver, materializedGenericParameters));
                    }
                }
                if (inferredBaseType == null || !iface._IsGenericType())
                {
                    var bsBp = Context.Project.Blueprint(iface);
                    if (bsBp.TypeAttribute != null || bsBp.ThirdParty != null)
                    {
                        inferredBaseType = resolver.ResolveTypeName(iface,
                            MergeMaterializedGenerics(iface, resolver, materializedGenericParameters));
                    }
                }

                if (inferredBaseType != null) yield return inferredBaseType;
            }
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
            if (element._BaseType() != null)
            {
                var baseBp = Context.Project.Blueprint(element._BaseType(), false);
                var bp = Context.Project.Blueprint(element);

                //if we cannot determine what is being exported - then.. well. okay
                if (baseBp == null) return;
                if (bp == null) return;

                if ((!bp.IsExportingAsInterface()) && baseBp.IsExportingAsInterface())
                {
                    // well.. bad but often case. 
                    // Here we should export members also for base class
                    // we do not export methods - just properties and fields
                    // but still. It is better thatn nothing

                    if (sw.Documentation == null) sw.Documentation = new RtJsdocNode();
                    sw.Documentation.AddTag(DocTag.Todo,
                        string.Format("Automatically implemented from {0}", resolver.ResolveTypeName(element._BaseType())));

                    var baseBlueprint = Context.Project.Blueprint(element._BaseType());
                    var basExSwtch = baseBlueprint.Attr<TsInterfaceAttribute>();
                    Context.SpecialCase = true;
                    ExportFields(sw, element._BaseType(), resolver, basExSwtch);
                    ExportProperties(sw, element._BaseType(), resolver, basExSwtch);
                    ExportMethods(sw, element._BaseType(), resolver, basExSwtch);
                    Context.SpecialCase = false;
                    Context.Warnings.Add(ErrorMessages.RTW0005_BaseClassExportingAsInterface.Warn(element._BaseType().FullName, element.FullName));
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
            GenerateMembers(element, resolver, typeMember, Context.Project.Blueprint(element).GetExportedFields());
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
            GenerateMembers(element, resolver, typeMember, Context.Project.Blueprint(element).GetExportedProperties());
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
            GenerateMembers(element, resolver, typeMember, Context.Project.Blueprint(element).GetExportedMethods());
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
            var bp = Context.Project.Blueprint(element);
            if (swtch.AutoExportConstructors)
            {
                if (!bp.IsExportingAsInterface()) // constructors are not allowed on interfaces
                {
                    var constructors =
                        element._GetConstructors(TypeExtensions.MembersFlags)
                            .Where(c => (c.GetCustomAttribute<CompilerGeneratedAttribute>() == null) && !bp.IsIgnored(c));
                    if (!constructors.Any())
                        constructors =
                            element._GetConstructors(TypeExtensions.MembersFlags)
                                .Where(c => !bp.IsIgnored(c));
                    GenerateMembers(element, resolver, typeMember, constructors.Take(1));
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
                var generator = Context.Generators.GeneratorFor(m);
                var member = generator.Generate(m, resolver);
                if (member != null) typeMember.Members.Add(member);
            }
        }
    }
}
