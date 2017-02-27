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