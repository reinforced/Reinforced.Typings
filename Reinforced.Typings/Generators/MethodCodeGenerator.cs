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
        /// <summary>
        /// Retrieves return type for specified method. Fell free to override it.
        /// </summary>
        /// <param name="element">Method</param>
        /// <returns>Types which is being returned by this method</returns>
        protected virtual Type GetReturnFunctionType(MethodInfo element)
        {
            Type t = element.ReturnType;
            var fa = element.GetCustomAttribute<TsFunctionAttribute>(false);
            if (fa != null)
            {
                if (fa.StrongType != null) t = fa.StrongType;
            }
            return t;
        }

        /// <summary>
        /// Retrieves function name corresponding to method and return type. Fell free to override it.
        /// </summary>
        /// <param name="element">Method info</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="name">Resulting method name</param>
        /// <param name="type">Resulting return type name</param>
        protected virtual void GetFunctionNameAndReturnType(MethodInfo element, TypeResolver resolver, out string name, out string type)
        {
            name = element.Name;
            var fa = element.GetCustomAttribute<TsFunctionAttribute>(false);

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
            name = Settings.ConditionallyConvertMethodNameToCamelCase(name);

        }


        /// <summary>
        /// Writes all method's parameters to output writer.
        /// </summary>
        /// <param name="element">Method info</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        protected virtual void WriteMethodParameters(MethodBase element, TypeResolver resolver, WriterWrapper sw)
        {
            ParameterInfo[] p = element.GetParameters();
            for (int index = 0; index < p.Length; index++)
            {
                var param = p[index];
                if (param.IsIgnored()) continue;
                var generator = resolver.GeneratorFor(param, Settings);
                generator.Generate(param, resolver, sw);
                if (index != p.Length - 1 && !p[index + 1].IsIgnored())
                {
                    sw.Write(", ");
                }
            }
        }

        /// <summary>
        /// Writes method body to output writer
        /// </summary>
        /// <param name="returnType">Method return type</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        /// <param name="content">Content for non-void body</param>
        protected virtual void GenerateBody(string returnType, TypeResolver resolver, WriterWrapper sw, string content = "return null;")
        {
            if (Settings.ExportPureTypings) //Ambient class declarations cannot have a body
            {
                sw.Write(";");
                sw.Br();
            }
            else
            {
                if (returnType != "void")
                {
                    sw.WriteLine();
                    sw.WriteIndented(@"{{ 
    {0}
}}", content);
                }
                else
                {
                    sw.Write(" {{ }}");
                    sw.Br();
                }
            }
        }

        /// <summary>
        /// Writes method name, accessor and opening brace to output writer
        /// </summary>
        /// <param name="isStatic">Is method static or not</param>
        /// <param name="accessModifier">Access modifier for method</param>
        /// <param name="name">Method name</param>
        /// <param name="sw">Output writer</param>
        /// <param name="isInterfaceDecl">Is this method interface declaration or not (access modifiers prohibited on interface declaration methods)</param>
        protected void WriteFunctionName(bool isStatic, AccessModifier accessModifier, string name, WriterWrapper sw, bool isInterfaceDecl = false)
        {
         
            if (!isInterfaceDecl)
            {
                sw.Write("{0} ", accessModifier.ToModifierText());
                if (isStatic) sw.Write("static ");
            }

            sw.Write("{0}(", name);
        }

        /// <summary>
        /// Writes rest of method declaration to output writer (after formal parameters list)
        /// </summary>
        /// <param name="type">Returning type name</param>
        /// <param name="sw">Output writer</param>
        protected void WriteRestOfDeclaration(string type, WriterWrapper sw)
        {
            if (string.IsNullOrEmpty(type))
            {
                sw.Write(")");
            }
            else
            {
                sw.Write("): {0}", type);
            }
        }
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(MethodInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;

            var isInterfaceMethod = element.DeclaringType.IsExportingAsInterface() && !Settings.SpecialCase;
            string name, type;

            GetFunctionNameAndReturnType(element, resolver, out name, out type);

            sw.Tab();
            Settings.Documentation.WriteDocumentation(element, sw);
            sw.Indent();
            var modifier = element.GetModifier();
            
            if (Settings.SpecialCase) modifier = AccessModifier.Public;
            
            WriteFunctionName(element.IsStatic, modifier, name, sw, isInterfaceMethod);
            WriteMethodParameters(element, resolver, sw);
            WriteRestOfDeclaration(type, sw);

            if (isInterfaceMethod) { sw.Write(";"); sw.Br(); }
            else GenerateBody(type, resolver, sw);
            sw.UnTab();
        }

        /// <summary>
        /// Export settings
        /// </summary>
        public ExportSettings Settings { get; set; }
    }
}
