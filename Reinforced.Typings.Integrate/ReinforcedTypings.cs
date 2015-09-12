using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Reinforced.Typings.Integrate
{
    public class ReinforcedTypings : Task
    {
        [Required]
        public string TargetFilePath { get; set; }

        public override bool Execute()
        {
            return true;
        }
    }
}
