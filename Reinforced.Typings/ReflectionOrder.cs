using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reinforced.Typings
{
    internal static class ReflectionOrder
    {
        internal static readonly StringComparer StringComparer = StringComparer.Ordinal;

        internal static string TypeKey(Type type)
        {
            if (type == null) return string.Empty;

            var asm = type.GetTypeInfo().Assembly.GetName().Name;
            return string.Concat(asm, ":", type.FullName ?? type.Name);
        }

        internal static string MemberKey(MemberInfo member)
        {
            if (member == null) return string.Empty;

            var method = member as MethodBase;
            var parameters = method == null
                ? string.Empty
                : string.Join(",", method.GetParameters().Select(ParameterKey).ToArray());

            return string.Concat(
                TypeKey(member.DeclaringType),
                ":",
                MemberRank(member),
                ":",
                member.Name,
                "(",
                parameters,
                ")");
        }

        internal static IEnumerable<T> OrderTypes<T>(IEnumerable<T> types) where T : Type
        {
            return types.OrderBy(TypeKey, StringComparer);
        }

        internal static IEnumerable<T> OrderMembers<T>(IEnumerable<T> members) where T : MemberInfo
        {
            return members
                .OrderBy(MetadataTokenOrMax)
                .ThenBy(MemberKey, StringComparer);
        }

        private static string ParameterKey(ParameterInfo parameter)
        {
            if (parameter == null) return string.Empty;
            var parameterType = parameter.ParameterType;
            return string.Concat(
                parameter.Position,
                ":",
                parameter.Name ?? string.Empty,
                ":",
                TypeKey(parameterType));
        }

        private static int MemberRank(MemberInfo member)
        {
            if (member is ConstructorInfo) return 0;
            if (member is FieldInfo) return 1;
            if (member is PropertyInfo) return 2;
            if (member is MethodInfo) return 3;
            return 4;
        }

        private static int MetadataTokenOrMax(MemberInfo member)
        {
            try
            {
                return member.MetadataToken;
            }
            catch (InvalidOperationException)
            {
                return int.MaxValue;
            }
        }
    }
}
