using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Type resolver. It is helper class to convert source types, members and parameter names to typescript ones
    /// </summary>
    public sealed class TypeResolver
    {
        private static readonly RtSimpleTypeName AnyType = new RtSimpleTypeName("any");
        private static readonly RtSimpleTypeName NumberType = new RtSimpleTypeName("number");
        private static readonly RtSimpleTypeName StringType = new RtSimpleTypeName("string");

        private readonly Dictionary<Type, RtTypeName> _resolveCache = new Dictionary<Type, RtTypeName>
        {
            {typeof (object), AnyType},
            {typeof (void), new RtSimpleTypeName("void")},
            {typeof (string), StringType},
            {typeof (char),StringType},
            {typeof (bool), new RtSimpleTypeName("boolean")},
            {typeof (byte), NumberType},
            {typeof (sbyte), NumberType},
            {typeof (short), NumberType},
            {typeof (ushort), NumberType},
            {typeof (int), NumberType},
            {typeof (uint), NumberType},
            {typeof (long), NumberType},
            {typeof (ulong), NumberType},
            {typeof (float), NumberType},
            {typeof (double), NumberType},
            {typeof (decimal), NumberType}
        };

        private readonly ExportContext _context;
        private readonly ExportedFile _file;
        

        /// <summary>
        ///     Constructs new type resolver
        /// </summary>
        internal TypeResolver(ExportContext context,ExportedFile file)
        {
            _context = context;
            _file = file;
        }

        private RtTypeName[] GetConcreteGenericArguments(Type t)
        {
            if (!t.IsGenericType) return new RtTypeName[0];
            var args = t.GetGenericArguments();
            return args.Select(ResolveTypeName).ToArray();
        }

        private RtTypeName Cache(Type t, RtTypeName name)
        {
            _resolveCache[t] = name;
            return name;
        }

        /// <summary>
        ///     Returns typescript-friendly type name node for specified type.
        ///     This method successfully handles dictionaries, IEnumerables, arrays, another TsExport-ed types, void, delegates,
        ///     most of CLR built-in types, parametrized types etc.
        ///     It also considers Ts*-attributes while resolving type names
        ///     If it cannot handle anything then it will return "any"
        /// </summary>
        /// <param name="t">Specified type</param>
        /// <returns>Typescript-friendly type name</returns>
        public RtTypeName ResolveTypeName(Type t)
        {
            try
            {
                return ResolveTypeNameInner(t);
            }
            catch (Exception ex)
            {
                ErrorMessages.RTE0005_TypeResolvationError.Throw(t.FullName, ex.Message);
                return null; // unreacheable
            }
        }


        private RtTypeName ResolveTypeNameInner(Type t)
        {
            if (_resolveCache.ContainsKey(t)) return _resolveCache[t];

            var td = ConfigurationRepository.Instance.ForType(t);
            if (td != null)
            {
                var ns = t.Namespace;
                if (!td.IncludeNamespace) ns = string.Empty;
                if (!string.IsNullOrEmpty(td.Namespace)) ns = td.Namespace;

                var result = t.GetName(GetConcreteGenericArguments(t));
                result.Prefix = ns;
                if (_context.Global.UseModules)
                {
                    return result; // do not cache type names when 
                }
                return Cache(t, result);
            }
            if (t.IsNullable())
            {
                return ResolveTypeName(t.GetArg());
            }
            if (t.IsDictionary())
            {
                if (!t.IsGenericType)
                {
                    _context.Warnings.Add(ErrorMessages.RTW0007_InvalidDictionaryKey.Warn(AnyType, t));
                    return Cache(t, new RtDictionaryType(AnyType, AnyType));
                }
                var gargs = t.GetGenericArguments();
                var key = ResolveTypeName(gargs[0]);
                if (key != NumberType && key != StringType)
                {
                    _context.Warnings.Add(ErrorMessages.RTW0007_InvalidDictionaryKey.Warn(key, t));
                }
                var value = ResolveTypeName(gargs[1]);
                return Cache(t, new RtDictionaryType(key, value));
            }
            if (t.IsNongenericEnumerable())
            {
                return Cache(t, new RtArrayType(AnyType));
            }
            if (t.IsEnumerable())
            {
                if (t.IsArray)
                {
                    return Cache(t, new RtArrayType(ResolveTypeName(t.GetElementType())));
                }
                var enumerable =
                    t.GetInterfaces()
                        .FirstOrDefault(c => c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (enumerable == null)
                {
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) enumerable = t;
                }
                if (enumerable == null) return Cache(t, new RtArrayType(AnyType));
                return Cache(t, new RtArrayType(ResolveTypeName(enumerable.GetArg())));
            }

            if (t.IsGenericParameter)
            {
                var genAt = t.GetCustomAttribute<TsGenericAttribute>(false);
                if (genAt != null)
                {
                    if (genAt.StrongType != null) return Cache(t, ResolveTypeName(genAt.StrongType));
                    if (genAt.Type != null) return Cache(t, new RtSimpleTypeName(genAt.Type));
                }
                return Cache(t, new RtSimpleTypeName(t.Name));
            }

            if (typeof(MulticastDelegate).IsAssignableFrom(t.BaseType))
            {
                var methodInfo = t.GetMethod("Invoke");
                return Cache(t, ConstructFunctionType(methodInfo));
            }


            if (t.IsGenericType && !t.IsGenericTypeDefinition)
            {
                var def = t.GetGenericTypeDefinition();
                var tsFriendly = ResolveTypeNameInner(def) as RtSimpleTypeName;
                if (tsFriendly != null && tsFriendly != AnyType)
                {
                    var parametrized = new RtSimpleTypeName(tsFriendly.TypeName,
                        t.GetGenericArguments().Select(ResolveTypeNameInner).ToArray())
                    {
                        Prefix = tsFriendly.Prefix
                    };
                    return Cache(t, parametrized);
                }

            }

            _context.Warnings.Add(ErrorMessages.RTW0003_TypeUnknown.Warn(t.FullName));

            return Cache(t, AnyType);
        }

        internal void PrintCacheInfo()
        {
            Console.WriteLine("Types resolving cache contains {0} entries", _resolveCache.Count);
        }

        private RtDelegateType ConstructFunctionType(MethodInfo methodInfo)
        {
            var retType = ResolveTypeName(methodInfo.ReturnType);
            var result = retType;

            var argAggreagtor = 0;
            List<RtArgument> arguments = new List<RtArgument>();
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var argName = argAggreagtor > 0 ? "arg" + argAggreagtor : "arg";
                var typeName = ResolveTypeName(parameterInfo.ParameterType);
                arguments.Add(new RtArgument() { Identifier = new RtIdentifier(argName), Type = typeName });
                argAggreagtor++;
            }

            return new RtDelegateType(arguments.ToArray(), result);
        }
    }
}