namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Core fluent export configuration interface
    /// </summary>
    /// <typeparam name="TAttributePrototype">Attribute prototype for specified member</typeparam>
    public interface IExportConfiguration<out TAttributePrototype>
    {
        /// <summary>
        /// Attribute prototype
        /// </summary>
        TAttributePrototype AttributePrototype { get; }
    }
}