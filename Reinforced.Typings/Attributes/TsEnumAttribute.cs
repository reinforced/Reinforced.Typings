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

        /// <summary>
        /// Gets or sets whetner enum fields must be exported with string initializers (TypeScript 2.4)
        /// </summary>
        public bool UseString { get; set; }
    }
}