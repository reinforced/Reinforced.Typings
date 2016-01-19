using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.jQuery;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings
{
    /// <summary>
    /// Since controller method only can return ActionResult and 
    /// there is no way to programmaticaly determine action result then 
    /// we need special attribute to describe controller method return. 
    /// We will inherit this attribute from TsFunction attribute since we 
    /// anyway have to specify exported methods in fluent configuration either 
    /// via attributes.
    /// </summary>
    public class TsReturnsAttribute : TsFunctionAttribute
    {
        public TsReturnsAttribute(Type returnType)
        {
            // Here we override method return type for TypeScript export
            StrongType = returnType;

            // Here we are specifying code generator for particular method
            CodeGeneratorType = typeof (JQueryActionCallGenerator);
        }
    }
}