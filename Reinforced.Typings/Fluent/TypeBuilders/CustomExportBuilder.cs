using System;
using Reinforced.Typings.Generators;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for class
    /// </summary>
    public class CustomExportBuilder
    {
        private string _fileName;

        internal CustomExportBuilder(ICustomCodeGenerator generator)
        {
            Generator = generator;
        }
        internal ICustomCodeGenerator Generator { get; }

        internal Type GeneratorType { get; set; }

        internal string FileName
        {
            get => string.IsNullOrWhiteSpace(_fileName) ? Generator.GetType().Name : _fileName;
            set => _fileName = value;
        }

        public void WithFileName(string fileName)
        {
            FileName = fileName;
        }
    }

    /// <summary>
    /// Set of extensions for type export configuration
    /// </summary>
    public static partial class TypeConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Includes specified <see cref="ICustomCodeGenerator"/> in generated TypeScript
        /// </summary>
        /// <typeparam name="T"><see cref="ICustomCodeGenerator"/> to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static CustomExportBuilder ExportCustomCode<T>(this ConfigurationBuilder builder) where T : ICustomCodeGenerator, new()
        {

            foreach (CustomExportBuilder customBuilder in builder.Context.CustomBuilders)
            {
                if (customBuilder.GeneratorType == typeof(T))
                    return customBuilder;
            }

            var conf = new CustomExportBuilder(new T()) {GeneratorType = typeof(T)};
            builder.Context.CustomBuilders.Add(conf);
            return conf;
        }

        /// <summary>
        ///     Includes specified <see cref="ICustomCodeGenerator"/> in generated TypeScript
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="generator"><see cref="ICustomCodeGenerator"/> to include</param>
        /// <returns>Fluent</returns>
        public static CustomExportBuilder ExportCustomCode(this ConfigurationBuilder builder, ICustomCodeGenerator generator)
        {

            foreach (CustomExportBuilder customBuilder in builder.Context.CustomBuilders)
            {
                if (customBuilder.Generator == generator)
                    return customBuilder;
            }
            
            var conf = new CustomExportBuilder(generator);
            builder.Context.CustomBuilders.Add(conf);
            return conf;
        }

       
    }
}