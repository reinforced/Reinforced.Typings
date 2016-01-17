using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Reinforced.Typings.Cli;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Fluent.Interfaces;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Playground
{
    class Program
    {

        public interface IMyInterface
        {
            void DoSomething(int a, string b);
        }
        static void Main(string[] args)
        {
            Bootstrapper.Main(Luurit);
            Console.ReadKey();


        }

        public static void Configure(ConfigurationBuilder builder)
        {
            builder.ExportAsInterface<IMyInterface>()
                .WithMethod(c => 
                    c.DoSomething(
                        Ts.Parameter<int>(a => a.OverrideName("blah")), 
                        Ts.Parameter<string>()));
            builder.ExportAsInterface<IMyInterface>()
                .WithPublicProperties();
        }

        private static void Configure(ParameterConfigurationBuilder builder)
        {
            builder.Type<string>();
        }

        private static string[] PowerTables = new string[]
                                    {
                        @"SourceAssemblies=""I:\Work\Pnovikov\PowerTables\PowerTables\PowerTables.Typings\obj\Debug\PowerTables.Typings.dll""",
            @"TargetFile=""I:\Work\Pnovikov\PowerTables\PowerTables\PowerTables.Typings\Scripts\Interfaces.ts""",
            @"TargetDirectory=""I:\Work\Pnovikov\PowerTables\PowerTables\PowerTables.Typings\Scripts\MyApplication""",
            @"References=""C:\Program Files (x86)\ReferenceAssemblies\Microsoft\Framework\.NETFramework\v4.5\Microsoft.CSharp.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.Web.Infrastructure.1.0.0.0\lib\net40\Microsoft.Web.Infrastructure.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll;I:\Work\Pnovikov\PowerTables\PowerTables\PowerTables\bin\Debug\PowerTables.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Reinforced.Typings.1.0.5\lib\net45\Reinforced.Typings.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.DataSetExtensions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.Helpers.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.Mvc.5.2.3\lib\net45\System.Web.Mvc.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.Razor.3.2.3\lib\net45\System.Web.Razor.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.Deployment.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.dll;I:\Work\Pnovikov\PowerTables\PowerTables\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.Razor.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.Linq.dll"" ",
            @"Hierarchy=""False"" ",
            @"WriteWarningComment=""True"" ",
            @"ExportPureTypings=""False"" ",
            @"RootNamespace=""My.Application.Root.Namespace"" ",
            @"CamelCaseForMethods=""False"" ",
            @"CamelCaseForProperties=""False"" ",
            @"DocumentationFilePath=""I:\Work\Pnovikov\PowerTables\PowerTables\PowerTables.Typings\"" ",
            @"GenerateDocumentation=""True"" ",
            @"ConfigurationMethod=""PowerTables.Typings.TypingsConfiguration.ConfigureTypings"""
                                    };

        private static string[] Luurit = new string[]
            {
 @"SourceAssemblies=""I:\Work\Redeem Nordics\Luurit\LuuritV3\obj\Debug\LuuritV3.dll""",
 @"TargetFile=""I:\Work\Redeem Nordics\Luurit\LuuritV3\Scripts\project.ts""",
 @"TargetDirectory=""I:\Work\Redeem Nordics\Luurit\LuuritV3\Scripts\MyApplication""",
 @"References=""I:\Work\Redeem Nordics\Luurit\ThirdParty\AdvancedIntellect.Ssl.dll;I:\Work\Redeem Nordics\Luurit\packages\Antlr.3.5.0.2\lib\Antlr3.Runtime.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\aspNetEmail.dll;I:\Work\Redeem Nordics\Luurit\packages\Autofac.3.5.2\lib\net40\Autofac.dll;I:\Work\Redeem Nordics\Luurit\packages\Autofac.Mvc5.3.3.4\lib\net45\Autofac.Integration.Mvc.dll;I:\Work\Redeem Nordics\Luurit\packages\Autofac.WebApi2.3.4.0\lib\net45\Autofac.Integration.WebApi.dll;I:\Work\Redeem Nordics\Luurit\packages\AutoMapper.4.0.0\lib\net45\AutoMapper.dll;I:\Work\Redeem Nordics\Luurit\packages\CsQuery.1.3.4\lib\net40\CsQuery.dll;I:\Work\Redeem Nordics\Luurit\packages\CsvTools.1.0.11\lib\CsvReader.dll;I:\Work\Redeem Nordics\Luurit\packages\OpenXmlSdk.2.0.0\lib\net45\DocumentFormat.OpenXml.dll;I:\Work\Redeem Nordics\Luurit\DynTypings\DynamicMvcTypings.exe;I:\Work\Redeem Nordics\Luurit\packages\elmah.corelibrary.1.2.2\lib\Elmah.dll;I:\Work\Redeem Nordics\Luurit\packages\EmbeddedResourceVirtualPathProvider.1.2.23\lib\net40\EmbeddedResourceVirtualPathProvider.dll;I:\Work\Redeem Nordics\Luurit\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll;I:\Work\Redeem Nordics\Luurit\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll;I:\Work\Redeem Nordics\Luurit\packages\EFUtilities.1.0.2\lib\EntityFramework.Utilities.dll;I:\Work\Redeem Nordics\Luurit\Enumerations\bin\Debug\Enumerations.dll;I:\Work\Redeem Nordics\Luurit\packages\ExpressiveAnnotations.2.6.3\lib\net45\ExpressiveAnnotations.dll;I:\Work\Redeem Nordics\Luurit\packages\ExpressiveAnnotations.2.6.3\lib\net45\ExpressiveAnnotations.MvcUnobtrusive.dll;I:\Work\Redeem Nordics\Luurit\packages\iTextSharp.5.5.6\lib\itextsharp.dll;I:\Work\Redeem Nordics\Luurit\packages\itextsharp.xmlworker.5.5.6\lib\itextsharp.xmlworker.dll;I:\Work\Redeem Nordics\Luurit\CSharp\Luurit.Common\bin\Debug\Luurit.Common.dll;I:\Work\Redeem Nordics\Luurit\Resources\bin\Debug\Luurit.Localization.dll;I:\Work\Redeem Nordics\Luurit\CSharp\Luurit.Logic\bin\Debug\Luurit.Logic.dll;I:\Work\Redeem Nordics\Luurit\CSharp\LuuritDataLayer\bin\Debug\LuuritDataLayer.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\Marquite\Marquite.Angular.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\Marquite\Marquite.Bootstrap.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\Marquite\Marquite.Core.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\Microsoft.CSharp.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.Web.Administration.7.0.0.0\lib\net20\Microsoft.Web.Administration.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.Web.Infrastructure.1.0.0.0\lib\net40\Microsoft.Web.Infrastructure.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\mscorlib.dll;I:\Work\Redeem Nordics\Luurit\Mvc.JQuery.Datatables\bin\Debug\Mvc.JQuery.Datatables.dll;I:\Work\Redeem Nordics\Luurit\packages\MvcMailer.4.5\lib\45\Mvc.Mailer.dll;I:\Work\Redeem Nordics\Luurit\packages\MvcCheckBoxList.1.4.4.5\lib\net45\MvcCheckBoxList.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\Neodynamic.WebControls.ImageDraw.dll;I:\Work\Redeem Nordics\Luurit\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll;I:\Work\Redeem Nordics\Luurit\packages\NLog.4.0.1\lib\net45\NLog.dll;I:\Work\Redeem Nordics\Luurit\packages\NLog.Elmah.2.0.0.0\lib\net35\NLog.Elmah.dll;I:\Work\Redeem Nordics\Luurit\packages\OpenExchangeRates.1.2.0\lib\OpenExchangeRates.dll;I:\Work\Redeem Nordics\Luurit\ThirdParty\PowerTables\PowerTables.dll;I:\Work\Redeem Nordics\Luurit\packages\QueryInterceptor.0.2\lib\net40\QueryInterceptor.dll;I:\Work\Redeem Nordics\Luurit\Redeem.Commands.Logic\bin\Debug\Redeem.Commands.Logic.dll;I:\Work\Redeem Nordics\Luurit\Redeem.Messaging.Logic\bin\Debug\Redeem.Messaging.Logic.dll;I:\Work\Redeem Nordics\Luurit\Redeem.Messaging.Email\bin\Debug\Redeem.Messaging.MessageTemplates.dll;I:\Work\Redeem Nordics\Luurit\Redeem.Platform.Templates.Documents\bin\Debug\Redeem.Platform.Templates.Documents.dll;I:\Work\Redeem Nordics\Luurit\Redeem.Templating\bin\Debug\Redeem.Templating.dll;I:\Work\Redeem Nordics\Luurit\packages\Reinforced.Typings.1.0.6\lib\net45\Reinforced.Typings.dll;I:\Work\Redeem Nordics\Luurit\ServiceLayer\bin\Debug\ServiceLayer.dll;I:\Work\Redeem Nordics\Luurit\packages\SmartFormat.NET.1.6.1.0\lib\net40\SmartFormat.dll;I:\Work\Redeem Nordics\Luurit\packages\SpreadsheetLight.3.4.4\lib\SpreadsheetLight.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.ComponentModel.DataAnnotations.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Configuration.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Core.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Data.DataSetExtensions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Data.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Data.Entity.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Drawing.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.EnterpriseServices.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Net.Http.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebApi.Client.5.2.3\lib\net45\System.Net.Http.Formatting.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Net.Http.WebRequest.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Runtime.Serialization.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Transactions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.Abstractions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.ApplicationServices.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.Cors.5.2.3\lib\net45\System.Web.Cors.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.DynamicData.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.Entity.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.Extensions.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.Helpers.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebApi.Cors.5.2.3\lib\net45\System.Web.Http.Cors.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebApi.Core.5.2.3\lib\net45\System.Web.Http.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebApi.WebHost.5.2.3\lib\net45\System.Web.Http.WebHost.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.Mvc.5.2.3\lib\net45\System.Web.Mvc.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.Web.Optimization.1.1.3\lib\net40\System.Web.Optimization.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.Razor.3.2.3\lib\net45\System.Web.Razor.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.Routing.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Web.Services.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.Deployment.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.dll;I:\Work\Redeem Nordics\Luurit\packages\Microsoft.AspNet.WebPages.3.2.3\lib\net45\System.Web.WebPages.Razor.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Xml.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Xml.Linq.dll;I:\Work\Redeem Nordics\Luurit\packages\WebGrease.1.6.0\lib\WebGrease.dll""",
 @"Hierarchy=""False""",
 @"WriteWarningComment=""True""",
 @"ExportPureTypings=""False""",
 @"RootNamespace=""My.Application.Root.Namespace""",
 @"CamelCaseForMethods=""False""",
 @"CamelCaseForProperties=""False""",
 @"DocumentationFilePath=""I:\Work\Redeem Nordics\Luurit\LuuritV3\bin\LuuritV3.XML"" GenerateDocumentation=""True""",
 @"ConfigurationMethod=""LuuritV3.ReinforcedTypingsConfiguration.Configure"""
            };

        private static string[] TestProject = new string[]
        {
@"SourceAssemblies=""I:\Work\Reinforced\tmp\TestProject\TestProject\obj\Debug\TestProject.dll"""
,@"TargetFile=""I:\Work\Reinforced\tmp\TestProject\TestProject\Scripts\project.ts"""
,@"TargetDirectory=""I:\Work\Reinforced\tmp\TestProject\TestProject\Scripts\MyApplication"""
,@"References=""I:\Work\Reinforced\tmp\TestProject\packages\Antlr.3.4.1.9004\lib\Antlr3.Runtime.dll;I:\Work\Reinforced\tmp\TestProject\packages\EntityFramework.6.1.1\lib\net45\EntityFramework.dll;I:\Work\Reinforced\tmp\TestProject\packages\EntityFramework.6.1.1\lib\net45\EntityFramework.SqlServer.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Identity.Core.2.1.0\lib\net45\Microsoft.AspNet.Identity.Core.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Identity.EntityFramework.2.1.0\lib\net45\Microsoft.AspNet.Identity.EntityFramework.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Identity.Owin.2.1.0\lib\net45\Microsoft.AspNet.Identity.Owin.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\Microsoft.CSharp.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.3.0.0\lib\net45\Microsoft.Owin.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Host.SystemWeb.3.0.0\lib\net45\Microsoft.Owin.Host.SystemWeb.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.Cookies.3.0.0\lib\net45\Microsoft.Owin.Security.Cookies.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.3.0.0\lib\net45\Microsoft.Owin.Security.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.Facebook.3.0.0\lib\net45\Microsoft.Owin.Security.Facebook.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.Google.3.0.0\lib\net45\Microsoft.Owin.Security.Google.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.MicrosoftAccount.3.0.0\lib\net45\Microsoft.Owin.Security.MicrosoftAccount.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.OAuth.3.0.0\lib\net45\Microsoft.Owin.Security.OAuth.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Owin.Security.Twitter.3.0.0\lib\net45\Microsoft.Owin.Security.Twitter.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.Web.Infrastructure.1.0.0.0\lib\net40\Microsoft.Web.Infrastructure.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll;I:\Work\Reinforced\tmp\TestProject\packages\Newtonsoft.Json.6.0.4\lib\net45\Newtonsoft.Json.dll;I:\Work\Reinforced\tmp\TestProject\packages\Owin.1.0\lib\net40\Owin.dll;I:\Work\Reinforced\tmp\TestProject\packages\Reinforced.Typings.1.0.6\lib\net45\Reinforced.Typings.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.ComponentModel.DataAnnotations.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Configuration.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.DataSetExtensions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Drawing.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.EnterpriseServices.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Net.Http.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Net.Http.WebRequest.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.Abstractions.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.ApplicationServices.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.DynamicData.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.Entity.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.Extensions.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.WebPages.3.2.2\lib\net45\System.Web.Helpers.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Mvc.5.2.2\lib\net45\System.Web.Mvc.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Web.Optimization.1.1.3\lib\net40\System.Web.Optimization.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.Razor.3.2.2\lib\net45\System.Web.Razor.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.Routing.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Web.Services.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.WebPages.3.2.2\lib\net45\System.Web.WebPages.Deployment.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.WebPages.3.2.2\lib\net45\System.Web.WebPages.dll;I:\Work\Reinforced\tmp\TestProject\packages\Microsoft.AspNet.WebPages.3.2.2\lib\net45\System.Web.WebPages.Razor.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.dll;C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.Linq.dll;I:\Work\Reinforced\tmp\TestProject\packages\WebGrease.1.5.2\lib\WebGrease.dll"""
,@"Hierarchy=""True"""
,@"WriteWarningComment=""True"""
,@"ExportPureTypings=""False"""
,@"RootNamespace=""TestProject.TestAppInfrastructure"""
,@"CamelCaseForMethods=""False"""
,@"CamelCaseForProperties=""False"""
,@"DocumentationFilePath=""I:\Work\Reinforced\tmp\TestProject\TestProject\bin\TestProject.XML"""
,@"GenerateDocumentation=""True"""
,@"ConfigurationMethod=""TestProject.App_Start.TypingsConfiguration.ConfigureTypings"""
        };
    }
}
