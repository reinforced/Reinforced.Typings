namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Configuration interface for members supporting names overriding
    /// </summary>
    public interface INameOverrideAttribute
    {
        /// <summary>
        ///     Name override
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// Configuration interface for members supporting namespaces overriding
    /// </summary>
    public interface INamespaceOverrideAttribute
    {
        /// <summary>
        ///     Name override
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        ///     Gets or sets whether type must be placed into corresponding namespace
        /// </summary>
        bool IncludeNamespace { get; set; }

    }

    /// <summary>
    ///     Configuration interface for members supporting camelCasing from attribute
    /// </summary>
    public interface ICamelCaseableAttribute
    {
        /// <summary>
        ///     camelCase flag
        /// </summary>
        bool ShouldBeCamelCased { get; set; }
    }

    /// <summary>
    ///     Configuration interface for members supporting PascalCasing from attribute
    /// </summary>
    public interface IPascalCasableAttribute
    {
        /// <summary>
        ///     PascalCase flag
        /// </summary>
        bool ShouldBePascalCased { get; set; }
    }
}