using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings
{
    /// <summary>
    /// Type resolver. It is helper class to convert source types, members and parameter names to typescript ones
    /// </summary>
    public class TypeResolver
    {
        private readonly Dictionary<MemberTypes, object> _defaultGenerators = new Dictionary<MemberTypes, object>();
        private readonly ITsCodeGenerator<ParameterInfo> _defaultParameterGenerator;
        private readonly ITsCodeGenerator<Type> _defaultInterfaceGenerator;
        private readonly ITsCodeGenerator<Type> _defaultClassGenerator;
        private readonly ITsCodeGenerator<Type> _defaultEnumGenerator;
        private readonly NamespaceCodeGenerator _defaultNsgenerator;

        private readonly ExportSettings _settings;
        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

        /// <summary>
        /// Constructs new type resolver
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
        /// Reteieves code generator instance for specified type member. 
        /// Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <typeparam name="T">Type member info type</typeparam>
        /// <param name="member">Type member info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for specified type member</returns>
        public ITsCodeGenerator<T> GeneratorFor<T>(T member, ExportSettings settings) where T : MemberInfo
        {
            var attr = ConfigurationRepository.Instance.ForMember<TsTypedMemberAttributeBase>(member);
            var fromAttr = GetFromAttribute<T>(attr, settings);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var decType = member.DeclaringType;
                var classAttr = ConfigurationRepository.Instance.ForType<TsClassAttribute>(decType);
                if (classAttr != null && classAttr.DefaultMethodCodeGenerator != null)
                {
                    return LazilyInstantiateGenerator<T>(classAttr.DefaultMethodCodeGenerator, settings);
                }
            }
            var gen = (ITsCodeGenerator<T>)_defaultGenerators[member.MemberType];
            gen.Settings = settings;
            return gen;
        }

        /// <summary>
        /// Retrieves code generator for ParameterInfo (since ParameterInfo does not derive from MemberInfo). 
        /// Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Parameter info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for parameter info</returns>
        public ITsCodeGenerator<ParameterInfo> GeneratorFor(ParameterInfo member, ExportSettings settings)
        {
            var attr =ConfigurationRepository.Instance.ForMember(member);
            var fromAttr = GetFromAttribute<ParameterInfo>(attr, settings);
            if (fromAttr != null) return fromAttr;
            return _defaultParameterGenerator;
        }

        /// <summary>
        /// Retrieves code generator for specified type
        /// Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <param name="member">Type info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for specified type</returns>
        public ITsCodeGenerator<Type> GeneratorFor(Type member, ExportSettings settings)
        {
            var attr = ConfigurationRepository.Instance.ForType(member);
            var fromAttr = GetFromAttribute<Type>(attr, settings);
            if (fromAttr != null) return fromAttr;

            bool isClass = attr is TsClassAttribute;
            bool isInterface = attr is TsInterfaceAttribute;
            bool isEnum = attr is TsEnumAttribute;

            if (isClass) return _defaultClassGenerator;
            if (isInterface) return _defaultInterfaceGenerator;
            if (isEnum) return _defaultEnumGenerator;
            return null;
        }

        /// <summary>
        /// Retrieves code generator for namespaces
        /// </summary>
        /// <returns></returns>
        public NamespaceCodeGenerator GeneratorForNamespace(ExportSettings settings)
        {
            _defaultNsgenerator.Settings = settings;
            return _defaultNsgenerator;
        }

        private ITsCodeGenerator<T> GetFromAttribute<T>(TsAttributeBase attr, ExportSettings settings)
        {
            if (attr != null)
            {
                var t = attr.CodeGeneratorType;
                if (t != null) return LazilyInstantiateGenerator<T>(t, settings);
            }
            return null;
        }

        private ITsCodeGenerator<T> LazilyInstantiateGenerator<T>(Type generatorType, ExportSettings settings)
        {
            lock (_generatorsCache)
            {
                if (!_generatorsCache.ContainsKey(generatorType))
                {
                    _generatorsCache[generatorType] = Activator.CreateInstance(generatorType);
                    var gen = (ITsCodeGenerator<T>)_generatorsCache[generatorType];
                    gen.Settings = settings;
                }
                return (ITsCodeGenerator<T>)_generatorsCache[generatorType];
            }
        }

        private string GetConcreteGenericArguments(Type t)
        {
            if (!t.IsGenericType) return String.Empty;
            var args = t.GetGenericArguments();
            return String.Format("<{0}>", string.Join(", ", args.Select(ResolveTypeName)));
        }

        private readonly Dictionary<Type, string> _resolveCache = new Dictionary<Type, string>()
        {
            {typeof(object),"any"},
            {typeof(void),"void"},
            {typeof(string),"string"},
            {typeof(char),"string"},
            {typeof(bool),"boolean"},
            {typeof(byte),"number"},{typeof(sbyte),"number"},{
            typeof(short),"number"},{typeof(ushort),"number"},{
            typeof(int),"number"},{typeof(uint),"number"},{
            typeof(long),"number"},{typeof(ulong),"number"},{
            typeof(float),"number"},{typeof(double),"number"},{
            typeof(decimal),"number"}

        };

        private string Cache(Type t, string name)
        {
            _resolveCache[t] = name;
            return TruncateNamespace(name);
        }

        private string TruncateNamespace(string typeName)
        {
            return typeName.Replace(_settings.CurrentNamespace, String.Empty).Trim('.');
        }

        /// <summary>
        /// Returns typescript-friendly type name for specified type. 
        /// This method successfully handles dictionaries, IEnumerables, arrays, another TsExport-ed types, void, delegates, most of CLR built-in types, parametrized types etc. 
        /// It also considers Ts*-attributes while resolving type names
        /// If it cannot handle anything then it will return "any"
        /// </summary>
        /// <param name="t">Specified type</param>
        /// <returns>Typescript-friendly type name</returns>
        public string ResolveTypeName(Type t)
        {
            if (_resolveCache.ContainsKey(t)) return TruncateNamespace(_resolveCache[t]);

            var td = ConfigurationRepository.Instance.ForType(t);
            if (td != null)
            {
                string ns = t.Namespace;
                if (!td.IncludeNamespace) ns = String.Empty;
                if (!string.IsNullOrEmpty(td.Namespace)) ns = td.Namespace;

                string name = t.GetName() + GetConcreteGenericArguments(t);
                if (string.IsNullOrEmpty(ns)) { return Cache(t, name); }

                return Cache(t, string.Format("{0}.{1}", ns, name));
            }
            if (t.IsNullable())
            {
                return ResolveTypeName(t.GetArg());
            }
            if (t.IsDictionary())
            {
                if (!t.IsGenericType) { return Cache(t, "{ [key: any]: any }"); }
                var gargs = t.GetGenericArguments();
                var key = ResolveTypeName(gargs[0]);
                var value = ResolveTypeName(gargs[1]);
                var name = String.Format("{{ [key: {0}]: {1} }}", key, value);
                return Cache(t, name);
            }
            if (t.IsNongenericEnumerable())
            {
                return Cache(t, "any[]");
            }
            if (t.IsEnumerable())
            {
                return Cache(t, ResolveTypeName(t.IsArray ? t.GetElementType() : t.GetArg()) + "[]");
            }

            if (t.IsGenericParameter)
            {
                var genAt = t.GetCustomAttribute<TsGenericAttribute>(false);
                if (genAt != null)
                {
                    if (genAt.StrongType != null) return Cache(t, ResolveTypeName(genAt.StrongType));
                    if (genAt.Type != null) return Cache(t, genAt.Type);
                }
                return Cache(t, t.Name);
            }
            if (typeof(MulticastDelegate).IsAssignableFrom(t.BaseType))
            {
                var methodInfo = t.GetMethod("Invoke");
                return Cache(t, ConstructFunctionType(methodInfo));
            }

            return Cache(t, "any");
        }

        internal void PrintCacheInfo()
        {
            Console.WriteLine("Types resolving cache contains {0} entries", _resolveCache.Count);
        }

        private string ConstructFunctionType(MethodInfo methodInfo)
        {
            var retType = ResolveTypeName(methodInfo.ReturnType);
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            int argAggreagtor = 0;
            bool first = true;
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                string argName = argAggreagtor > 0 ? "arg" + argAggreagtor : "arg";
                var typeName = ResolveTypeName(parameterInfo.ParameterType);
                if (!first)
                {
                    sb.Append(", ");
                }
                else
                {
                    first = false;
                }
                sb.AppendFormat("{0}:{1}", argName, typeName);
                argAggreagtor++;
            }
            sb.AppendFormat(") => {0}", retType);
            return sb.ToString();
        }
    }
}
