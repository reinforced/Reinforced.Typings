using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        interface User
        {
            int Id { get; }

            string Name { get; }

            FSharpMap<string,int> AccessRights { get; }

            FSharpMap<int, FSharpMap<string,string>> Privilegies { get; }
        }

        [Fact]
        public void ADIXReadonlyDictionaryWorkaround()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
	export interface IUser
	{
		Id: number;
		Name: string;
		AccessRights: { [key:string]: number };
		Privilegies: { [key:number]: { [key:string]: string } };
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());

                s.SubstituteGeneric(typeof(FSharpMap<,>), (t, tr) =>
                {
                    var args = t.GetGenericArguments();
                    return new RtDictionaryType(tr.ResolveTypeName(args[0]),tr.ResolveTypeName(args[1]));
                });

                s.ExportAsInterface<User>().WithAllProperties();
            }, result);
        }
    }

    /// <summary>
    /// Simulate F# map
    /// </summary>
    class FSharpMap<T, V> : IReadOnlyDictionary<T, V>
    {
       
        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get; }

        public bool ContainsKey(T key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(T key, out V value)
        {
            throw new System.NotImplementedException();
        }

        public V this[T key]
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<T> Keys { get; }

        public IEnumerable<V> Values { get; }
    }
}