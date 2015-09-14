using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Base for all attributes
    /// </summary>
    public abstract class TsAttributeBase : Attribute
    {
        /// <summary>
        /// Dummy function body generator 
        /// If empty then it's being generated empty/return null body.
        /// </summary>
        public virtual Type CodeGeneratorType { get; set; }
    }
}
