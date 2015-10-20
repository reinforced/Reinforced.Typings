using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IInterfaceConfigurationBuilder : ITypeConfigurationBuilder,
        IExportConfiguration<TsInterfaceAttribute>
    {
        
    }
}