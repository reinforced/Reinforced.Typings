using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.Dependency;

namespace Reinforced.Typings.ReferencesInspection
{
    sealed class ImportComparer : IEqualityComparer<RtImport>
    {
        public bool Equals(RtImport x, RtImport y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Target, y.Target) && string.Equals(x.From, y.From) && x.IsRequire == y.IsRequire;
        }

        public int GetHashCode(RtImport obj)
        {
            unchecked
            {
                var hashCode = (obj.Target != null ? obj.Target.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.From != null ? obj.From.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.IsRequire.GetHashCode();
                return hashCode;
            }
        }

        private static readonly ImportComparer _instance = new ImportComparer();

        public static ImportComparer Instance
        {
            get { return _instance; }
        }
    }
}
