using System;

namespace Reinforced.WebTypings
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
        /// Specifies default value
        /// </summary>
        public object DefaultValue { get; set; }
    }
}
