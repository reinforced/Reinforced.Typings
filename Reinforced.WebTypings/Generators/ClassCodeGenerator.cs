using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reinforced.WebTypings.Generators
{
    public class ClassCodeGenerator : ITsCodeGenerator<Type>
    {
        public virtual void Generate(Type element, TypeResolver resolver, WriterWrapper sw)
        {
            var tc = element.GetCustomAttribute<TsClassAttribute>();
            if (tc == null) throw new ArgumentException("TsClassAttribute is not present", "element");
            Export("class", element, resolver, sw, tc);
        }

        protected virtual void Export(string declType, Type element, TypeResolver resolver, WriterWrapper sw, IAutoexportSwitch swtch)
        {
            if (swtch.DefaultMethodCodeGenerator != null)
            {
                
            }

            string name = element.GetName();
            
            sw.Indent();
            sw.Write("export {0} ", declType);
            sw.Write(name);
            var ifaces = element.GetInterfaces();
            var bs = element.BaseType;
            if (bs != null && bs != typeof(object))
            {
                if (bs.GetCustomAttribute<TsAttributeBase>() != null)
                {
                    sw.Write(" extends {0} ", resolver.ResolveTypeName(bs));
                }
            }
            var ifacesStrings =  ifaces.Where(c => c.GetCustomAttribute<TsInterfaceAttribute>() != null).Select(resolver.ResolveTypeName).ToArray();
            if (ifacesStrings.Length>0)
            {
                string implemets = String.Join(", ",ifacesStrings);
                sw.Write("implements {0}",implemets);
            }

            sw.Write(" {{");
            sw.WriteLine();
            ExportMembers(element, resolver, sw, swtch);
            //sw.UnTab();
            sw.WriteLine("}");
        }

        protected virtual void ExportMembers(Type element, TypeResolver resolver, WriterWrapper sw,
            IAutoexportSwitch swtch)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            Func<MemberInfo, bool> predicate = c => c.GetCustomAttribute<TsIgnoreAttribute>() == null;

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
            
            var methods = element.GetMethods(flags).Where(c=>predicate(c)&&!c.IsSpecialName);
            if (!swtch.AutoExportMethods)
            {
                methods = methods.Where(c => c.GetCustomAttribute<TsFunctionAttribute>() != null);
            }
            GenerateMembers(element, resolver, sw, methods);
        }

        protected virtual void GenerateMembers<T>(Type element, TypeResolver resolver, WriterWrapper sw, IEnumerable<T> fields) where T : MemberInfo
        {

            foreach (var fieldInfo in fields)
            {
                var generator = resolver.GeneratorFor(fieldInfo);
                generator.Generate(fieldInfo, resolver, sw);
            }
        }
    }
}
