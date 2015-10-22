using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Specifies exporting enum value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TsValueAttribute : TsAttributeBase, INameOverrideAttribute
    {
        /// <summary>
        ///     Overrides enum value name
        /// </summary>
        public virtual string Name { get; set; }
    }
}