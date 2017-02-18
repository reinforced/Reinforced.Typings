using System;
using System.IO;

namespace Reinforced.Typings
{
    /// <summary>
    /// Interface of files operator
    /// It has to be public for testing purposes
    /// </summary>
    public interface IFilesOperations
    {
        /// <summary>
        /// Writes temporary files contents to disk
        /// </summary>
        void DeployTempFiles();

        /// <summary>
        /// Retrieves stream for temporary file
        /// </summary>
        /// <param name="fileName">Temporary file name</param>
        /// <returns></returns>
        Stream GetTmpFile(string fileName);

        /// <summary>
        /// Retrieves physical path type must be placed to according to .ExportTo settings
        /// </summary>
        /// <param name="t">Type that path is needed for</param>
        /// <returns>Absolute path for type to be placed to</returns>
        string GetPathForType(Type t);

        /// <summary>
        /// Retrieves path for type to be exported to relatively to currently exported type
        /// </summary>
        /// <param name="typeToReference">Type that needs to be referenced</param>
        /// <param name="currentlyExportingType">Currently esporting type</param>
        /// <returns>String representing path to typeToReference related to currentlyExportingType</returns>
        string GetRelativePathForType(Type typeToReference, Type currentlyExportingType);

        /// <summary>
        /// Cleans up temporary files registry
        /// </summary>
        void ClearTempRegistry();
    }
}