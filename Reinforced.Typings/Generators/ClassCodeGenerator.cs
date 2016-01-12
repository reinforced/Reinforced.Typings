﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for CLR type (class)
    /// </summary>
    public class ClassCodeGenerator : ITsCodeGenerator<Type>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(element);
            if (tc == null) throw new ArgumentException("TsClassAttribute is not present", "element");
            Export("class", element, resolver, sw, tc);
        }

        /// <summary>
        ///     Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }

        /// <summary>
        ///     Exports entire class to specified writer
        /// </summary>
        /// <param name="declType">
        ///     Declaration type. Used in "export $gt;class&lt; ... " line. This parameter allows switch it to
        ///     "interface"
        /// </param>
        /// <param name="type">Exporting class type</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void Export(string declType, Type type, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            var name = GetName(type);

            Settings.Documentation.WriteDocumentation(type, sw);
            sw.Indent();


            sw.Write(Settings.GetDeclarationFormat(type), declType);
            sw.Write(name);

            var ifaces = type.GetInterfaces();
            var bs = type.BaseType;
            var baseClassIsExportedAsInterface = false;
            if (bs != null && bs != typeof (object))
            {
                if (ConfigurationRepository.Instance.ForType<TsDeclarationAttributeBase>(bs) != null)
                {
                    if (bs.IsExportingAsInterface()) baseClassIsExportedAsInterface = true;
                    else
                    {
                        sw.Write(" extends {0} ", resolver.ResolveTypeName(bs));
                    }
                }
            }
            var ifacesStrings =
                ifaces.Where(c => ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(c) != null)
                    .Select(resolver.ResolveTypeName).ToList();
            if (baseClassIsExportedAsInterface)
            {
                ifacesStrings.Add(resolver.ResolveTypeName(bs));
            }
            if (ifacesStrings.Any())
            {
                var implemets = string.Join(", ", ifacesStrings);
                if (type.IsExportingAsInterface()) sw.Write(" extends {0}", implemets);
                else sw.Write(" implements {0}", implemets);
            }

            sw.Write(" {{");
            sw.WriteLine();
            ExportMembers(type, resolver, sw, swtch);
            sw.WriteLine("}");
        }

        /// <summary>
        ///     Exports all type members sequentially
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMembers(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            ExportFields(element, resolver, sw, swtch);
            ExportProperties(element, resolver, sw, swtch);
            ExportMethods(element, resolver, sw, swtch);
            ExportConstructors(element, resolver, sw, swtch);
            HandleBaseClassExportingAsInterface(element, resolver, sw, swtch);
        }

        /// <summary>
        ///     Here you can customize what to export when base class is class but exporting as interface
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void HandleBaseClassExportingAsInterface(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
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

                    Settings.Documentation.WriteComment(sw,
                        string.Format("Automatically implemented from {0}", resolver.ResolveTypeName(element.BaseType)));
                    var basExSwtch = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(element.BaseType);
                    Settings.SpecialCase = true;
                    ExportFields(element.BaseType, resolver, sw, basExSwtch);
                    ExportMethods(element.BaseType, resolver, sw, basExSwtch);
                    Settings.SpecialCase = false;
                }
            }
        }

        /// <summary>
        ///     Exports type fields
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportFields(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, sw, element.GetExportedFields());
        }

        /// <summary>
        ///     Exports type properties
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportProperties(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, sw, element.GetExportedProperties());
        }

        /// <summary>
        ///     Exports type methods
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMethods(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, sw, element.GetExportedMethods());
        }

        /// <summary>
        ///     Exports type constructors
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportConstructors(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {
            if (swtch.AutoExportConstructors)
            {
                if (!element.IsExportingAsInterface()) // constructors are not allowed on interfaces
                {
                    var constructors =
                        element.GetConstructors(TypeExtensions.MembersFlags)
                            .Where(c => TypeExtensions.TypeScriptMemberSearchPredicate(c));
                    GenerateMembers(element, resolver, sw, constructors);
                }
            }
        }

        /// <summary>
        ///     Exports list of type members
        /// </summary>
        /// <typeparam name="T">Type member type</typeparam>
        /// <param name="element">Exporting class</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="members">Type members to export</param>
        protected virtual void GenerateMembers<T>(Type element, TypeResolver resolver, WriterWrapper sw,
            IEnumerable<T> members) where T : MemberInfo
        {
            foreach (var m in members)
            {
                var generator = resolver.GeneratorFor(m, Settings);
                generator.Generate(m, resolver, sw);
            }
        }

        /// <summary>
        ///     Gets resulting typescript type name of exporting type
        /// </summary>
        /// <param name="element">Exporting class</param>
        /// <returns>Resulting ts type name</returns>
        protected virtual string GetName(Type element)
        {
            return element.GetName();
        }
    }
}