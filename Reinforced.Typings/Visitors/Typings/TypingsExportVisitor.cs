using System.IO;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    /// <summary>
    /// Visitor that is generating .d.ts from existing model
    /// </summary>
    public partial class TypingsExportVisitor : TypeScriptExportVisitor
    {
        public override void Visit(RtDecorator node)
        {
            //no decorators allowed in the .d.ts
        }


        public TypingsExportVisitor(TextWriter writer, ExportContext exportContext) : base(writer, exportContext)
        {
        }
    }
}
