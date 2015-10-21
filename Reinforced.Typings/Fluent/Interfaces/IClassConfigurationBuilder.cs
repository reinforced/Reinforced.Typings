using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Technical interface for class configuration builder
    /// </summary>
    public interface IClassConfigurationBuilder : ITypeConfigurationBuilder,
        IExportConfiguration<TsClassAttribute>
    {
        
    }
}