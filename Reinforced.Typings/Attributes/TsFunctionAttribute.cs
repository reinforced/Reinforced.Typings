using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Overrides function export
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Method)]
    public class TsFunctionAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides function return type
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Overrides function return type with managed type
        /// </summary>
        public virtual Type StrongType { get; set; }

        /// <summary>
        /// Overrides name
        /// </summary>
        public virtual string Name { get; set; }
    }
}
