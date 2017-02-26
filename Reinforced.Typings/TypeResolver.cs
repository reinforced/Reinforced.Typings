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
        private readonly ITsCodeGenerator<Type> _defaultClassGenerator;
        private readonly ITsCodeGenerator<Type> _defaultEnumGenerator;
        private readonly Dictionary<MemberTypes, object> _defaultGenerators = new Dictionary<MemberTypes, object>();
        private readonly ITsCodeGenerator<Type> _defaultInterfaceGenerator;
        private readonly NamespaceCodeGenerator _defaultNsgenerator;
        private readonly ITsCodeGenerator<ParameterInfo> _defaultParameterGenerator;
        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

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

        public ExportedFile CurrentFile { get; internal set; }

        /// <summary>
        ///     Constructs new type resolver
        /// </summary>
        internal TypeResolver(ExportContext context)
        {
            _defaultGenerators[MemberTypes.Property] = new PropertyCodeGenerator { Context = context };
            _defaultGenerators[MemberTypes.Field] = new FieldCodeGenerator { Context = context };
            _defaultGenerators[MemberTypes.Method] = new MethodCodeGenerator { Context = context };
            _defaultGenerators[MemberTypes.Constructor] = new ConstructorCodeGenerator { Context = context };
            _defaultParameterGenerator = new ParameterCodeGenerator { Context = context };
            _defaultClassGenerator = new ClassCodeGenerator { Context = context };
            _defaultInterfaceGenerator = new InterfaceCodeGenerator { Context = context };
            _defaultEnumGenerator = new EnumGenerator { Context = context };
            _defaultNsgenerator = new NamespaceCodeGenerator { Context = context };
            _context = context;
        }

        /// <summary>
        ///     Reteieves code generator instance for specified type member.
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <typeparam name="T">Type member info type</typeparam>
        /// <param name="member">Type member info</param>
        /// <param name="context">Export settings</param>
        /// <returns>Code generator for specified type member</returns>
        public ITsCodeGenerator<T> GeneratorFor<T>(T member, ExportContext context)
            where T : MemberInfo
        {
            var attr = ConfigurationRepository.Instance.ForMember<TsTypedMemberAttributeBase>(member);
            var fromAttr = GetFromAttribute<T>(attr, context);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var decType = member.DeclaringType;
                var classAttr = ConfigurationRepository.Instance.ForType<TsClassAttribute>(decType);
                if (classAttr != null && classAttr.DefaultMethodCodeGenerator != null)
                {
                    return LazilyInstantiateGenerator<T>(classAttr.DefaultMethodCodeGenerator, context);
                }
            }
            var gen = (ITsCodeGenerator<T>)_defaultGenerators[member.MemberType];
            gen.Context = context;
            return gen;
        }

        /// <summary>
        ///     Retrieves code generator for ParameterInfo (since ParameterInfo does not derive from MemberInfo).
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Parameter info</param>
        /// <param name="context">Export settings</param>
        /// <returns>Code generator for parameter info</returns>
        public ITsCodeGenerator<ParameterInfo> GeneratorFor(ParameterInfo member, ExportContext context)
        {
            var attr = ConfigurationRepository.Instance.ForMember(member);
            var fromAttr = GetFromAttribute<ParameterInfo>(attr, context);
            if (fromAttr != null) return fromAttr;
            return _defaultParameterGenerator;
        }

        /// <summary>
        ///     Retrieves code generator for specified type
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Type info</param>
        /// <param name="context">Export settings</param>
        /// <returns>Code generator for specified type</returns>
        public ITsCodeGenerator<Type> GeneratorFor(Type member, ExportContext context)
        {
            var attr = ConfigurationRepository.Instance.ForType(member);
            var fromAttr = GetFromAttribute<Type>(attr, context);
            if (fromAttr != null) return fromAttr;

            var isClass = attr is TsClassAttribute;
            var isInterface = attr is TsInterfaceAttribute;
            var isEnum = attr is TsEnumAttribute;

            if (isClass) return _defaultClassGenerator;
            if (isInterface) return _defaultInterfaceGenerator;
            if (isEnum) return _defaultEnumGenerator;
            return null;
        }

        /// <summary>
        ///     Retrieves code generator for namespaces
        /// </summary>
        /// <returns></returns>
        public NamespaceCodeGenerator GeneratorForNamespace(ExportContext context)
        {
            _defaultNsgenerator.Context = context;
            return _defaultNsgenerator;
        }

        private ITsCodeGenerator<T> GetFromAttribute<T>(TsAttributeBase attr, ExportContext context)
        {
            if (attr != null)
            {
                var t = attr.CodeGeneratorType;
                if (t != null) return LazilyInstantiateGenerator<T>(t, context);
            }
            return null;
        }

        private ITsCodeGenerator<T> LazilyInstantiateGenerator<T>(Type generatorType, ExportContext context)
        {
            lock (_generatorsCache)
            {
                if (!_generatorsCache.ContainsKey(generatorType))
                {
                    try
                    {
                        _generatorsCache[generatorType] = Activator.CreateInstance(generatorType);
                        var gen = (ITsCodeGenerator<T>)_generatorsCache[generatorType];
                        gen.Context = context;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0003_GeneratorInstantiate.Throw(generatorType.FullName, ex.Message);
                    }
                }
                return (ITsCodeGenerator<T>)_generatorsCache[generatorType];
            }
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