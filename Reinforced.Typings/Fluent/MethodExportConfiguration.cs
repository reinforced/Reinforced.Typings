using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent configuration builder for exported methods
    /// </summary>
    public class MethodExportConfiguration : IExportConfiguration<TsFunctionAttribute>, IIgnorable
    {
        bool IIgnorable.Ignore { get; set; }
        private TsFunctionAttribute AttributePrototype { get; set; }

        TsFunctionAttribute IExportConfiguration<TsFunctionAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        internal MethodExportConfiguration()
        {
            AttributePrototype = new TsFunctionAttribute();
        }
    }
}