using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Type resolver. It is helper class to convert source types, members and parameter names to typescript ones
    /// </summary>
    public class TypeResolver
    {
        private readonly ITsCodeGenerator<Type, RtClass> _defaultClassGenerator;
        private readonly ITsCodeGenerator<Type, RtEnum> _defaultEnumGenerator;
        private readonly Dictionary<MemberTypes, object> _defaultGenerators = new Dictionary<MemberTypes, object>();
        private readonly ITsCodeGenerator<Type, RtInterface> _defaultInterfaceGenerator;
        private readonly NamespaceCodeGenerator _defaultNsgenerator;
        private readonly ITsCodeGenerator<ParameterInfo, RtArgument> _defaultParameterGenerator;
        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

        private static readonly RtSimpleTypeName AnyType = new RtSimpleTypeName("any");

        private readonly Dictionary<Type, RtTypeName> _resolveCache = new Dictionary<Type, RtTypeName>
        {
            {typeof (object), AnyType},
            {typeof (void), new RtSimpleTypeName("void")},
            {typeof (string), new RtSimpleTypeName("string")},
            {typeof (char), new RtSimpleTypeName("string")},
            {typeof (bool), new RtSimpleTypeName("boolean")},
            {typeof (byte), new RtSimpleTypeName("number")},
            {typeof (sbyte), new RtSimpleTypeName("number")},
            {typeof (short), new RtSimpleTypeName("number")},
            {typeof (ushort), new RtSimpleTypeName("number")},
            {typeof (int), new RtSimpleTypeName("number")},
            {typeof (uint), new RtSimpleTypeName("number")},
            {typeof (long), new RtSimpleTypeName("number")},
            {typeof (ulong), new RtSimpleTypeName("number")},
            {typeof (float), new RtSimpleTypeName("number")},
            {typeof (double), new RtSimpleTypeName("number")},
            {typeof (decimal), new RtSimpleTypeName("number")}
        };

        private readonly ExportSettings _settings;

        /// <summary>
        ///     Constructs new type resolver
        /// </summary>
        public TypeResolver(ExportSettings settings)
        {
            _defaultGenerators[MemberTypes.Property] = new PropertyCodeGenerator { Settings = settings };
            _defaultGenerators[MemberTypes.Field] = new FieldCodeGenerator { Settings = settings };
            _defaultGenerators[MemberTypes.Method] = new MethodCodeGenerator { Settings = settings };
            _defaultGenerators[MemberTypes.Constructor] = new ConstructorCodeGenerator { Settings = settings };
            _defaultParameterGenerator = new ParameterCodeGenerator { Settings = settings };
            _defaultClassGenerator = new ClassCodeGenerator { Settings = settings };
            _defaultInterfaceGenerator = new InterfaceCodeGenerator { Settings = settings };
            _defaultEnumGenerator = new EnumGenerator { Settings = settings };
            _defaultNsgenerator = new NamespaceCodeGenerator { Settings = settings };
            _settings = settings;
        }

        /// <summary>
        ///     Reteieves code generator instance for specified type member.
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <typeparam name="T">Type member info type</typeparam>
        /// <param name="member">Type member info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for specified type member</returns>
        public ITsCodeGenerator<T, TNode> GeneratorFor<T, TNode>(T member, ExportSettings settings)
            where T : MemberInfo
            where TNode : RtNode
        {
            var attr = ConfigurationRepository.Instance.ForMember<TsTypedMemberAttributeBase>(member);
            var fromAttr = GetFromAttribute<T, TNode>(attr, settings);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var decType = member.DeclaringType;
                var classAttr = ConfigurationRepository.Instance.ForType<TsClassAttribute>(decType);
                if (classAttr != null && classAttr.DefaultMethodCodeGenerator != null)
                {
                    return LazilyInstantiateGenerator<T, TNode>(classAttr.DefaultMethodCodeGenerator, settings);
                }
            }
            var gen = (ITsCodeGenerator<T, TNode>)_defaultGenerators[member.MemberType];
            gen.Settings = settings;
            return gen;
        }

        /// <summary>
        ///     Retrieves code generator for ParameterInfo (since ParameterInfo does not derive from MemberInfo).
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Parameter info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for parameter info</returns>
        public ITsCodeGenerator<ParameterInfo, RtArgument> GeneratorFor(ParameterInfo member, ExportSettings settings)
        {
            var attr = ConfigurationRepository.Instance.ForMember(member);
            var fromAttr = GetFromAttribute<ParameterInfo, RtArgument>(attr, settings);
            if (fromAttr != null) return fromAttr;
            return _defaultParameterGenerator;
        }

        /// <summary>
        ///     Retrieves code generator for specified type
        ///     Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Type info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for specified type</returns>
        public ITsCodeGenerator<Type, RtCompilationUnit> GeneratorFor(Type member, ExportSettings settings)
        {
            var attr = ConfigurationRepository.Instance.ForType(member);
            var fromAttr = GetFromAttribute<Type, RtCompilationUnit>(attr, settings);
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
        public NamespaceCodeGenerator GeneratorForNamespace(ExportSettings settings)
        {
            _defaultNsgenerator.Settings = settings;
            return _defaultNsgenerator;
        }

        private ITsCodeGenerator<T, TNode> GetFromAttribute<T, TNode>(TsAttributeBase attr, ExportSettings settings) where TNode : RtNode
        {
            if (attr != null)
            {
                var t = attr.CodeGeneratorType;
                if (t != null) return LazilyInstantiateGenerator<T, TNode>(t, settings);
            }
            return null;
        }

        private ITsCodeGenerator<T, TNode> LazilyInstantiateGenerator<T, TNode>(Type generatorType, ExportSettings settings) where TNode : RtNode
        {
            lock (_generatorsCache)
            {
                if (!_generatorsCache.ContainsKey(generatorType))
                {
                    _generatorsCache[generatorType] = Activator.CreateInstance(generatorType);
                    var gen = (ITsCodeGenerator<T, TNode>)_generatorsCache[generatorType];
                    gen.Settings = settings;
                }
                return (ITsCodeGenerator<T, TNode>)_generatorsCache[generatorType];
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
            if (_resolveCache.ContainsKey(t)) return _resolveCache[t];

            var td = ConfigurationRepository.Instance.ForType(t);
            if (td != null)
            {
                var ns = t.Namespace;
                if (!td.IncludeNamespace) ns = string.Empty;
                if (!string.IsNullOrEmpty(td.Namespace)) ns = td.Namespace;

                var result = t.GetName(GetConcreteGenericArguments(t));
                result.Namespace = ns;
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
                    return Cache(t, new RtDictionaryType(AnyType, AnyType));
                }
                var gargs = t.GetGenericArguments();
                var key = ResolveTypeName(gargs[0]);
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