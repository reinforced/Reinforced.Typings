using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for CLR type (class) 
    /// </summary>
    public class ClassCodeGenerator : ITsCodeGenerator<Type>
    {
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var tc = element.GetCustomAttribute<TsClassAttribute>(false);
            if (tc == null) throw new ArgumentException("TsClassAttribute is not present", "element");
            Export("class", element, resolver, sw, tc);
        }

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }

        /// <summary>
        /// Exports entire class to specified writer
        /// </summary>
        /// <param name="declType">Declaration type. Used in "export $gt;class&lt; ... " line. This parameter allows switch it to "interface"</param>
        /// <param name="type">Exporting class type</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void Export(string declType, Type type, TypeResolver resolver, WriterWrapper sw, IAutoexportSwitchAttribute swtch)
        {
            string name = type.GetName();

            Settings.Documentation.WriteDocumentation(type, sw);
            sw.Indent();


            sw.Write(Settings.GetDeclarationFormat(type), declType);
            sw.Write(name);

            var ifaces = type.GetInterfaces();
            var bs = type.BaseType;
            bool baseClassIsExportedAsInterface = false;
            if (bs != null && bs != typeof(object))
            {
                if (bs.GetCustomAttribute<TsAttributeBase>(false) != null)
                {
                    if (bs.IsExportingAsInterface()) baseClassIsExportedAsInterface = true;
                    else
                    {
                        sw.Write(" extends {0} ", resolver.ResolveTypeName(bs));
                    }
                }
            }
            var ifacesStrings = ifaces.Where(c => c.GetCustomAttribute<TsInterfaceAttribute>(false) != null).Select(resolver.ResolveTypeName).ToList();
            if (baseClassIsExportedAsInterface)
            {
                ifacesStrings.Add(resolver.ResolveTypeName(bs));
            }
            if (ifacesStrings.Any())
            {
                string implemets = String.Join(", ", ifacesStrings);
                sw.Write(" implements {0}", implemets);
            }

            sw.Write(" {{");
            sw.WriteLine();
            ExportMembers(type, resolver, sw, swtch);
            sw.WriteLine("}");
        }

        private const BindingFlags MembersFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        private readonly Func<MemberInfo, bool> _memberPredicate = c => c.GetCustomAttribute<TsIgnoreAttribute>(false) == null && c.GetCustomAttribute<CompilerGeneratedAttribute>() == null;

        /// <summary>
        /// Exports all type members sequentially
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMembers(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitchAttribute swtch)
        {

            ExportFieldsAndProperties(element, resolver, sw, swtch);

            ExportMethodsAndConstructors(element, resolver, sw, swtch);

            if (element.BaseType != null)
            {
                if (
                    element.BaseType.IsExportingAsInterface() && !element.IsExportingAsInterface())
                {
                    // well.. bad but often case. 
                    // Here we should export members also for base class
                    // we do not export methods - just properties and fields
                    // but still. It is better thatn nothing

                    Settings.Documentation.WriteComment(sw, String.Format("Automatically implemented from {0}", resolver.ResolveTypeName(element.BaseType)));
                    var basExSwtch = element.BaseType.GetCustomAttribute<TsInterfaceAttribute>();
                    Settings.SpecialCase = true;
                    ExportFieldsAndProperties(element.BaseType, resolver, sw, basExSwtch);
                    ExportMethodsAndConstructors(element.BaseType, resolver, sw, basExSwtch);
                    Settings.SpecialCase = false;
                }
            }
        }

        private void ExportFieldsAndProperties(Type element, TypeResolver resolver, WriterWrapper sw, IAutoexportSwitchAttribute swtch)
        {
            var fields = element.GetFields(MembersFlags).Where(_memberPredicate).OfType<FieldInfo>();
            if (!swtch.AutoExportFields)
            {
                fields = fields.Where(c => c.GetCustomAttribute<TsPropertyAttribute>(false) != null);
            }
            GenerateMembers(element, resolver, sw, fields);

            var properties = element.GetProperties(MembersFlags).Where(_memberPredicate).OfType<PropertyInfo>();
            if (!swtch.AutoExportProperties)
            {
                properties = properties.Where(c => c.GetCustomAttribute<TsPropertyAttribute>(false) != null);
            }
            GenerateMembers(element, resolver, sw, properties);
        }

        private void ExportMethodsAndConstructors(Type element, TypeResolver resolver, WriterWrapper sw, IAutoexportSwitchAttribute swtch)
        {
            var methods = element.GetMethods(MembersFlags).Where(c => _memberPredicate(c) && !c.IsSpecialName);
            if (!swtch.AutoExportMethods)
            {
                methods = methods.Where(c => c.GetCustomAttribute<TsFunctionAttribute>(false) != null);
            }
            GenerateMembers(element, resolver, sw, methods);

            if (!element.IsExportingAsInterface()) // constructors are not allowed on interfaces
            {
                var constructors = element.GetConstructors(MembersFlags).Where(c => _memberPredicate(c));
                GenerateMembers(element, resolver, sw, constructors);
            }

        }


        /// <summary>
        /// Exports list of type members
        /// </summary>
        /// <typeparam name="T">Type member type</typeparam>
        /// <param name="element">Exporting class</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="members">Type members to export</param>
        protected virtual void GenerateMembers<T>(Type element, TypeResolver resolver, WriterWrapper sw, IEnumerable<T> members) where T : MemberInfo
        {

            foreach (var m in members)
            {
                var generator = resolver.GeneratorFor(m, Settings);
                generator.Generate(m, resolver, sw);
            }
        }
    }
}
