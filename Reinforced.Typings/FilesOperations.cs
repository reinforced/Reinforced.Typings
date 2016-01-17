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
#if DEBUG
                Console.WriteLine("File replaced: {0} -> {1}", tmpFile, origFile);
#endif
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
#if DEBUG
            Console.WriteLine("Test file aquired: {0}", fileName);
#endif
            _tmpFiles.Add(fileName);
            return fileName;
        }

        public string GetPathForType(Type t)
        {
            var fromConfiguration = ConfigurationRepository.Instance.GetPathForFile(t);
            if (!string.IsNullOrEmpty(fromConfiguration))
                return Path.Combine(_settings.TargetDirectory, fromConfiguration).Replace("/", "\\");

            var ns = t.GetNamespace();
            var tn = t.GetName().ToString();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (_settings.ExportPureTypings) tn = tn + ".d.ts";
            else tn = tn + ".ts";

            if (string.IsNullOrEmpty(ns)) return Path.Combine(_settings.TargetDirectory, tn);
            if (!string.IsNullOrEmpty(_settings.RootNamespace))
            {
                ns = ns.Replace(_settings.RootNamespace, string.Empty);
            }
            ns = ns.Trim('.').Replace('.', '\\');

            var pth =
                Path.Combine(
                    !string.IsNullOrEmpty(ns) ? Path.Combine(_settings.TargetDirectory, ns) : _settings.TargetDirectory,
                    tn);

            return pth;
        }

        public string GetRelativePathForType(Type typeToReference, Type currentlyExportingType)
        {
            var currentFile = GetPathForType(currentlyExportingType);
            var desiredFile = GetPathForType(typeToReference);
            if (currentFile==desiredFile) return String.Empty;

            var desiredFileName = Path.GetFileName(desiredFile);

            var relPath = GetRelativeNamespacePath(Path.GetDirectoryName(currentFile),
                Path.GetDirectoryName(desiredFile));

            relPath = Path.Combine(relPath, desiredFileName);
            relPath = relPath.Replace('\\', '/');
            return relPath;
        }

        private string GetRelativeNamespacePath(string currentNamespace, string desiredNamespace)
        {
            if (currentNamespace == desiredNamespace) return string.Empty;
            if (string.IsNullOrEmpty(currentNamespace)) return desiredNamespace;


            var current = currentNamespace.Split('\\');
            var desired = desiredNamespace.Split('\\');

            var result = new StringBuilder();
            if (string.IsNullOrEmpty(desiredNamespace))
            {
                for (var i = 0; i < current.Length; i++) result.Append("..\\");
            }
            else
            {
                var level = current.Length - 1;
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