namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IExportConfiguration<out TAttributePrototype>
    {
        TAttributePrototype AttributePrototype { get; }
    }
}