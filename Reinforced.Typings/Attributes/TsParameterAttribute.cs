using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Overrides settings for exporting parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TsParameterAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides property/field type
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Overrides property/field type with managed type
        /// </summary>
        public virtual Type StrongType { get; set; }

        /// <summary>
        /// Overrides name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Specifies default value
        /// </summary>
        public virtual object DefaultValue { get; set; }
    }
}
