namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IExportConfiguration<out TAttributePrototype> : IExportConfigurationNongeneric
    {
        TAttributePrototype AttributePrototype { get; }
    }
}