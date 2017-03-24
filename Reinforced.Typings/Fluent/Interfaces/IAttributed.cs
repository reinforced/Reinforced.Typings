namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    ///     Core fluent export configuration interface
    /// </summary>
    /// <typeparam name="TAttributePrototype">Attribute prototype for specified member</typeparam>
    public interface IAttributed<out TAttributePrototype>
    {
        /// <summary>
        ///     Attribute prototype
        /// </summary>
        TAttributePrototype AttributePrototype { get; }
    }

    /// <summary>
    /// Member export configuration
    /// </summary>
    /// <typeparam name="TAttributePrototype"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    public interface IMemberExportConfiguration<out TAttributePrototype, out TMember> : IAttributed<TAttributePrototype>
    {
        /// <summary>
        /// Exporting member
        /// </summary>
        TMember Member { get; }
    }
}