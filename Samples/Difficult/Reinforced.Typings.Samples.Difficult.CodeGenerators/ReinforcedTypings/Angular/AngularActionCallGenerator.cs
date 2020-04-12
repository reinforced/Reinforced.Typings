using System;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    /// <summary>
    /// Action call generator for controller method inside angularjs glue-class is quite similar
    /// to jQuery's one.
    /// </summary>
    public class AngularActionCallGenerator : MethodCodeGenerator
    {
        public override RtFunction GenerateNode(MethodInfo element, RtFunction result, TypeResolver resolver)
        {

            result = base.GenerateNode(element, result, resolver);
            if (result == null) return null;

            // here we are overriding return type to corresponding promise
            var retType = result.ReturnType;
            bool isVoid = (retType is RtSimpleTypeName) && (((RtSimpleTypeName)retType).TypeName == "void");

            // we use TypeResolver to get "any" type to avoid redundant type name construction
            // (or because I'm too lazy to manually construct "any" type)
            if (isVoid) retType = resolver.ResolveTypeName(typeof(object));

            // Here we override TS method return type to make it angular.IPromise
            // We are using RtSimpleType with generig parameter of existing method type
            result.ReturnType = new RtSimpleTypeName(new[] { retType }, "angular", "IPromise");

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

            const string code = @"var params = {{ {1} }};
return this.http.post('{0}', params)
    .then((response) => {{ response.data['requestParams'] = params; return response.data; }});";

            RtRaw body = new RtRaw(String.Format(code, path, dataParameters));
            result.Body = body;

            // That's all. here we return node that will be written to target file.
            // Check result in /Scripts/ReinforcedTypings/GeneratedTypings.ts
            return result;
        }
    }
}