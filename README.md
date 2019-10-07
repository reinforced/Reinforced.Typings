What is that?
=================

Reinforced.Typings converts your .NET assemblies to TypeScript code. It integrates to VisualStudio build process and simply does its job according to configuration. Please check out [documentation](https://github.com/reinforced/Reinforced.Typings/wiki) to discover numbers of useful features (type substitutions, modules, code generators, fluent configuration, multi-file export, JSDOC). 

Reinforced.Typings is available on [NuGet](https://www.nuget.org/packages/Reinforced.Typings/).
=================
```sh
PM> Install-Package Reinforced.Typings
```

**Find out detailed information in Reinforced.Typings [wiki](https://github.com/reinforced/Reinforced.Typings/wiki)**

News
=================
> :gift: Version **1.5.4** released

* .NET Core 3.0 version
* Bugfixes
* Finally I have made detailed [code generators and customization guide](https://github.com/reinforced/Reinforced.Typings/wiki/Code-Generators). Check it!

Job needed
=================
The power of community, please help me. I'm looking for remote job as C#/.NET Fullstack Engineer. I have almost 10 years of remote experience, here is my [UpWork Profile](https://www.upwork.com/o/profiles/users/_~01d070a561f288ffe7/) covering last 5 years. My code is... well... can be found at least within this repo. Price is negotiable. Contact me (or tell your manager to) [by email](mailto:pavel.b.novikov@gmail.com) or by skype (nsu_the_cjay). Thanks for attention!

Support policy
=================

Please **do not** ask your questions in github issues anymore. Such format is not suitable for storing FAQ. If you have question - please go to StackOverflow and ask it there. Tag your question with [reinforced-typings](https://stackoverflow.com/questions/tagged/reinforced-typings) tag. I watch full list of questions and will answer ASAP. Make experience that you've got available for other users! 

**UPD**: You can notify me about question by sending link via Twitter ([@reinforced_sc](https://twitter.com/reinforced_sc)) to get answer faster.

GitHub issues are for confirmed bugs/feature requests now. If you've found bug - please write and PR test if you can. If you have feature idea - please describe it from fluent/attribute configuration point of view. Describe how'd you gonna to configure RT for desired result. Thanks in advance!

Best to be used for
=================

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
        public List<Order> Orders { get; set; }

        public Dictionary<int, Order> 
                        Cache { get; set; }

        public string[] Tags { get; set; }

        public IEnumerable<object> 
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

Temporary disabling TypeScript compilation in your project
-------------
Now you will not stay powerless when generated typings fail your TypeScript build in project. See [RtBypassTypeScriptCompilation](https://github.com/reinforced/Reinforced.Typings/wiki/Reinforced.Typings.settings.xml#RtBypassTypeScriptCompilation) configuration parameter.

Inheritance preservation
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

Use fluent configuration
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
            	.ExportAsInterface<SelectListItem>()
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

Generate any custom glue code
-------------
Read more [here](https://github.com/reinforced/Reinforced.Typings/wiki#writing-custom-code-generators).

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
