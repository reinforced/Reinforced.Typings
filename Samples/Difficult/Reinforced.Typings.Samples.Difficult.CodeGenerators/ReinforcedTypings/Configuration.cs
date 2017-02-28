using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings
{
    public class Configuration
    {
        public static void Configure(ConfigurationBuilder builder)
        {
            // Configuration for exporting JQuery infrastructure

            // Configuration exporting test model class as interface
            builder.ExportAsInterface<SampleResponseModel>().WithPublicProperties().ExportTo("models.ts");

            // Configuration for JQueryController
            // Setting code generator for each method performed in JQueryMethodAttribute
            builder.ExportAsClass<JQueryController>().ExportTo("JQueryController.ts");

            // Configuration for Angular.js-firndly controller
            // Setting code generator for methods also performed in attribute - see AngularMethodAttribute
            builder.ExportAsClass<AngularController>()
                .ExportTo("AngularController.ts")
                .WithCodeGenerator<AngularControllerGenerator>();
        }
    }
}