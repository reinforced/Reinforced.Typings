using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular;

[assembly: TsGlobal(GenerateDocumentation = true)]

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings
{
    public class Configuration
    {
        public static void Configure(ConfigurationBuilder builder)
        {
            
            // --- For both tutorials
            // Configuration exporting test model class as interface
            builder.ExportAsInterface<SampleResponseModel>().WithPublicProperties().ExportTo("models.ts");

            // --- For jQuery Tutorial
            // Configuration for JQueryController
            // Setting code generator for each method performed in JQueryMethodAttribute
            builder.ExportAsClass<JQueryController>().ExportTo("JQueryController.ts");

            // --- For Angular tutorial
            // Configuration for Angular.js-firndly controller
            // Setting code generator for methods also performed in attribute - see AngularMethodAttribute
            builder.ExportAsClass<AngularController>()
                .ExportTo("AngularController.ts")
                .WithCodeGenerator<AngularControllerGenerator>();
        }
    }
}