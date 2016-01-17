using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public interface ITypeMember
    {
        RtJsdocNode Documentation { get; set; }
        List<RtMember> Members { get; }
        RtSimpleTypeName Name { get; set; }
        bool NeedsExports { get; set; }
        List<RtSimpleTypeName> Implementees { get; }
    }
}
