using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Exports enum as TypeScript Enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class TsEnumAttribute : TsDeclarationAttributeBase
    {
    }
}