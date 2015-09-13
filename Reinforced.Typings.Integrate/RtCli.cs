using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Reinforced.Typings.Integrate
{
    public class RtCli : ToolTask
    {
        [Required]
        public ITaskItem[] RtCliPath { get; set; }

        [Required]
        public string TargetFilePath { get; set; }

        [Required]
        public string ProjectTargetPath { get; set; }

        public ITaskItem[] AdditionalDlls { get; set; }

        protected override string GenerateFullPathToTool()
        {
            var rtcli = RtCliPath[0];
            var path = rtcli.ItemSpec;
            return path;
        }

        protected override string ToolName
        {
            get { return "rtcli.exe"; }
        }

        protected override string GenerateCommandLineCommands()
        {
            var pathes = String.Join(" ", ExtractDllsPathes());
            var target = TargetFilePath;

            return string.Concat(String.Join(" ", pathes), " ", target);
        }

        private string[] ExtractDllsPathes()
        {
            var dir = Path.GetDirectoryName(ProjectTargetPath);
            List<string> assemblyPathes = new List<string>() { ProjectTargetPath };
            if (AdditionalDlls == null) return assemblyPathes.ToArray();
            foreach (var additionalDll in AdditionalDlls)
            {
                var path = additionalDll.ItemSpec;
                var items = path.Contains(";") ? path.Split(';') : new string[] { path };
                foreach (var pth in items)
                {
                    assemblyPathes.Add(Path.Combine(dir, pth));
                }
            }
            return assemblyPathes.ToArray();
        }
    }
}
