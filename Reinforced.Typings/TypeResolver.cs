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

        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

        /// <summary>
        /// Constructs new type resolver
        /// </summary>
        public TypeResolver(ExportSettings settings)
        {
            _defaultGenerators[MemberTypes.Property] = new PropertyCodeGenerator{Settings = settings};
            _defaultGenerators[MemberTypes.Field] = new FieldCodeGenerator { Settings = settings };
            _defaultGenerators[MemberTypes.Method] = new MethodCodeGenerator { Settings = settings };
            _defaultParameterGenerator = new ParameterCodeGenerator { Settings = settings };
            _defaultClassGenerator = new ClassCodeGenerator { Settings = settings };
            _defaultInterfaceGenerator = new InterfaceCodeGenerator { Settings = settings };
            _defaultEnumGenerator = new EnumGenerator { Settings = settings };
            _defaultNsgenerator = new NamespaceCodeGenerator { Settings = settings };

        }

        /// <summary>
        /// Reteieves code generator instance for specified type member. 
        /// Also this method considers Typings attribute and instantiates generator specified there if necessary
        /// </summary>
        /// <typeparam name="T">Type member info type</typeparam>
        /// <param name="member">Type member info</param>
        /// <param name="settings">Export settings</param>
        /// <returns>Code generator for specified type member</returns>
        public ITsCodeGenerator<T> GeneratorFor<T>(T member,ExportSettings settings) where T : MemberInfo
        {
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<T>(attr,settings);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var decType = member.DeclaringType;
                var classAttr = decType.GetCustomAttribute<TsClassAttribute>();
                if (classAttr != null && classAttr.DefaultMethodCodeGenerator != null)
                {
                    return LazilyInstantiateGenerator<T>(classAttr.DefaultMethodCodeGenerator,settings);
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
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<ParameterInfo>(attr,settings);
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
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<Type>(attr,settings);
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
                if (t != null) return LazilyInstantiateGenerator<T>(t,settings);
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
                    var gen = (ITsCodeGenerator<T>) _generatorsCache[generatorType];
                    gen.Settings = settings;
                }
                return (ITsCodeGenerator<T>)_generatorsCache[generatorType];
            }
        }
        
        private readonly HashSet<Type> _numerics = new HashSet<Type>
        {
            typeof(byte),typeof(sbyte),
            typeof(short),typeof(ushort),
            typeof(int),typeof(uint),
            typeof(long),typeof(ulong),
            typeof(float),typeof(double),
            typeof(decimal)
        };

        private string GetConcreteGenericArguments(Type t)
        {
            if (!t.IsGenericType) return String.Empty;
            var args = t.GetGenericArguments();
            return String.Format("<{0}>", string.Join(", ", args.Select(ResolveTypeName)));
        }

        /// <summary>
        /// Returns typescript-friendly type name for specified type. 
        /// This method successfully handles dictionaries, IEnumerables, arrays, another TsExport-ed types, void, delegates, most of CLR built-in types, parametrized types etc. 
        /// It also considers Ts*-attributes while resolving type names
        /// If it cannot handle anything then it will return "any"
        /// </summary>
        /// <param name="type">Specified type</param>
        /// <returns>Typescript-friendly type name</returns>
        public string ResolveTypeName(Type type)
        {
            if (type == typeof(object)) return "any";
            if (type == typeof(void)) return "void";
            if (type == typeof(string) || type == typeof(char)) return "string";
            if (type.IsPrimitive)
            {
                if (type == typeof(bool)) return "boolean";
                if (_numerics.Contains(type)) return "number";
            }
            var td = type.GetCustomAttribute<TsDeclarationAttributeBase>();
            if (td != null)
            {
                string ns = type.Namespace;
                if (!td.IncludeNamespace) ns = String.Empty;
                if (!string.IsNullOrEmpty(td.Namespace)) ns = td.Namespace;

                string name = type.GetName() + GetConcreteGenericArguments(type);
                if (string.IsNullOrEmpty(ns)) return name;
                return string.Format("{0}.{1}", ns, name);
            }
            if (type.IsNullable())
            {
                return ResolveTypeName(type.GetArg());
            }
            if (type.IsDictionary())
            {
                if (!type.IsGenericType) return "{ [key: any]: any }";
                var gargs = type.GetGenericArguments();
                var key = ResolveTypeName(gargs[0]);
                var value = ResolveTypeName(gargs[1]);
                return String.Format("{{ [key: {0}]: {1} }}", key, value);
            }
            if (type.IsNongenericEnumerable())
            {
                return "any[]";
            }
            if (type.IsEnumerable())
            {
                return ResolveTypeName(type.IsArray ? type.GetElementType() : type.GetArg()) + "[]";
            }
            
            if (type.IsGenericParameter)
            {
                var genAt = type.GetCustomAttribute<TsGenericAttribute>();
                if (genAt != null)
                {
                    if (genAt.StrongType != null) return ResolveTypeName(genAt.StrongType);
                    if (genAt.Type != null) return genAt.Type;
                }
                return type.Name;
            }
            if (typeof(MulticastDelegate).IsAssignableFrom(type.BaseType))
            {
                var methodInfo = type.GetMethod("Invoke");
                return ConstructFunctionType(methodInfo);
            }

            return "any";

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
