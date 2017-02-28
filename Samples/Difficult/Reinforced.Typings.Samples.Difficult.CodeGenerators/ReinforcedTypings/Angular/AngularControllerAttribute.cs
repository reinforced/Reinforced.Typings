using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.ReinforcedTypings.Angular
{
    /// <summary>
    /// Attribute for controller that contains angular-exported methods
    /// </summary>
    public class AngularControllerAttribute : TsClassAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AngularControllerAttribute()
        {
            // Here we override code generator for whole type
            CodeGeneratorType = typeof (AngularControllerGenerator);

            // Disable constructors export
            AutoExportConstructors = false;

            // Disable automatic methods export
            AutoExportMethods = false;
        }
    }
}