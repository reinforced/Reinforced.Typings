using System;
using System.Collections.Generic;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings
{
    internal static class DictionaryExtensions
    {
        public static TV GetOrNull<T, TV>(this Dictionary<T, TV> dictionary, T key)
        {
            if (!dictionary.ContainsKey(key)) return default(TV);
            return dictionary[key];
        }
        public static TV GetOr<T, TV>(this Dictionary<T, TV> dictionary, T key,Func<TV> or)
        {
            if (!dictionary.ContainsKey(key))
            {
                return or();
            }
            return dictionary[key];
        }

        public static TV GetOrCreate<T, TV>(this Dictionary<T, TV> dictionary, T key) where TV : new()
        {
            TV result;
            if (dictionary.ContainsKey(key)) result = dictionary[key];
            else
            {
                result = new TV();
                dictionary[key] = result;
            }
            return result;
        }

        public static TV GetOrCreate<T, TV>(this Dictionary<T, TV> dictionary, T key, Func<TV> createDelegate)
        {
            TV result;
            if (dictionary.ContainsKey(key)) result = dictionary[key];
            else
            {
                result = createDelegate();
                dictionary[key] = result;
            }
            return result;
        }

        public static bool CheckIgnored<T>(this IExportConfiguration<T> conf)
        {
            var ignore = conf as IIgnorable;
            if (ignore == null) return false;
            return ignore.Ignore;
        }
    }
}