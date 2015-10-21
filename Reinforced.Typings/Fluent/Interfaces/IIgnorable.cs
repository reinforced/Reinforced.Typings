namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Configuration for members that are supporting ignoring
    /// </summary>
    public interface IIgnorable
    {
        /// <summary>
        /// Ignore flag
        /// </summary>
        bool Ignore { get; set; }
    }
}
