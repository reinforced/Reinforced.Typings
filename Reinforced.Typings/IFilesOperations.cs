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
        ExportContext Context { set; }

        /// <summary>
        /// Writes temporary files contents to disk
        /// </summary>
        void DeployTempFiles();

        /// <summary>
        /// Cleans up temporary files registry
        /// </summary>
        void ClearTempRegistry();

        /// <summary>
        /// Exports specified syntax nodes to specified file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="file">File to be exported</param>
        void Export(string fileName, ExportedFile file);
    }
}