using System;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default typescript code generator for method
    /// </summary>
    public class MethodCodeGenerator : ITsCodeGenerator<MethodInfo>
    {
        protected virtual Type GetReturnFunctionType(MethodInfo element)
        {
            Type t = element.ReturnType;
            var fa = element.GetCustomAttribute<TsFunctionAttribute>();
            if (fa != null)
            {
                if (fa.StrongType != null) t = fa.StrongType;
            }
            return t;
        }
        protected virtual void GetFunctionNameAndReturnType(MethodInfo element, TypeResolver resolver, out string name, out string type)
        {
            name = element.Name;
            var fa = element.GetCustomAttribute<TsFunctionAttribute>();
            if (fa != null)
            {
                if (!string.IsNullOrEmpty(fa.Name)) name = fa.Name;

                if (!string.IsNullOrEmpty(fa.Type)) type = fa.Type;
                else if (fa.StrongType != null) type = resolver.ResolveTypeName(fa.StrongType);
                else type = resolver.ResolveTypeName(element.ReturnType);
            }
            else
            {
                type = resolver.ResolveTypeName(element.ReturnType);
            }

        }

        protected virtual void WriteExistingParameters(MethodInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            ParameterInfo[] p = element.GetParameters();
            for (int index = 0; index < p.Length; index++)
            {
                var param = p[index];
                if (param.IsIgnored()) continue;
                var generator = resolver.GeneratorFor(param);
                generator.Generate(param, resolver, sw);
                if (index != p.Length - 1 && !p[index + 1].IsIgnored())
                {
                    sw.Write(", ");
                }
            }
        }
        protected virtual void GenerateBody(MethodInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.ReturnType != typeof(void))
            {
                sw.WriteLine();
                sw.WriteIndented(@"{ 
    return null; 
}");
            }
            else
            {
                sw.Write("{{ }}");
                sw.WriteLine();
            }
        }

        protected void WriteFunctionName(bool isStatic, string name, WriterWrapper sw, bool isInterfaceDecl = false)
        {
            sw.Tab();
            sw.Indent();
            if (!isInterfaceDecl)
            {
                sw.Write("public ");
                if (isStatic) sw.Write("static ");
            }

            sw.Write("{0}(", name);
        }

        protected void WriteRestOfDeclaration(string type, WriterWrapper sw)
        {
            sw.Write(") : {0}", type);
        }
        public virtual void Generate(MethodInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;

            var isInterfaceMethod = element.DeclaringType.IsExportingAsInterface();
            string name, type;

            GetFunctionNameAndReturnType(element, resolver, out name, out type);
            WriteFunctionName(element.IsStatic, name, sw, isInterfaceMethod);

            WriteExistingParameters(element, resolver, sw);

            WriteRestOfDeclaration(type, sw);

            if (isInterfaceMethod) { sw.Write(";"); sw.WriteLine(); }
            else GenerateBody(element, resolver, sw);
            sw.UnTab();
        }



    }
}
