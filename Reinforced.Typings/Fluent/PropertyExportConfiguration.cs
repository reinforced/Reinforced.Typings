using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent configuration builder for exported properties
    /// </summary>
    public class PropertyExportConfiguration : IExportConfiguration<TsPropertyAttribute>, IIgnorable
    {
        bool IIgnorable.Ignore { get; set; }
        private TsPropertyAttribute AttributePrototype { get; set; }
        TsPropertyAttribute IExportConfiguration<TsPropertyAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }
        internal PropertyExportConfiguration()
        {
            AttributePrototype = new TsPropertyAttribute();
        }
    }
}