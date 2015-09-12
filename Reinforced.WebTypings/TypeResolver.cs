using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Reinforced.WebTypings.Generators;

namespace Reinforced.WebTypings
{
    public class TypeResolver
    {
        private readonly Dictionary<MemberTypes, object> _defaultGenerators = new Dictionary<MemberTypes, object>();
        private readonly ITsCodeGenerator<ParameterInfo> _defaultParameterGenerator;
        private readonly ITsCodeGenerator<Type> _defaultInterfaceGenerator;
        private readonly ITsCodeGenerator<Type> _defaultClassGenerator;
        private readonly ITsCodeGenerator<Type> _defaultEnumGenerator;

        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

        public TypeResolver()
        {
            _defaultGenerators[MemberTypes.Property] = new PropertyCodeGenerator();
            _defaultGenerators[MemberTypes.Field] = new FieldCodeGenerator();
            _defaultGenerators[MemberTypes.Method] = new MethodCodeGenerator();
            _defaultParameterGenerator = new ParameterCodeGenerator();
            _defaultClassGenerator = new ClassCodeGenerator();
            _defaultInterfaceGenerator = new InterfaceCodeGenerator();
            _defaultEnumGenerator = new EnumGenerator();

        }

        public ITsCodeGenerator<T> GeneratorFor<T>(T member) where T : MemberInfo
        {
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<T>(attr);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var decType = member.DeclaringType;
                var classAttr = decType.GetCustomAttribute<TsClassAttribute>();
                if (classAttr != null && classAttr.DefaultMethodCodeGenerator != null)
                {
                    return LazilyInstantiateGenerator<T>(classAttr.DefaultMethodCodeGenerator);
                }
            }
            return (ITsCodeGenerator<T>)_defaultGenerators[member.MemberType];
        }

        public ITsCodeGenerator<ParameterInfo> GeneratorFor(ParameterInfo member)
        {
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<ParameterInfo>(attr);
            if (fromAttr != null) return fromAttr;
            return _defaultParameterGenerator;
        }

        private ITsCodeGenerator<T> GetFromAttribute<T>(TsAttributeBase attr)
        {
            if (attr != null)
            {
                var t = attr.CodeGeneratorType;
                if (t != null) return LazilyInstantiateGenerator<T>(t);
            }
            return null;
        }

        private ITsCodeGenerator<T> LazilyInstantiateGenerator<T>(Type generatorType)
        {
            lock (_generatorsCache)
            {
                if (!_generatorsCache.ContainsKey(generatorType))
                {
                    _generatorsCache[generatorType] = Activator.CreateInstance(generatorType);
                }
                return (ITsCodeGenerator<T>)_generatorsCache[generatorType];
            }
        }

        public ITsCodeGenerator<Type> GeneratorFor(Type member)
        {
            var attr = member.GetCustomAttribute<TsAttributeBase>();
            var fromAttr = GetFromAttribute<Type>(attr);
            if (fromAttr != null) return fromAttr;

            bool isClass = attr is TsClassAttribute;
            bool isInterface = attr is TsInterfaceAttribute;
            bool isEnum = attr is TsEnumAttribute;

            if (isClass) return _defaultClassGenerator;
            if (isInterface) return _defaultInterfaceGenerator;
            if (isEnum) return _defaultEnumGenerator;
            return null;
        }



        private readonly HashSet<Type> _numerics = new HashSet<Type>()
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
                else
                {
                    var gargs = type.GetGenericArguments();
                    var key = ResolveTypeName(gargs[0]);
                    var value = ResolveTypeName(gargs[1]);
                    return String.Format("{{ [key: {0}]: {1} }}", key, value);
                }
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
