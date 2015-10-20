using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IClassConfigurationBuilder : ITypeConfigurationBuilder,
        IExportConfiguration<TsClassAttribute>
    {
        
    }
}