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
            var tc = element.GetCustomAttribute<TsClassAttribute>();
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

            sw.Tab();
            sw.Write(Settings.GetDeclarationFormat(type), declType);
            sw.Write(name);

            var ifaces = type.GetInterfaces();
            var bs = type.BaseType;
            if (bs != null && bs != typeof(object))
            {
                if (bs.GetCustomAttribute<TsAttributeBase>() != null)
                {
                    sw.Write(" extends {0} ", resolver.ResolveTypeName(bs));
                }
            }
            var ifacesStrings = ifaces.Where(c => c.GetCustomAttribute<TsInterfaceAttribute>() != null).Select(resolver.ResolveTypeName).ToArray();
            if (ifacesStrings.Length > 0)
            {
                string implemets = String.Join(", ", ifacesStrings);
                sw.Write("implements {0}", implemets);
            }

            sw.Write(" {{");
            sw.WriteLine();
            ExportMembers(type, resolver, sw, swtch);
            sw.WriteLine("}");
        }

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
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            Func<MemberInfo, bool> predicate = c => c.GetCustomAttribute<TsIgnoreAttribute>() == null && c.GetCustomAttribute<CompilerGeneratedAttribute>() == null;

            var fields = element.GetFields(flags).Where(predicate).OfType<FieldInfo>();
            if (!swtch.AutoExportFields)
            {
                fields = fields.Where(c => c.GetCustomAttribute<TsPropertyAttribute>() != null);
            }
            GenerateMembers(element, resolver, sw, fields);

            var properties = element.GetProperties(flags).Where(predicate).OfType<PropertyInfo>();
            if (!swtch.AutoExportProperties)
            {
                properties = properties.Where(c => c.GetCustomAttribute<TsPropertyAttribute>() != null);
            }
            GenerateMembers(element, resolver, sw, properties);

            var methods = element.GetMethods(flags).Where(c => predicate(c) && !c.IsSpecialName);
            if (!swtch.AutoExportMethods)
            {
                methods = methods.Where(c => c.GetCustomAttribute<TsFunctionAttribute>() != null);
            }
            GenerateMembers(element, resolver, sw, methods);

            if (!element.IsExportingAsInterface()) // constructors are not allowed on interfaces
            {
                var constructors = element.GetConstructors(flags).Where(c => predicate(c) && !c.IsSpecialName);
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

            foreach (var fieldInfo in members)
            {
                var generator = resolver.GeneratorFor(fieldInfo, Settings);
                generator.Generate(fieldInfo, resolver, sw);
            }
        }
    }
}
