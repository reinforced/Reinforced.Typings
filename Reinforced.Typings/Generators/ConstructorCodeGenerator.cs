using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    /// Default code generator for constructor
    /// </summary>
    public class ConstructorCodeGenerator : MethodCodeGenerator, ITsCodeGenerator<ConstructorInfo>
    {
        /// <summary>
        /// Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="sw">Output writer</param>
        public virtual void Generate(ConstructorInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            if (element.IsIgnored()) return;
            var isInterfaceMethod = element.DeclaringType.IsExportingAsInterface();
            if (element.GetParameters().Length==0) return;
            sw.Tab();
            Settings.Documentation.WriteDocumentation(element, sw);
            sw.Indent();
            WriteFunctionName(false, element.GetModifier(), "constructor", sw, isInterfaceMethod);
            WriteMethodParameters(element, resolver, sw);
            WriteRestOfDeclaration(String.Empty, sw);
            WriteConstructorBody(element, resolver, sw);
            sw.UnTab();
        }

        private void WriteConstructorBody(ConstructorInfo element, TypeResolver resolver, WriterWrapper sw)
        {
            // 1. Check presence of base type 
            var bt = element.DeclaringType != null ? element.DeclaringType.BaseType : null;
            if (bt == typeof(object) || bt.IsExportingAsInterface()) bt = null;

            if (bt == null)
            {
                // 1. If not present then generate empty constructor body
                GenerateBody("void", resolver, sw);
                return;
            }
            var parameters = element.GetParameters();
            // 2. Check presence of [TsBaseParam]
            var attr = element.GetCustomAttribute<TsBaseParamAttribute>(false);
            if (attr != null)
            {
                // 2. If present then generate super() call with supplied parameters
                var ctorParams = string.Join(", ", attr.Values.Take(parameters.Length));
                GenerateBody("somethingnonvoid", resolver, sw, String.Format("super({0});", ctorParams));
                return;
            }
            var baseConstructors = bt.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly);

            // 3. Trying to lookup constructor with same parameters
            bool found = false;

            var paramTypes = parameters.Select(c => c.ParameterType).ToArray();
            var corresponding = TypeExtensions.GetMethodWithSameParameters(baseConstructors.Cast<MethodBase>().ToArray(), paramTypes);
            found = corresponding != null;

            if (found)
            {
                // 3.If constructor with same parameters found - just use it
                var ctorParams = string.Join(", ", parameters.Select(c => c.GetName()));
                GenerateBody("somethingnonvoid", resolver, sw, String.Format("super({0}); // Please use [TsBaseParam] attribute here to generate more sensible super() call", ctorParams));
                return;
            }

            // 4. Maybe parameterles constructor?
            if (baseConstructors.Any(c => c.GetParameters().Length == 0))
            {
                // 4. If not present then generate empty super() call
                GenerateBody("somethingnonvoid", resolver, sw, "super(); // Please use [TsBaseParam] attribute here to generate more sensible super() call");
                return;
            }

            // 5. If nothing found - well... we simply leave here super with all nulls supplied
            var maxParams = baseConstructors.Max(c => c.GetParameters().Length);
            var mockedCtorParams = string.Join(", ", Enumerable.Repeat("null", maxParams));
            GenerateBody("somethingnonvoid", resolver, sw, String.Format("super({0}); // Please use [TsBaseParam] attribute here to generate more sensible super() call", mockedCtorParams));
        }
    }
}
