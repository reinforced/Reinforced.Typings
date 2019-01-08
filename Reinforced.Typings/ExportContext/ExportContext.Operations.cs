using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable CheckNamespace
namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public partial class ExportContext
    {
        internal void Lock()
        {
            _isLocked = true;
            Global.Lock();
        }

        internal void Unlock()
        {
            _isLocked = false;
            Global.Unlock();
        }

        /// <summary>
        /// Retrieves full path to file where specified type will be exported to
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="stripExtension">Remove file extension. Set to false if you still want to get path with extension in case of module export</param>
        /// <returns>Full path to file containing exporting type</returns>
        internal string GetPathForType(Type t, bool stripExtension = true)
        {
            var fromConfiguration = Project.GetPathForFile(t);
            if (!string.IsNullOrEmpty(fromConfiguration))
            {
                if (Global.UseModules && stripExtension)
                {
                    if (fromConfiguration.EndsWith(".d.ts"))
                        fromConfiguration = fromConfiguration.Substring(0, fromConfiguration.Length - 5);
                    if (fromConfiguration.EndsWith(".ts"))
                        fromConfiguration = fromConfiguration.Substring(0, fromConfiguration.Length - 3);
                }
                var r = Path.Combine(TargetDirectory, fromConfiguration);
                return r;
            }

            var ns = Project.Blueprint(t).GetNamespace();
            var tn = Project.Blueprint(t).GetName().ToString();

            var idx = tn.IndexOf('<');
            if (idx != -1) tn = tn.Substring(0, idx);
            if (!Global.UseModules || !stripExtension)
            {
                if (Global.ExportPureTypings) tn = tn + ".d.ts";
                else tn = tn + ".ts";
            }

            if (string.IsNullOrEmpty(ns)) return Path.Combine(TargetDirectory, tn);
            if (!string.IsNullOrEmpty(Global.RootNamespace))
            {
                ns = ns.Replace(Global.RootNamespace, string.Empty);
            }
            ns = ns.Trim('.').Replace('.', Path.DirectorySeparatorChar);

            var pth =
                Path.Combine(
                    !string.IsNullOrEmpty(ns) ? Path.Combine(TargetDirectory, ns) : TargetDirectory,
                    tn);

            return pth;
        }

        /// <summary>
        /// Sets up exported file dummy
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Exported file dummy</returns>
        public ExportedFile CreateExportedFile(string fileName = null)
        {
            if (!Hierarchical && fileName == TargetFile) fileName = null;
            IEnumerable<Type> types = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!TypesToFilesMap.ContainsKey(fileName))
                {
                    var allFiles = string.Join(", ", TypesToFilesMap.Keys);
                    throw new Exception("Current configuration does not contain file " + fileName + ", only " + allFiles);
                }

                types = new HashSet<Type>(TypesToFilesMap[fileName]);
            }
            else
            {
                types = _allTypesHash;
            }
            var typesHash = new HashSet<Type>(types.Where(d => Project.Blueprint(d).ThirdParty == null));

            ExportedFile ef = new ExportedFile(typesHash, fileName, _globalReferences.Duplicate(), this);
            return ef;
        }
    }
}