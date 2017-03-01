using System;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    /// <summary>
    /// Attribute that we place above out controller's methods
    /// </summary>
    public class AngularMethodAttribute : TsFunctionAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="returnType"></param>
        public AngularMethodAttribute(Type returnType)
        {
            // Here we override method return type for TypeScript export
            StrongType = returnType;

            // Here we are specifying code generator for particular method
            CodeGeneratorType = typeof (AngularActionCallGenerator);
        }
    }
}