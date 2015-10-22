using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides settings for exporting parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TsParameterAttribute : TsTypedMemberAttributeBase
    {
        /// <summary>
        ///     Specifies default value
        /// </summary>
        public virtual object DefaultValue { get; set; }
    }
}