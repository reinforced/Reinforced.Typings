using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.jQuery
{
    /// <summary>
    /// This is our code generator. Its purpose is to consume MethodInfo as source 
    /// and produce RtFunction TypeScript AST node as result. 
    /// We will inherit regular Method Code generator and slilghtly adjust its result 
    /// for convinence.
    /// </summary>
    public class JQueryActionCallGenerator : MethodCodeGenerator
    {
        /// <summary>
        /// We override GenerateNode method (since it is almost single method of code generator). 
        /// </summary>
        /// <param name="element">Method of controller that we generate code for</param>
        /// <param name="result">
        /// Resulting node - we do not have to create resulting node by ourselfs. 
        /// But wi still can return null from GenerateNode method
        /// </param>
        /// <param name="resolver">
        /// TypeResolver object that we will use to safely convert CLR types to TypeScript types names
        /// </param>
        /// <returns>AST node for method declaration</returns>
        public override RtFuncion GenerateNode(MethodInfo element, RtFuncion result, TypeResolver resolver)
        {
            // Here we get default result of method export
            result =  base.GenerateNode(element, result, resolver);
            
            // We make method static since we will call it like JQueryController.Method
            result.IsStatic = true;

            // Below we will add special arguments for specifying  element that should be 
            // disabled during query (disable element) and element placeholder for
            // loading inidicator
            result.Arguments.Add(
                new RtArgument()
                {
                    Identifier = new RtIdentifier("loadingPlaceholderSelector"),
                    Type = resolver.ResolveTypeName(typeof(string)),
                    DefaultValue = "''"
                });

            result.Arguments.Add(
               new RtArgument()
               {
                   Identifier = new RtIdentifier("disableElement"),
                   Type = resolver.ResolveTypeName(typeof(string)),
                   DefaultValue = "''"
               });

            // We save original type name
            var retType = result.ReturnType;
            
            // ... and in case of void we just replace it with "any"
            bool isVoid = (retType is RtSimpleTypeName) && (((RtSimpleTypeName) retType).TypeName == "void");

            // we use TypeResolver to get "any" type to avoid redundant type name construction
            // (or because I'm too lazy to manually construct "any" type)
            if (isVoid) retType = resolver.ResolveTypeName(typeof (object));

            // Here we override TS method return type to make it JQueryPromise
            // We are using RtSimpleType with generig parameter of existing method type
            result.ReturnType = new RtSimpleTypeName("JQueryPromise", new[] { retType });

            // Here we retrieve method parameters
            // We are using .GetName() extension method to retrieve parameter name
            // It is supplied within Reinforced.Typings and retrieves parameter name 
            // including possible name override with Fluent configuration or 
            // [TsParameter] attribute
            var p = element.GetParameters().Select(c => string.Format("'{0}': {0}", c.GetName()));

            // Joining parameters for method body code
            var dataParameters = string.Join(", ", p);

            // Here we get path to controller
            // It is quite simple solution requiring /{controller}/{action} route
            string controller = element.DeclaringType.Name.Replace("Controller", String.Empty);
            string path = String.Format("/{0}/{1}", controller, element.Name);

            // Here we are constructing our glue code
            string code = String.Format(
@"return QueryController.query<{2}>(
        '{0}', 
        {{ {1} }}, 
        loadingPlaceholderSelector,
        disableElement
    );", 
       path, dataParameters, retType);

            // Here we just set method body and return method node
            result.Body = new RtRaw(code);
            return result;
        }
    }
}