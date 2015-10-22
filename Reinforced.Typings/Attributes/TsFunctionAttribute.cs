using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Overrides function export
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TsFunctionAttribute : TsTypedMemberAttributeBase
    {
    }
}