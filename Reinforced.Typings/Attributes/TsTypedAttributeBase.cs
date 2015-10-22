using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Base attribute for typed members/parameters
    /// </summary>
    public abstract class TsTypedAttributeBase : TsAttributeBase
    {
        /// <summary>
        ///     Overrides member type
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        ///     Overrides member type with managed type
        /// </summary>
        public virtual Type StrongType { get; set; }
    }
}