using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    public class AngularControllerGenerator : ClassCodeGenerator
    {
        public override RtClass GenerateNode(Type element, RtClass result, TypeResolver resolver)
        {
            result = base.GenerateNode(element, result, resolver);
            if (result == null) return null;

            var httpServiceType = new RtSimpleTypeName("IHttpService"){Namespace = "ng"};

            RtConstructor constructor = new RtConstructor();
            constructor.Arguments.Add(new RtArgument(){Type = httpServiceType,Identifier = new RtIdentifier("$http")});
            constructor.Body = new RtRaw("this.http = $http;");

            RtField httpServiceField = new RtField()
            {
                Type = httpServiceType,
                Identifier = new RtIdentifier("http"),
                AccessModifier = AccessModifier.Private
            };

            result.Members.Add(httpServiceField);
            result.Members.Add(constructor);

            const string initializerFormat =
                "app.factory('Api.{0}', ['$http', ($http: ng.IHttpService) => new {1}($http)]);";

            RtRaw registration = new RtRaw(String.Format(initializerFormat,element.Name,result.Name));
            Context.Location.CurrentModule.CompilationUnits.Add(registration);
            
            return result;
        }
    }
}