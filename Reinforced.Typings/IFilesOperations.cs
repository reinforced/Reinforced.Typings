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
        /// Cleans up temporary files registry
        /// </summary>
        void ClearTempRegistry();

        void Export(string fileName, ExportedFile file);
    }
}