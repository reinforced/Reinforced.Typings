using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Specifies file where to put generated code for type.
    ///     This attribute is being ignored when RtDivideTypesAmongFiles is false.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class TsFile : Attribute
    {
        /// <summary>
        ///     Constructs new TsFile attribute
        /// </summary>
        /// <param name="fileName">File name (related to RtTargetDirectory) where to put generated code</param>
        public TsFile(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        ///     File name (related to RtTargetDirectory) where to put generated code
        /// </summary>
        public string FileName { get; set; }
    }
}