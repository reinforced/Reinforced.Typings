using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    ///     Technical interface for interface configuration builder
    /// </summary>
    public interface IInterfaceConfigurationBuilder : ITypeConfigurationBuilder,
        IExportConfiguration<TsInterfaceAttribute>
    {
    }
}