using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings
{
    /// <summary>
    /// Class for managing and instantiating code generators
    /// </summary>
    public class GeneratorManager
    {
        private readonly ITsCodeGenerator<Type> _defaultClassGenerator;
        private readonly ITsCodeGenerator<Type> _defaultEnumGenerator;
        private readonly Dictionary<MemberTypes, object> _defaultGenerators = new Dictionary<MemberTypes, object>();
        private readonly ITsCodeGenerator<Type> _defaultInterfaceGenerator;
        private readonly NamespaceCodeGenerator _defaultNsgenerator;
        private readonly ITsCodeGenerator<ParameterInfo> _defaultParameterGenerator;
        private readonly Dictionary<Type, object> _generatorsCache = new Dictionary<Type, object>();

        internal GeneratorManager(ExportContext context)
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
            var attr = context.CurrentBlueprint.ForMember<TsTypedMemberAttributeBase>(member);
            var fromAttr = GetFromAttribute<T>(attr, context);
            if (fromAttr != null) return fromAttr;
            if (member is MethodInfo)
            {
                var classAttr = context.CurrentBlueprint.Attr<TsClassAttribute>();
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
            var attr = context.CurrentBlueprint.ForMember(member);
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
            var attr = context.Project.Blueprint(member).TypeAttribute;
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
    }
}
