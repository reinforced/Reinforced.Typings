using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Specifies exporting enum value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TsValueAttribute : TsAttributeBase
    {
        /// <summary>
        /// Overrides enum value name
        /// </summary>
        public string NameOverride { get; set; }
    }
}
