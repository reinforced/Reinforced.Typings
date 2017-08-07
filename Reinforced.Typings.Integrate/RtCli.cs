using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Reinforced.Typings.Cli;

namespace Reinforced.Typings.Integrate
{
    /// <summary>
    /// Task for gathering dynamic typings
    /// </summary>
    public class RtCli : ToolTask
    {
        /// <summary>
        /// Path to rtcli.exe
        /// </summary>
        [Required]
        public string RtCliPath { get; set; }

        /// <summary>
        /// Additional library references
        /// </summary>
        [Required]
        public ITaskItem[] References { get; set; }

        /// <summary>
        /// Source assembly
        /// </summary>
        [Required]
        public ITaskItem[] SourceAssembly { get; set; }

        /// <summary>
        /// Target .td/.d.ts file
        /// </summary>
        public string TargetFile { get; set; }

        /// <summary>
        /// Target directory for hierarchical generation
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Generate types to multiple files
        /// </summary>
        public bool Hierarchical { get; set; }

       /// <summary>
        /// Additional source assemblies to import
        /// </summary>
        public ITaskItem[] AdditionalSourceAssemblies { get; set; }

        /// <summary>
        /// ProjectDir variable
        /// </summary>
        public string ProjectRoot { get; set; }

        /// <summary>
        /// Path to documentation XML
        /// </summary>
        public string DocumentationFilePath { get; set; }

      
        /// <summary>
        /// Full-qualified name of fluent configuration method
        /// </summary>
        public string ConfigurationMethod { get; set; }
        
        protected override string GenerateFullPathToTool()
        {
            return RtCliPath;
        }

        protected override string ToolName
        {
            get { return "rtcli.exe"; }
        }

        protected override string GenerateCommandLineCommands()
        {
            ExporterConsoleParameters consoleParams = new ExporterConsoleParameters()
            {
                Hierarchy = Hierarchical,
                TargetDirectory = FixTargetPath(TargetDirectory),
                TargetFile = FixTargetPath(TargetFile),
                ReferencesTmpFilePath = PutReferencesToTempFile(),
                SourceAssemblies = ExtractSourceAssemblies(),
                DocumentationFilePath = DocumentationFilePath.EndsWith(".xml",StringComparison.InvariantCultureIgnoreCase)?DocumentationFilePath:String.Empty,
                ConfigurationMethod = ConfigurationMethod
            };

            return consoleParams.ExportConsoleParameters();
        }

        private string FixTargetPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (!Path.IsPathRooted(path))
                {
                    return Path.Combine(ProjectRoot, path);
                }
            }
            return path;
        }


        private string PutReferencesToTempFile()
        {
            if (References==null) return string.Empty;
            var tmpFile = Path.GetTempFileName();
            using (var fs = File.OpenWrite(tmpFile))
            {
                using (var tw = new StreamWriter(fs))
                {
                    foreach (var rf in References.Select(c => c.ItemSpec))
                    {
                        tw.WriteLine(rf);
                    }
                    tw.Flush();
                }
            }
            return tmpFile;
        }

        private string[] ExtractSourceAssemblies()
        {
            List<string> srcAssemblies = new List<string>();
            if (AdditionalSourceAssemblies != null)
            {
                srcAssemblies.AddRange(AdditionalSourceAssemblies.Select(c=>c.ItemSpec));
            }
            if (SourceAssembly != null)
            {
                srcAssemblies.AddRange(SourceAssembly.Select(c => Path.Combine(ProjectRoot, c.ItemSpec)));
            }
            return srcAssemblies.ToArray();
        }
    }
}
