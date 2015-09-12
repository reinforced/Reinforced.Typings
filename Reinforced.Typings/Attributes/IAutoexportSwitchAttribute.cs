using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Interface containing base properties for some attributes
    /// </summary>
    public interface IAutoexportSwitchAttribute
    {
        /// <summary>
        /// Export all methods automatically or not. 
        /// </summary>
        bool AutoExportMethods { get; }

        /// <summary>
        /// Export all properties automatically or not. 
        /// </summary>
        bool AutoExportProperties { get; }

        /// <summary>
        /// Export all fields automatically or not. 
        /// </summary>
        bool AutoExportFields { get; }

        /// <summary>
        /// Reference to code geenrator which will be applied to every method
        /// </summary>
        Type DefaultMethodCodeGenerator { get; }
    }
}
