using System;
using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Tests
{
    public class TypeNameEqualityComparer : IEqualityComparer<RtTypeName>
    {
        public bool Equals(RtTypeName x, RtTypeName y)
        {
            if (x.GetType() != y.GetType()) return false;
            if (x is RtSimpleTypeName) return CompareSimple((RtSimpleTypeName)x, (RtSimpleTypeName)y);
            if (x is RtArrayType) return CompareArray((RtArrayType)x, (RtArrayType)y);
            if (x is RtDelegateType) return CompareDelegate((RtDelegateType)x, (RtDelegateType)y);
            if (x is RtDictionaryType) return CompareDictionary((RtDictionaryType)x, (RtDictionaryType)y);
            if (x is RtTuple) return CompareTuple((RtTuple)x, (RtTuple)y);
            throw new Exception(x.GetType().FullName + " is not valid type for comparison");
        }

        private bool CompareSimple(RtSimpleTypeName x, RtSimpleTypeName y)
        {
            if (x.TypeName != y.TypeName) return false;
            if (x.Prefix != y.Prefix) return false;
            if (x.GenericArguments.Length != y.GenericArguments.Length) return false;

            for (int i = 0; i < x.GenericArguments.Length; i++)
            {
                if (!Equals(x.GenericArguments[i], y.GenericArguments[i])) return false;
            }

            return true;
        }
        private bool CompareArray(RtArrayType x, RtArrayType y)
        {
            return Equals(x.ElementType, y.ElementType);
        }

        private bool CompareTuple(RtTuple x, RtTuple y)
        {
            if (x.TupleTypes.Count != y.TupleTypes.Count) return false;

            for (int i = 0; i < x.TupleTypes.Count; i++)
            {
                if (!Equals(x.TupleTypes[i], y.TupleTypes[i])) return false;
            }
            return true;
        }

        private bool CompareDelegate(RtDelegateType x, RtDelegateType y)
        {
            if (x.Arguments.Length != y.Arguments.Length) return false;
            if (!Equals(x.Result, y.Result)) return false;
            for (int i = 0; i < x.Arguments.Length; i++)
            {
                if (!Equals(x.Arguments[i], y.Arguments[i])) return false;
            }
            return true;
        }
        private bool CompareDictionary(RtDictionaryType x, RtDictionaryType y)
        {
            if (!Equals(x.KeyType, y.KeyType)) return false;
            return Equals(x.ValueType, y.ValueType);
        }

        public int GetHashCode(RtTypeName obj)
        {
            return obj.GetHashCode();
        }
    }
}