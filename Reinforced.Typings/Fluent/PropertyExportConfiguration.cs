using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported properties
    /// </summary>
    public class PropertyExportConfiguration : IExportConfiguration<TsPropertyAttribute>, IIgnorable
    {
        internal PropertyExportConfiguration()
        {
            AttributePrototype = new TsPropertyAttribute();
        }

        private TsPropertyAttribute AttributePrototype { get; set; }

        TsPropertyAttribute IExportConfiguration<TsPropertyAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
    }
}