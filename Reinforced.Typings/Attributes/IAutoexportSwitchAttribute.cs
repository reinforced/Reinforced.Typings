using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Interface containing base properties for some attributes
    /// </summary>
    public interface IAutoexportSwitchAttribute
    {
        /// <summary>
        ///     When true, code for all methods will be automatically generated
        /// </summary>
        bool AutoExportMethods { get; set; }

        /// <summary>
        ///     When true, code for all properties will be automatically generated
        /// </summary>
        bool AutoExportProperties { get; set; }

        /// <summary>
        ///     When true, code for all fields will be automatically generated
        /// </summary>
        bool AutoExportFields { get; }

        /// <summary>
        ///     Reference to code geenrator which will be applied to every method
        /// </summary>
        Type DefaultMethodCodeGenerator { get; }

        /// <summary>
        ///     When true, code for all constructors will be automatically generated
        /// </summary>
        bool AutoExportConstructors { get; }
    }
}