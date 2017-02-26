using System;
using System.IO;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;

namespace Reinforced.Typings.Visitors.Typings
{
    public partial class TypingsExportVisitor : TypeScriptExportVisitor
    {
        public TypingsExportVisitor(TextWriter writer, string tabulation)
            : base(writer,tabulation)
        {
        }
    }
}
