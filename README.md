Reinforced.Typings is available on [NuGet](https://www.nuget.org/packages/Reinforced.Typings/).
=================

```sh
PM> Install-Package Reinforced.Typings
```

**Find out detailed information in Reinforced.Typings [wiki](https://github.com/reinforced/Reinforced.Typings/wiki)**

News
=================
Version **1.3.0** releaed! 

Migration tips:
* Some of parameters are moved from [[Reinforced.Typings.settings.xml]] to [[[TsGlobal] attribute|Configuration-attributes]] and [[.Global fluent call|Fluent-configuration#global-configuration-builder]]. Please check out corresponding documentation pages to find out what has been changed
* `RtTypeName` and other types ASTs has been moved to `Reinforced.Typings.Ast.TypeNames` namespace. Be aware of updating your usings

Features:
* Modules support 
* Inline implementations 
* Substitutions 
* TS tuples 
* Types and members reordering 
* Global parameters 
* Improved generics support 
* Bugfixes
* Updated documentation
* Updated generators sample. Now documentation regarding using code generators is located in the [corresponding example](https://github.com/reinforced/Reinforced.Typings/tree/master/Samples/Difficult/Reinforced.Typings.Samples.Difficult.CodeGenerators). Please refer to it to find out how to use code generators

What is that?
=================

Simply, this thing converts your .NET assemblies to TypeScript code. It integrates to VisualStudio build process and does following tricks:

Exporting ViewModels
-----------------
<table>
<tr><td align="center" width="48%">C#</td><td></td><td align="center"  width="48%">TypeScript</td></tr>
<tr>
	<td>
<pre lang="csharp">
namespace MyApp
{
    using Reinforced.Typings.Attributes;

    [TsInterface]
    public class Order
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
        public bool IsPaid { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
    }
    
    [TsClass]
    public class User
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
    }

    [TsEnum]
    public enum UserType { One, Two }
}
</pre>
</td>
	<td><h1>&#8680;</h1></td>
	<td>
	<pre lang="typescript">
module MyApp {
	export interface IOrder
	{
		ItemName: string;
		Quantity: number;
		Subtotal: number;
		IsPaid: boolean;
		ClientName: string;
		Address: string;
	}
	export class User
	{
		public FirstName: string;
		public Email: string;
		public Type: MyApp.UserType;
	}
	export enum UserType { 
		One = 0, 
		Two = 1, 
	}
}	
	</pre>
	</td>
</tr>
</table>

...even complex ViewModels
-------------
<table>
<tr><td align="center" width="43%">C#</td><td></td><td align="center"  width="48%">TypeScript</td></tr>
<tr>
	<td>
<pre lang="csharp">
namespace MyApp
{
    using Reinforced.Typings.Attributes;

    [TsInterface]
    public class Page
    {
        public List&lt;Order&gt; Orders { get; set; }

        public Dictionary&lt;int, Order&gt; 
                        Cache { get; set; }

        public string[] Tags { get; set; }

        public IEnumerable&lt;object&gt; 
                        Things { get; set; }
    }
}
</pre>
</td>
	<td><h1>&#8680;</h1></td>
	<td>
	<pre lang="typescript">
module MyApp {
	export interface IPage
	{
		Orders: MyApp.IOrder[];
		Cache: { [key:number]: MyApp.IOrder };
		Tags: string[];
		Things: any[];
	}
}	
	</pre>
	</td>
</tr>
</table>

Can disable itself. Or TS compilation. Or both.
-------------
It is needed for some situations. See [RtBypassTypeScriptCompilation](https://github.com/reinforced/Reinforced.Typings/wiki/Reinforced.Typings.settings.xml#RtBypassTypeScriptCompilation) and [RtDisable](https://github.com/reinforced/Reinforced.Typings/wiki/Reinforced.Typings.settings.xml#RtDisable) configuration parameters.

Preserves inheritance
-------------
<table>
<tr><td align="center" width="43%">C#</td><td></td><td align="center"  width="48%">TypeScript</td></tr>
<tr>
	<td>
<pre lang="csharp">
namespace MyApp
{
    using Reinforced.Typings.Attributes;

    public interface INonExport
    {
        string Boom { get; }
    }

    [TsInterface]
    public class WithoutInterface
                : INonExport
    {
        public string Boom { get; set; }
    }

    [TsInterface]
    public interface IEntity
    {
        int Id { get; set; }
    }

    [TsInterface]
    public class User : IEntity
    {
        public int Id { get; set; }

        public string Login { get; set; }
    }
}
</pre>
</td>
	<td><h1>&#8680;</h1></td>
	<td>
	<pre lang="typescript">
module MyApp {
	export interface IWithoutInterface
	{
		Boom: string;
	}
	export interface IEntity
	{
		Id: number;
	}
	export interface IUser extends MyApp.IEntity
	{
		Id: number;
		Login: string;
	}
}	
	</pre>
	</td>
</tr>
</table>

Supports fluent configuration
-------------
Details can be found [on the corresponding wiki page](https://github.com/reinforced/Reinforced.Typings/wiki/Fluent-configuration)
<table>
<tr><td align="center" width="43%">C#</td><td></td><td align="center"  width="48%">TypeScript</td></tr>
<tr>
	<td>
<pre lang="csharp">
namespace MyApp
{
    using Reinforced.Typings.Fluent;
    using System.Web.Mvc;
    
    public class Configuration
    {
        public static void 
            Configure(ConfigurationBuilder builder)
        {
            builder
            	.ExportAsInterface&lt;SelectListItem&gt()
                .OverrideNamespace("MyApp")
                .WithPublicProperties();
        }
    }
}
</pre>
</td>
	<td><h1>&#8680;</h1></td>
	<td>
	<pre lang="typescript">
module MyApp {
	export interface ISelectListItem
	{
		Disabled: boolean;
		Group: any;
		Selected: boolean;
		Text: string;
		Value: string;
	}
}	
	</pre>
	</td>
</tr>
<tr><td align="center" colspan="3">Reinforced.Typings.settings.xml: <code>&lt;RtConfigurationMethod&gt;MyApp.Configuration.Configure&lt;/RtConfigurationMethod&gt;</code></td></tr>
</table>

Generates any custom glue code
-------------
Please obtain full sample code [here](https://github.com/reinforced/Reinforced.Typings/tree/master/Samples/Difficult/Reinforced.Typings.Samples.Difficult.CodeGenerators). It is long to be fully published in readme.

<table>
<tr><td align="center" width="30%">C#</td><td></td><td align="center"  width="48%">TypeScript</td></tr>
<tr>
	<td>
<pre lang="csharp">
namespace MyApp
{
    using Reinforced.Typings.Fluent;
    using System.Web.Mvc;
    
    [TsClass(CodeGeneratorType = typeof(AngularControllerGenerator)]
    public class AngularController : Controller
    {
        [AngularMethod(typeof(SampleResponseModel))]
        public ActionResult Save(Order order)
        {
            return Json(new {
                Message = "Success",
                Success = true
            });
        }
    }
    
    public class AngularMethodAttribute 
            : TsFunctionAttribute
    {
        public AngularMethodAttribute(Type returnType)
        {
            StrongType = returnType;
            CodeGeneratorType = typeof 
             (AngularActionCallGenerator);
        }
    }
    
    public class AngularActionCallGenerator 
            : MethodCodeGenerator
    {
        // too long - see sample
    }
    
    public class AngularControllerGenerator 
            : ClassCodeGenerator
    {
        // too long - see sample
    }
    
    [TsInterface]
    public class SampleResponseModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }    
    }
}
</pre>
</td>
	<td><h1>&#8680;</h1></td>
	<td>
	<pre lang="typescript">
module MyApp {
	export interface ISampleResponseModel
	{
		Message: string;
		Success: boolean;
	}
    
	if (window['app']) {
        window['app'].factory('Api.AngularController', 
        ['$http', 
            ($http: angular.IHttpService) => new AngularController($http)]);
    }
    
	/** Result of AngularControllerGenerator activity */
	export class AngularController
	{
		constructor ($http: angular.IHttpService)
		{
			this.http = $http;
		}
		public Save(order: IOrder) : angular.IPromise&lt;ISampleResponseModel&gt;
		{
			var params = { 'order': order };
			return this.http.post('/Angular/Save', params)
			    .then((response) => { return response.data; });
		}
    }        
}	
	</pre>
	</td>
</tr>
</table>
