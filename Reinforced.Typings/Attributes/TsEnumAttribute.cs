using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Exports enum as TypeScript Enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class TsEnumAttribute : TsDeclarationAttributeBase
    {
        /// <summary>
        /// When true, results "const" enum instead of usual
        /// </summary>
        public bool IsConst { get; set; }
    }
}