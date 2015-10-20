using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Fluent.Interfaces
{
    public interface IReferenceConfiguration
    {
        ICollection<TsAddTypeReferenceAttribute> References { get; }
    }
}
