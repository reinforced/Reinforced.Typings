using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Reinforced.Typings.Cli;

namespace Reinforced.Typings.Cli
{
    public static class TypeExtensions
    {
        internal static PropertyInfo[] _GetProperties(this Type t, BindingFlags flags)
        {
#if NETSTANDARD15
            return t.GetTypeInfo().GetProperties(flags);
#else
            return t.GetProperties(flags);
#endif
        }
    }
}


namespace Reinforced.Typings.Integrate
{
    /// <summary>
    /// Task for gathering dynamic typings
    /// </summary>
    public class RtCli : ToolTask
    {
        /// <summary>
        /// Framework version to invoke rtcli
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// Forced usage of target framework
        /// </summary>
        public string RtForceTargetFramework { get; set; }

        /// <summary>
        /// Package's "build" directory
        /// </summary>
        [Required]
        public string BuildDirectory { get; set; }

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

        /// <summary>Projects may set this to override a task's ToolName. Tasks may override this to prevent that.</summary>
        public override string ToolExe
        {
            get; set;
        }

        protected override string GenerateFullPathToTool()
        {
            if (IsCore)
            {
#if NETCORE1
                return
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    "dotnet.exe"
                    : "dotnet";
#else
                return "dotnet.exe"; 
#endif
            }
            return Path.Combine(GetProperRtcliPath(), ToolName);
        }

        private string GetProperRtcliPath()
        {
            var bd = new DirectoryInfo(BuildDirectory);
            var toolsPath = Path.Combine(bd.Parent.FullName, "tools");
            var fwPath = Path.Combine(toolsPath, NormalizeFramework());
            return fwPath;
        }

        private string NormalizeFramework()
        {
            if (!string.IsNullOrEmpty(RtForceTargetFramework)) return RtForceTargetFramework;
            if (string.IsNullOrEmpty(TargetFramework))
#if NETCORE1
                return "netcoreapp2.0";
#else
                return "net45";
#endif

            if (TargetFramework.StartsWith("netstandard"))
            {
                var version = int.Parse(TargetFramework.Substring("netstandard".Length)[0].ToString());
                if (version == 1) return "netcoreapp1.0";
                return "netcoreapp2.0";
            }
            if (TargetFramework.StartsWith("netcoreapp")) return TargetFramework;

            if (TargetFramework.StartsWith("net46")) return "net461";
            return TargetFramework;
        }

        private bool IsCore
        {
            get
            {
                if (string.IsNullOrEmpty(TargetFramework)) return false;
                if (TargetFramework.StartsWith("netstandard")) return true;
                if (TargetFramework.StartsWith("netcoreapp")) return true;
                return false;
            }
        }


        protected override string ToolName
        {
            get
            {
                if (IsCore)
                {
#if NETCORE1
                    return
                        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                            "dotnet.exe"
                            : "dotnet";
#else
                return "dotnet.exe";
#endif
                }
                return "rtcli.exe";
            }
        }

        protected override string GenerateCommandLineCommands()
        {
            ExporterConsoleParameters consoleParams = new ExporterConsoleParameters()
            {
                Hierarchy = Hierarchical,
                TargetDirectory = FixTargetPath(TargetDirectory),
                TargetFile = FixTargetPath(TargetFile),
                ReferencesTmpFilePath = string.Empty,
                SourceAssemblies = ExtractSourceAssemblies(),
                DocumentationFilePath = DocumentationFilePath.EndsWith(".xml",
#if NETCORE1
                StringComparison.CurrentCultureIgnoreCase
#else
                StringComparison.InvariantCultureIgnoreCase
#endif
                ) ? DocumentationFilePath : String.Empty,
                ConfigurationMethod = ConfigurationMethod
            };

            var tmpFile = Path.GetTempFileName();
            using (var fs = File.OpenWrite(tmpFile))
            {
                using (var tw = new StreamWriter(fs))
                {
                    consoleParams.ToFile(tw);
                    PutReferencesToTempFile(tw);
                    tw.Flush();
                }
            }
            if (IsCore)
            {
                var pth = Path.Combine(GetProperRtcliPath(), "rtcli.dll");
                return string.Format("\"{1}\" profile \"{0}\"", tmpFile, pth);
            }
            return string.Format("profile \"{0}\"", tmpFile);

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


        private void PutReferencesToTempFile(TextWriter tw)
        {
            if (References == null) return;
            foreach (var rf in References.Select(c => c.ItemSpec))
            {
                tw.WriteLine(rf);
            }
            tw.Flush();

        }

        private string[] ExtractSourceAssemblies()
        {
            List<string> srcAssemblies = new List<string>();
            if (AdditionalSourceAssemblies != null)
            {
                srcAssemblies.AddRange(AdditionalSourceAssemblies.Select(c => c.ItemSpec));
            }
            if (SourceAssembly != null)
            {
                srcAssemblies.AddRange(SourceAssembly.Select(c => Path.Combine(ProjectRoot, c.ItemSpec)));
            }
            return srcAssemblies.ToArray();
        }

    }
}
