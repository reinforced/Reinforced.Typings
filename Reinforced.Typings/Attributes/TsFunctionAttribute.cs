using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Overrides function export
    /// </summary>
    public class TsFunctionAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides function return type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Overrides function return type with managed type
        /// </summary>
        public Type StrongType { get; set; }

        /// <summary>
        /// Overrides name
        /// </summary>
        public string Name { get; set; }
    }
}
