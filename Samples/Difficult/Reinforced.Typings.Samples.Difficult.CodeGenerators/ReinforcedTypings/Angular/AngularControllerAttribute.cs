using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    public class AngularControllerAttribute : TsClassAttribute
    {
        public AngularControllerAttribute()
        {
            CodeGeneratorType = typeof (AngularControllerGenerator);
            AutoExportConstructors = false;
            AutoExportMethods = false;
        }
    }
}