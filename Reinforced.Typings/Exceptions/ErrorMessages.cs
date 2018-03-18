using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Exceptions
{
    /// <summary>
    /// This class contains all RT's error and siagnostic messages. 
    /// Why didnt I use resources? I dont want to add one more .dll to RT's NuGet package. 
    /// if localization will be required through issues then I will add one
    /// </summary>
    class ErrorMessages
    {
        #region Errors
        /// <summary>
        /// Could not acuire temorary file {0}: {1}
        /// </summary>
        public static readonly ErrorMessage RTE0001_TempFileError = new ErrorMessage(0001,"Could not acuire temorary file {0}: {1}","IO");

        /// <summary>
        /// Could not replace source file {0}: {1}
        /// </summary>
        public static readonly ErrorMessage RTE0002_DeployingFilesError = new ErrorMessage(0002,"Could not replace source file {0}: {1}","IO");

        /// <summary>
        /// Could not instantiate code generator {0}: {1}
        /// </summary>
        public static readonly ErrorMessage RTE0003_GeneratorInstantiate = new ErrorMessage(0003,"Could not instantiate code generator {0}: {1}","Code generation");

        /// <summary>
        /// Code generator {0} has thrown an error: {1}
        /// </summary>
        public static readonly ErrorMessage RTE0004_GeneratorError = new ErrorMessage(0004,"Code generator {0} has thrown an error: {1}","Code generation");

        /// <summary>
        /// Could not resolve type for {0}. An error occured: {1}
        /// </summary>
        public static readonly ErrorMessage RTE0005_TypeResolvationError = new ErrorMessage(0005, "Could not resolve type for {0}. An error occured: {1}", "Type resolvation");

        /// <summary>
        /// Exception thrown when applying fluent configuration method for {1} '{2}': {0}
        /// </summary>
        public static readonly ErrorMessage RTE0006_FluentSingleError = new ErrorMessage(0006, "Exception thrown when applying fluent configuration method for {1} '{2}': {0}", "Fluent configuration");

        /// <summary>
        /// Exception thrown when applying fluent configuration method for collection of {1}: {0}
        /// </summary>
        public static readonly ErrorMessage RTE0007_FluentBatchError = new ErrorMessage(0007, "Exception thrown when applying fluent configuration method for collection of {1}: {0}", "Fluent configuration");

        /// <summary>
        /// MethodCallExpression should be provided for .WithMethod call. Please use only lamba expressions in this place.
        /// </summary>
        public static readonly ErrorMessage RTE0008_FluentWithMethodError = new ErrorMessage(0008, "MethodCallExpression should be provided for .WithMethod call. Please use only lamba expressions in this place.", "Fluent configuration");


        /// <summary>
        /// Sorry, but {0} is not very good idea for parameter configuration. Try using simplier lambda expression.
        /// </summary>
        public static readonly ErrorMessage RTE0009_FluentWithMethodCouldNotParse = new ErrorMessage(0009, "Sorry, but {0} is not very good idea for parameter configuration. Try using simplier lambda expression.", "Fluent configuration");

        /// <summary>
        /// Property lambda expression expected in {0}
        /// </summary>
        public static readonly ErrorMessage RTE0010_PropertyLambdaExpected = new ErrorMessage(0010, "Property lambda expression expected in {0}", "Fluent configuration");
        
        /// <summary>
        /// Field lambda expression expected in {0}
        /// </summary>
        public static readonly ErrorMessage RTE0011_FieldLambdaExpected = new ErrorMessage(0011, "Field lambda expression expected in {0}", "Fluent configuration");

        /// <summary>
        /// NewExpression should be provided for .WithConstructor call. Please use only lamba expressions in this place.
        /// </summary>
        public static readonly ErrorMessage RTE0012_NewExpressionLambdaExpected = new ErrorMessage(0012, "NewExpression should be provided for .WithConstructor call. Please use only 'new ...' lamba expressions in this place.", "Fluent configuration");

        /// <summary>
        /// Error when trying to locate particular field
        /// </summary>
        public static readonly ErrorMessage RTE0012_InvalidField = new ErrorMessage(0013, "Could not locate field {0} in class {1}", "Reflection");

        /// <summary>
        /// Error when trying to locate particular property
        /// </summary>
        public static readonly ErrorMessage RTE0013_InvalidProperty = new ErrorMessage(0014, "Could not locate property {0} in class {1}", "Reflection");
        #endregion

        #region Warnings
        /// <summary>
        /// XMLDOC file not supplied
        /// </summary>
        public static readonly ErrorMessage RTW0001_DocumentationNotSupplied = new ErrorMessage(0001, "XMLDOC file not supplied", "JSDOC");

        /// <summary>
        /// Could not find XMLDOC file {0}
        /// </summary>
        public static readonly ErrorMessage RTW0002_DocumentationNotFound = new ErrorMessage(0002, "Could not find XMLDOC file {0}", "JSDOC");

        /// <summary>
        /// Could not find suitable TypeScript type for {0}. 'any' assumed.
        /// </summary>
        public static readonly ErrorMessage RTW0003_TypeUnknown = new ErrorMessage(0003, "Could not find suitable TypeScript type for {0}. 'any' assumed.", "Type resolvation");

        /// <summary>
        /// No suitable base constructor found for {0}. Generating 'super' call with all nulls.
        /// </summary>
        public static readonly ErrorMessage RTW0004_DefaultSuperCall = new ErrorMessage(0004, "No suitable base constructor found for {0}. Generating 'super' call with all nulls.", "Class code generation");
        
        /// <summary>
        /// Class {0} (base for {1}) is exported as interface. It is potentially unsafe facility.
        /// </summary>
        public static readonly ErrorMessage RTW0005_BaseClassExportingAsInterface = new ErrorMessage(0005, "Class {0} (base for {1}) is exported as interface. It is potentially unsafe facility.", "Class code generation");

        /// <summary>
        /// Error parsering XMLDOC file {0}: {1}
        /// </summary>
        public static readonly ErrorMessage RTW0006_DocumentationParseringError = new ErrorMessage(0006, "Error parsering XMLDOC file {0}: {1}", "JSDOC");

        /// <summary>
        /// Error parsering XMLDOC file {0}: {1}
        /// </summary>
        public static readonly ErrorMessage RTW0007_InvalidDictionaryKey = new ErrorMessage(0007, "{0} is not valid type for JS object key (original type {1})", "Type resolvation");

        /// <summary>
        /// Error of type loding
        /// </summary>
        public static readonly ErrorMessage RTW0008_TypeloadException = new ErrorMessage(0008, "Some types cannot be loaded via reflection: {0}", "Type loading");

        

        #endregion
    }
}
