using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings
{
    internal class FilesOperations : IFilesOperations
    {
        private readonly ExportContext _context;
        private readonly List<string> _tmpFiles = new List<string>();

        public FilesOperations(ExportContext context)
        {
            _context = context;
        }

        public void DeployTempFiles()
        {
            foreach (var tmpFile in _tmpFiles)
            {
                var origFile = Path.GetFileNameWithoutExtension(tmpFile);
                var origDir = Path.GetDirectoryName(tmpFile);
                origFile = Path.Combine(origDir, origFile);
                try
                {
                    if (File.Exists(origFile)) File.Delete(origFile);
                    File.Move(tmpFile, origFile);
#if DEBUG
                Console.WriteLine("File replaced: {0} -> {1}", tmpFile, origFile);
#endif
                }
                catch (Exception ex)
                {
                    ErrorMessages.RTE0002_DeployingFilesError.Throw(origFile, ex.Message);
                }
            }
        }

        public Stream GetTmpFile(string fileName)
        {
            fileName = fileName + ".tmp";
            try
            {
                var dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
#if DEBUG
            Console.WriteLine("Temp file aquired: {0}", fileName);
#endif
                _tmpFiles.Add(fileName);
            }
            catch (Exception ex)
            {
                ErrorMessages.RTE0001_TempFileError.Throw(fileName, ex.Message);
            }

            return File.OpenWrite(fileName);
        }

        
        
        public void ClearTempRegistry()
        {
            _tmpFiles.Clear();
        }
    }

    internal static class ArrayExtensions
    {
        public static T I<T>(this T[] array, int idx)
        {
            if (idx >= array.Length) return default(T);
            return array[idx];
        }
    }
}