using System;

namespace Reinforced.WebTypings
{
    /// <summary>
    /// Instructs DynTyping do not to export mentioned member
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Method|AttributeTargets.Parameter)]
    public class TsIgnoreAttribute : Attribute
    {
    }
}
