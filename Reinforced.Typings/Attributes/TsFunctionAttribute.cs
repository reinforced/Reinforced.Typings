using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides function export
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TsFunctionAttribute : TsTypedMemberAttributeBase, IOrderableAttribute
    {
        /// <inheritdoc />
        public double Order { get; set; }

        /// <summary>
        /// Inline function code to be converted to RtRaw and used as function body
        /// </summary>
        public string Implementation { get; set; }

    }
}