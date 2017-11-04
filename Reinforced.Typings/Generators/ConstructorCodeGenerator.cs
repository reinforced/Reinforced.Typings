using System;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Xmldoc.Model;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for constructor
    /// </summary>
    public class ConstructorCodeGenerator : TsCodeGeneratorBase<ConstructorInfo,RtConstructor>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtConstructor GenerateNode(ConstructorInfo element, RtConstructor result, TypeResolver resolver)
        {
            if (element.IsIgnored()) return null;
            if (element.GetParameters().Length == 0) return null;

            var doc = Context.Documentation.GetDocumentationMember(element);
            if (doc != null)
            {
                RtJsdocNode jsdoc = new RtJsdocNode { Description = doc.Summary.Text };
                foreach (var documentationParameter in doc.Parameters)
                {
                    jsdoc.TagToDescription.Add(new Tuple<DocTag, string>(DocTag.Param,
                        documentationParameter.Name + " " + documentationParameter.Description));
                }
                result.Documentation = jsdoc;
            }

            var p = element.GetParameters();
            foreach (var param in p)
            {
                if (param.IsIgnored()) continue;
                var generator = Context.Generators.GeneratorFor(param, Context);
                var argument = generator.Generate(param, resolver);
                result.Arguments.Add((RtArgument) argument);
            }
            SetupSuperCall(result, element);
            return result;
        }

        private void SetupSuperCall(RtConstructor constructor, ConstructorInfo element)
        {
            constructor.SuperCallParameters.Clear();
            // 1. Check presence of base type 
            var bt = element.DeclaringType != null ? element.DeclaringType._BaseType() : null;
            if ((bt == typeof (object) || bt.IsExportingAsInterface()) || !bt.IsExportingAsClass()) bt = null;
            
            if (bt == null)
            {
                // 1. If not present then generate empty constructor body
                return;
            }
            var parameters = element.GetParameters();
            // 2. Check presence of [TsBaseParam]
            var attr = element.GetCustomAttribute<TsBaseParamAttribute>(false);
            if (attr != null)
            {
                // 2. If present then generate super() call with supplied parameters
                constructor.SuperCallParameters.AddRange(attr.Values.Take(parameters.Length).ToList());
                if (constructor.SuperCallParameters.Count > 0)
                {
                    constructor.NeedsSuperCall = true;
                }
                return;
            }
            var baseConstructors =
                bt.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                   BindingFlags.Static |
                                   BindingFlags.DeclaredOnly);

            // 3. Trying to lookup constructor with same parameters
            var found = false;

            var paramTypes = parameters.Select(c => c.ParameterType).ToArray();
            var corresponding = TypeExtensions.GetMethodWithSameParameters(
                baseConstructors.Cast<MethodBase>().ToArray(), paramTypes);
            found = corresponding != null;

            if (found)
            {
                // 3.If constructor with same parameters found - just use it
                constructor.SuperCallParameters.AddRange(parameters.Select(c => c.GetName()));
                if (constructor.SuperCallParameters.Count > 0)
                {
                    constructor.NeedsSuperCall = true;
                }
                return;
            }

            // 4. Maybe parameterles constructor?
            if (baseConstructors.Any(c => c.GetParameters().Length == 0))
            {
                // 4. If not present then generate empty super() call
                constructor.NeedsSuperCall = true;
                return;
            }

            // 5. If nothing found - well... we simply leave here super with all nulls supplied
            var maxParams = baseConstructors.Max(c => c.GetParameters().Length);
            var mockedCtorParams = Enumerable.Repeat("null", maxParams);
            constructor.NeedsSuperCall = true;
            constructor.SuperCallParameters.AddRange(mockedCtorParams);
            Context.Warnings.Add(ErrorMessages.RTW0004_DefaultSuperCall.Warn(element.DeclaringType.FullName));
        }
    }
}