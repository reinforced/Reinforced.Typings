using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace Reinforced.Typings.Integrate
{
    public class RemoveTypescriptStep : ITask
    {
        [Output]
        public string[] Fixed { get; set; }

        [Required]
        public string Original { get; set; }

        /// <summary>Executes a task.</summary>
        /// <returns>true if the task executed successfully; otherwise, false.</returns>
        public bool Execute()
        {
            Console.WriteLine("RT is fixing build tasks");
            string[] targets = Original.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> result = new List<string>();
            foreach (string s in targets)
            {
                string v = s.Trim();
                bool isCompileTs = (v == "CompileTypeScript");
                bool isCompileTsConfig = (v == "CompileTypeScriptWithTSConfig");
                bool isGetTypeScriptOutputForPublishing = (v == "GetTypeScriptOutputForPublishing");
                if ((!isCompileTs) && (!isCompileTsConfig) && (!isGetTypeScriptOutputForPublishing))
                {
                    result.Add(v);
                }
                else
                {
                    Console.WriteLine(v + " task will be removed");
                }
            }
            Fixed = result.ToArray();
            Console.WriteLine("RT has fixed build tasks:" + string.Join(";", Fixed));
            return true;
        }

        /// <summary>Gets or sets the build engine associated with the task.</summary>
        /// <returns>The build engine associated with the task.</returns>
        public IBuildEngine BuildEngine { get; set; }

        /// <summary>Gets or sets any host object that is associated with the task.</summary>
        /// <returns>The host object associated with the task.</returns>
        public ITaskHost HostObject { get; set; }
    }
}
