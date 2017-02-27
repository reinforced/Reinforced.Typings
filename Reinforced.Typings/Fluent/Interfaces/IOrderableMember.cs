namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    /// Configuration interface for members supporting reordering from attribute
    /// </summary>
    public interface IOrderableMember
    {
        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        double MemberOrder { get; set; }
    }
}