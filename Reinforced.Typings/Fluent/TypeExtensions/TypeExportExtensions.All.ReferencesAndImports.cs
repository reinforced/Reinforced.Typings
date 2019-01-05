using System;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypeExportExtensions
    {
        /// <summary>
        ///     Adds reference directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="referenceFile">Path to referenced file</param>
        public static T AddReference<T>(this T configuration, string referenceFile)
            where T : TypeExportBuilder
        {
            configuration.Blueprint.References.Add(new TsAddTypeReferenceAttribute(referenceFile));
            return configuration;
        }

        /// <summary>
        ///     Adds reference directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="referencedType">Another generated type that should be referenced</param>
        public static T AddReference<T>(this T configuration, Type referencedType)
            where T : TypeExportBuilder
        {
            configuration.Blueprint.References.Add(new TsAddTypeReferenceAttribute(referencedType));
            return configuration;
        }

        /// <summary>
        ///     Adds import directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="target">
        /// What we are importing from module.
        /// Everything that is placed after "import" keyword and before "from" or "= require(..)"
        /// Examples: 
        /// - "import * as shape from './Shapes'" -> "* as shape" is target <br/>
        /// - "import { Foo } from 'Bar'" -> "{ Foo }" is target <br/>
        /// - "import { Bar2 as bar } from 'Baz'" -> "{ Bar2 as bar }" is target <br/>
        /// If ImportTarget is null then side-effect import will be generated. 
        /// </param>
        /// <param name="from">
        /// Import source is everything that follows after "from" keyword. 
        /// Please not the you do not have to specify quotes here! Quotes will be added automatically
        /// </param>
        /// <param name="isRequire">When true, import will be generated as "import ImportTarget = require('ImportSource')"</param>
        public static T AddImport<T>(this T conf, string target, string from, bool isRequire = false)
            where T : TypeExportBuilder
        {
            conf.Blueprint.Imports.Add(new TsAddTypeImportAttribute(target, @from, isRequire));
            return conf;
        }

        /// <summary>
        ///     Overrides target file name where specified name will be exported.
        ///     This option will only be processed when RtDivideTypesAmongFiles is true.
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="fileName">Target file path override. Related to RtTargetDirectory</param>
        public static T ExportTo<T>(this T configuration, string fileName) where T : TypeExportBuilder
        {
            configuration.Blueprint.PathToFile = fileName;
            return configuration;
        }
    }
}