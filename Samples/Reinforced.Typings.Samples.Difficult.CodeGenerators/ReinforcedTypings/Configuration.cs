using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.Models;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings
{
    public class Configuration
    {
        public static void Configure(ConfigurationBuilder builder)
        {
            // Configuration for exporting JQuery infrastructure
            builder.ExportAsClass<JQueryController>();
            builder.ExportAsInterface<JQuerySampleResponseModel>();

        }
    }
}