using System;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.jQuery
{
    /// <summary>
    /// Since controller method only can return ActionResult and 
    /// there is no way to programmaticaly determine action result then 
    /// we need special attribute to describe controller method return. 
    /// We will inherit this attribute from TsFunction attribute since we 
    /// anyway have to specify exported methods in fluent configuration either 
    /// via attributes.
    /// </summary>
    public class JQueryMethodAttribute : TsFunctionAttribute
    {
        public JQueryMethodAttribute(Type returnType)
        {
            // Here we override method return type for TypeScript export
            StrongType = returnType;

            // Here we are specifying code generator for particular method
            CodeGeneratorType = typeof (JQueryActionCallGenerator);
        }
    }
}