using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    /// <summary>
    ///     Configuration interface for members supporting custom typescript-per-file references
    /// </summary>
    public interface IReferenceConfigurationBuilder
    {
        /// <summary>
        ///     Reference paths list
        /// </summary>
        ICollection<TsAddTypeReferenceAttribute> References { get; }

        /// <summary>
        ///     Path to file where to put generated code
        /// </summary>
        string PathToFile { get; set; }
    }
}