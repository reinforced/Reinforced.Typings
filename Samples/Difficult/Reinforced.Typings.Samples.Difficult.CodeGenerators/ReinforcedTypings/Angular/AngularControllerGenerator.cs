using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    /// <summary>
    /// We have to add some fields and constructor to Angular service
    /// </summary>
    public class AngularControllerGenerator : ClassCodeGenerator
    {
        public override RtClass GenerateNode(Type element, RtClass result, TypeResolver resolver)
        {
            result = base.GenerateNode(element, result, resolver);
            if (result == null) return null;

            // We add some docs to keep you oriented
            result.Documentation = new RtJsdocNode(){Description = "Result of AngularControllerGenerator activity"};

            // Here we just create ng.IHttpService type name because it is used several times
            var httpServiceType = new RtSimpleTypeName("IHttpService") { Namespace = "angular" };

            // Here we are declaring constructor for our angular service using $http as parameter
            // It is quite simple so no more details
            RtConstructor constructor = new RtConstructor();
            constructor.Arguments.Add(new RtArgument(){Type = httpServiceType,Identifier = new RtIdentifier("$http")});
            constructor.Body = new RtRaw("this.http = $http;");

            // Here we declaring class field for storing $http instance
            RtField httpServiceField = new RtField()
            {
                Type = httpServiceType,
                Identifier = new RtIdentifier("http"),
                AccessModifier = AccessModifier.Private,
                Documentation = new RtJsdocNode() { Description = "Keeps $http instance received on construction"}
            };

            // Here we are adding our constructor and field to resulting class
            result.Members.Add(httpServiceField);
            result.Members.Add(constructor);

            // Also we will add controller registration to our app instance
            // To automatically get it registered in Angular's IoC
            const string initializerFormat =
                "if (window['app']) window['app'].factory('Api.{0}', ['$http', ($http: angular.IHttpService) => new {1}($http)]);";

            RtRaw registration = new RtRaw(String.Format(initializerFormat,element.Name,result.Name));
            
            // Since RtModule.compilationUnits is not typed and could contain any type then we 
            // simply add RtRaw node here with registration glue code
            // app variable is declared in /Scripts/ReinforcedTypings/App.ts since it is used in 
            // corresponding client script
            Context.Location.CurrentModule.CompilationUnits.Add(registration);

            // That's all. here we return node that will be written to target file.
            // Check result in /Scripts/ReinforcedTypings/GeneratedTypings.ts
            return result;
        }
    }
}