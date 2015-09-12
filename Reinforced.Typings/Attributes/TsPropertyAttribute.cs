using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Overrides property/field export settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class TsPropertyAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides property/field type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Overrides property/field type with managed type
        /// </summary>
        public Type StrongType { get; set; }

        /// <summary>
        /// Overrides name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Forces property to be a nullable
        /// </summary>
        public bool ForceNullable { get; set; }
    }
}
