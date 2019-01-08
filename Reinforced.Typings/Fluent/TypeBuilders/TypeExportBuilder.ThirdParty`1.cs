using System;
using System.Linq.Expressions;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for class (generic)
    /// </summary>
    public class ThirdPartyExportBuilder<T> : ThirdPartyExportBuilder
    {
        internal ThirdPartyExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
        }
    }
}