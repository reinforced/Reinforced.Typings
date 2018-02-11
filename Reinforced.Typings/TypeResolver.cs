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
using Reinforced.Typings.ReferencesInspection;

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
        private readonly ReferenceInspector _refInspector;

        /// <summary>
        ///     Constructs new type resolver
        /// </summary>
        internal TypeResolver(ExportContext context, ExportedFile file, ReferenceInspector refInspector)
        {
            _context = context;
            _file = file;
            _refInspector = refInspector;
        }

        private RtTypeName[] GetConcreteGenericArguments(Type t, Dictionary<string, RtTypeName> materializedGenerics = null)
        {
            if (!t._IsGenericType()) return new RtTypeName[0];
            var args = t._GetGenericArguments();
            if (materializedGenerics == null) return args.Select(ResolveTypeName).ToArray();
            else
            {
                List<RtTypeName> result = new List<RtTypeName>();
                foreach (var type in args)
                {
                    if (materializedGenerics.ContainsKey(type.Name)) result.Add(materializedGenerics[type.Name]);
                    else result.Add(ResolveTypeName(type));
                }
                return result.ToArray();
            }
        }

        private RtTypeName Cache(Type t, RtTypeName name)
        {
            _resolveCache[t] = name;
            return name;
        }

        internal RtTypeName ResolveTypeName(Type t, Dictionary<string, RtTypeName> materializedGenerics)
        {
            if (materializedGenerics == null) return ResolveTypeName(t);

            try
            {
                return ResolveTypeNameInner(t, materializedGenerics);
            }
            catch (Exception ex)
            {
                ErrorMessages.RTE0005_TypeResolvationError.Throw(t.FullName, ex.Message);
                return null; // unreacheable
            }
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


        internal RtTypeName ResolveTypeNameInner(Type t, Dictionary<string, RtTypeName> materializedGenerics = null)
        {
            var substitution = t.Substitute(_context.Location.CurrentType);
            if (substitution != null) return substitution; // order important!

            if (t.IsGenericParameter)
            {
                var genAt = t.GetCustomAttribute<TsGenericAttribute>(false);
                if (genAt != null)
                {
                    if (genAt.StrongType != null) return ResolveTypeName(genAt.StrongType);
                    if (genAt.Type != null) return new RtSimpleTypeName(genAt.Type);
                }
                return new RtSimpleTypeName(t.Name);
            }

            if (materializedGenerics == null && _resolveCache.ContainsKey(t)) return _resolveCache[t];

            var td = ConfigurationRepository.Instance.ForType(t);
            if (td != null)
            {
                var ns = t.Namespace;
                if (!td.IncludeNamespace) ns = string.Empty;
                var result = t.GetName(GetConcreteGenericArguments(t, materializedGenerics));

                if (_context.Global.UseModules)
                {
                    var import = _refInspector.EnsureImport(t, result.TypeName, _file);
                    if (_context.Global.DiscardNamespacesWhenUsingModules) ns = string.Empty;
                    if (import == null || !import.IsWildcard)
                    {
                        result.Prefix = ns;
                        return Cache(t, result);
                    }

                    result.Prefix = string.IsNullOrEmpty(ns) ? import.WildcardAlias : string.Format("{0}.{1}", import.WildcardAlias, ns);
                    return Cache(t, result);
                }
                else
                {
                    _refInspector.EnsureReference(t, _file);
                    if (!string.IsNullOrEmpty(td.Namespace)) ns = td.Namespace;
                    result.Prefix = ns;
                    return Cache(t, result);
                }
            }
            if (t.IsNullable())
            {
                return ResolveTypeName(t.GetArg());
            }
            if (t.IsTuple())
            {
                var args = t._GetGenericArguments().Select(ResolveTypeName);
                return Cache(t, new RtTuple(args));
            }
            if (t.IsDictionary())
            {
                if (!t._IsGenericType())
                {
                    _context.Warnings.Add(ErrorMessages.RTW0007_InvalidDictionaryKey.Warn(AnyType, t));
                    return Cache(t, new RtDictionaryType(AnyType, AnyType));
                }
                var gargs = t._GetGenericArguments();
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
                    t._GetInterfaces()
                        .FirstOrDefault(c => c._IsGenericType() && c.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (enumerable == null)
                {
                    if (t._IsGenericType() && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) enumerable = t;
                }
                if (enumerable == null) return Cache(t, new RtArrayType(AnyType));
                return Cache(t, new RtArrayType(ResolveTypeName(enumerable.GetArg())));
            }
            
            if (typeof(MulticastDelegate)._IsAssignableFrom(t._BaseType()))
            {
                var methodInfo = t._GetMethod("Invoke");
                return Cache(t, ConstructFunctionType(methodInfo));
            }


            if (t._IsGenericType() && !t._IsGenericTypeDefinition())
            {
                var def = t.GetGenericTypeDefinition();
                var tsFriendly = ResolveTypeNameInner(def) as RtSimpleTypeName;
                if (tsFriendly != null && tsFriendly != AnyType)
                {
                    var parametrized = new RtSimpleTypeName(tsFriendly.TypeName,
                        t._GetGenericArguments().Select(c => ResolveTypeNameInner(c, null)).ToArray())
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