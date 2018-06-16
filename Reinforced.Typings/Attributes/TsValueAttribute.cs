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

        /// <summary>
        /// Overrides enum value's string initializer. This property works only if there is <see cref="TsEnumAttribute.UseString"/> property set to true.
        /// Please escape quotes manually.
        /// </summary>
        public string Initializer { get; set; }
    }
}