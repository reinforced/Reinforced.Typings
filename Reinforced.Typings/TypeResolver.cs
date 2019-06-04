using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;

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

        /// <summary>
        /// Hash set of all numeric types
        /// </summary>
        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new[]
        {
            typeof (byte),
            typeof (sbyte),
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
            typeof (float),
            typeof (double),
            typeof (decimal),
            typeof (byte?),
            typeof (sbyte?),
            typeof (short?),
            typeof (ushort?),
            typeof (int?),
            typeof (uint?),
            typeof (long?),
            typeof (ulong?),
            typeof (float?),
            typeof (double?),
            typeof (decimal?)
        });

        /// <summary>
        /// Hash set of all integer types
        /// </summary>
        public static readonly HashSet<Type> IntegerTypes = new HashSet<Type>(new[]
        {
            typeof (byte),
            typeof (sbyte),
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
            typeof (byte?),
            typeof (sbyte?),
            typeof (short?),
            typeof (ushort?),
            typeof (int?),
            typeof (uint?),
            typeof (long?),
            typeof (ulong?)
        });

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

        private ExportContext Context
        {
            get { return _file.Context; }
        }

        private readonly ExportedFile _file;

        /// <summary>
        ///     Constructs new type resolver
        /// </summary>
        internal TypeResolver(ExportedFile file)
        {
            _file = file;
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
            var substitution = Context.Project.Substitute(t, this);
            if (substitution != null) return substitution; // order important!

            if (Context.CurrentBlueprint != null)
            {
                var localSubstitution = Context.CurrentBlueprint.Substitute(t, this);
                if (localSubstitution != null) return localSubstitution;
            }
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

            var bp = Context.Project.Blueprint(t, false);
            if (bp != null && bp.ThirdParty != null)
            {
                var result = bp.GetName(bp.IsExportedExplicitly ? null : GetConcreteGenericArguments(t, materializedGenerics));
                if (Context.Global.UseModules) _file.EnsureImport(t, result.TypeName);
                _file.EnsureReference(t);
                return Cache(t, result);
            }
            else
            {
                var declaration = bp == null ? null : bp.TypeAttribute;
                if (declaration != null)
                {
                    var ns = t.Namespace;
                    if (!string.IsNullOrEmpty(declaration.Namespace)) ns = declaration.Namespace;
                    if (!declaration.IncludeNamespace) ns = string.Empty;
                    var result = bp.GetName(bp.IsExportedExplicitly ? null : GetConcreteGenericArguments(t, materializedGenerics));

                    if (Context.Global.UseModules)
                    {
                        var import = _file.EnsureImport(t, result.TypeName);
                        if (Context.Global.DiscardNamespacesWhenUsingModules) ns = string.Empty;
                        if (import == null || !import.IsWildcard)
                        {
                            result.Prefix = ns;
                            return Cache(t, result);
                        }

                        result.Prefix = string.IsNullOrEmpty(ns)
                            ? import.WildcardAlias
                            : string.Format("{0}.{1}", import.WildcardAlias, ns);
                        return Cache(t, result);
                    }
                    else
                    {
                        _file.EnsureReference(t);
                        if (!string.IsNullOrEmpty(declaration.Namespace)) ns = declaration.Namespace;
                        result.Prefix = ns;
                        return Cache(t, result);
                    }
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
                    Context.Warnings.Add(ErrorMessages.RTW0007_InvalidDictionaryKey.Warn(AnyType, t));
                    return Cache(t, new RtDictionaryType(AnyType, AnyType));
                }
                var gargs = t._GetGenericArguments();
                bool isKeyEnum = gargs[0]._IsEnum();
                var key = ResolveTypeName(gargs[0]);
                if (key != NumberType && key != StringType && !isKeyEnum)
                {
                    Context.Warnings.Add(ErrorMessages.RTW0007_InvalidDictionaryKey.Warn(key, t));
                }
                var value = ResolveTypeName(gargs[1]);
                return Cache(t, new RtDictionaryType(key, value, isKeyEnum));
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
                return Cache(t, ConstructFunctionType(methodInfo, materializedGenerics));
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

            if (t._IsEnum())
            {
                return Cache(t, NumberType);
            }

            Context.Warnings.Add(ErrorMessages.RTW0003_TypeUnknown.Warn(t.FullName));

            return Cache(t, AnyType);
        }

        private RtDelegateType ConstructFunctionType(MethodInfo methodInfo, Dictionary<string, RtTypeName> materializedGenerics = null)
        {
            var retType = ResolveTypeName(methodInfo.ReturnType, materializedGenerics);
            var result = retType;

            var argAggreagtor = 0;
            List<RtArgument> arguments = new List<RtArgument>();
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var argName = argAggreagtor > 0 ? "arg" + argAggreagtor : "arg";
                var typeName = ResolveTypeName(parameterInfo.ParameterType, materializedGenerics);
                arguments.Add(new RtArgument() { Identifier = new RtIdentifier(argName), Type = typeName });
                argAggreagtor++;
            }

            return new RtDelegateType(arguments.ToArray(), result);
        }
    }
}