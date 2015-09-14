using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reinforced.Typings
{
    internal class FilesOperations
    {
        private readonly ExportSettings _settings;
        private readonly List<string> _tmpFiles = new List<string>();

        public FilesOperations(ExportSettings settings)
        {
            _settings = settings;
        }

        public void DeployTempFiles()
        {
            foreach (var tmpFile in _tmpFiles)
            {
                var origFile = Path.GetFileNameWithoutExtension(tmpFile);
                var origDir = Path.GetDirectoryName(tmpFile);

                origFile = Path.Combine(origDir, origFile);

                if (File.Exists(origFile)) File.Delete(origFile);
                File.Move(tmpFile, origFile);
                Console.WriteLine("File replaced: {0} -> {1}", tmpFile, origFile);
            }
        }

        public string GetTmpFile(string fileName)
        {
            fileName = fileName + ".tmp";
            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            Console.WriteLine("Test file aquired: {0}", fileName);
            _tmpFiles.Add(fileName);
            return fileName;
        }

        public string GetPathForType(Type t)
        {
            var ns = t.GetNamespace();
            var tn = t.GetName();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (_settings.ExportPureTypings) tn = tn + ".d.ts";
            else tn = tn + ".ts";

            if (string.IsNullOrEmpty(ns)) return Path.Combine(_settings.TargetDirectory, tn);
            if (!string.IsNullOrEmpty(_settings.RootNamespace))
            {
                ns = ns.Replace(_settings.RootNamespace, String.Empty);
            }
            ns = ns.Trim('.').Replace('.', '\\');

            var pth = Path.Combine(!string.IsNullOrEmpty(ns) ? Path.Combine(_settings.TargetDirectory, ns) : _settings.TargetDirectory, tn);

            return pth;
        }

        public string GetRelativePathForType(Type t, string currentNamespace)
        {
            currentNamespace = currentNamespace.Replace(_settings.RootNamespace, String.Empty);

            currentNamespace = currentNamespace.Replace('.', '\\').Trim('\\');

            var path = GetPathForType(t).Replace(_settings.TargetDirectory, String.Empty);
            var fileName = Path.GetFileName(path);
            var desiredNamespace = Path.GetDirectoryName(path).Trim('\\');

            var relPath = GetRelativeNamespacePath(currentNamespace, desiredNamespace);

            relPath = Path.Combine(relPath, fileName);
            relPath = relPath.Replace('\\', '/');
            return relPath;
        }

        private string GetRelativeNamespacePath(string currentNamespace, string desiredNamespace)
        {
            if (currentNamespace==desiredNamespace) return String.Empty;
            if (string.IsNullOrEmpty(currentNamespace)) return desiredNamespace;
            

            var current = currentNamespace.Split('\\');
            var desired = desiredNamespace.Split('\\');
            
            StringBuilder result = new StringBuilder();
            if (string.IsNullOrEmpty(desiredNamespace))
            {
                for (int i = 0; i < current.Length; i++) result.Append("..\\");
            }
            else
            {
                int level = current.Length - 1;
                while (level >= 0 && (current.I(level) != desired.I(level)))
                {
                    result.Append("..\\");
                    level--;
                }
                level++;
                for (; level < desired.Length; level++)
                {
                    result.AppendFormat("{0}\\", desired[level]);
                }
            }
            return result.ToString().Trim('\\');
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
