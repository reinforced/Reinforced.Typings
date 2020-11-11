using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using Reinforced.Typings.ReferencesInspection;
using Reinforced.Typings.Xmldoc;
// ReSharper disable CheckNamespace
namespace Reinforced.Typings
{
    /// <summary>
    ///     TsExport exporting settings
    /// </summary>
    public partial class ExportContext
    {
        private bool _isInitialized;

        internal void Initialize()
        {
            if (_isInitialized) return;

            ApplyFluent();
            InitializeDocumentation();
            BuildTypesCache();
            InspectGlobalReferences();
            Generators = new GeneratorManager(this);

            _isInitialized = true;
        }

        private void ApplyFluent()
        {
            var hasFluentConfiguration = ConfigurationMethod != null;
            if (hasFluentConfiguration)
            {
                var configurationBuilder = new ConfigurationBuilder(this);
                ConfigurationMethod(configurationBuilder);
            }
        }

        private void InitializeDocumentation()
        {
            Documentation =
                new DocumentationManager(Global.GenerateDocumentation ? DocumentationFilePath : null, Warnings);
            foreach (var additionalDocumentationPath in Project.AdditionalDocumentationPathes)
            {
                Documentation.CacheDocumentation(additionalDocumentationPath, Warnings);
            }
        }

        internal InspectedReferences _globalReferences;
        private void InspectGlobalReferences()
        {
            var assemblies = SourceAssemblies;
            var references = assemblies.Where(c => c.GetCustomAttributes<TsReferenceAttribute>().Any())
                .SelectMany(c => c.GetCustomAttributes<TsReferenceAttribute>())
                .Select(c => c.ToReference())
                .Union(Project.References);

            if (Global.UseModules)
            {
                var imports = assemblies.Where(c => c.GetCustomAttributes<TsImportAttribute>().Any())
                    .SelectMany(c => c.GetCustomAttributes<TsImportAttribute>())
                    .Select(c => c.ToImport())
                    .Union(Project.Imports);
                _globalReferences = new InspectedReferences(references, imports);
                return;
            }
            _globalReferences = new InspectedReferences(references);
        }



        private HashSet<Type> _allTypesHash;
        private void BuildTypesCache()
        {
            var allTypes = SourceAssemblies
                .SelectMany(c => c._GetTypes(Warnings)
                    .Where(d => d.GetCustomAttribute<TsAttributeBase>(false) != null || d.GetCustomAttribute<TsThirdPartyAttribute>() != null))
                .Union(Project.BlueprintedTypes)
                .Distinct()
                .ToList();

            _allTypesHash = new HashSet<Type>(allTypes);
            if (Hierarchical)
            {
                foreach (var type in _allTypesHash)
                {
                    Project.AddFileSeparationSettings(type);
                }
            }

            if (!Hierarchical)
            {
                TypesToFilesMap = new Dictionary<string, IEnumerable<Type>>();
                CustomGeneratorsToFilesMap = new Dictionary<string, IEnumerable<ICustomCodeGenerator>>();
            }
            else 
            {
                TypesToFilesMap =
                    allTypes
                        .Where(d => Project.Blueprint(d).ThirdParty == null)
                        .GroupBy(c => GetPathForType(c, stripExtension: false))
                        .ToDictionary(c => c.Key, c => c.AsEnumerable());
                CustomGeneratorsToFilesMap =
                    CustomBuilders
                        .GroupBy(b => Path.Combine(TargetDirectory,b.FileName))
                        .ToDictionary(g => g.Key, g => g.Select(b => b.Generator));
            }


        }
    }
}