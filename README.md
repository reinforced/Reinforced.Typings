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
> ⚙ Version **1.5.2** released

* Visitor overrides
* Documentation bugfixes

Support policy
=================

Please **do not** ask your questions in github issues anymore. Such format is not suitable for storing FAQ. If you have question - please go to StackOverflow and ask it there. Tag your question with [reinforced-typings](https://stackoverflow.com/questions/tagged/reinforced-typings) tag. I watch full list of questions and will answer ASAP. Make experience that you've got available for other users! 

**UPD**: You can notify me about question by sending link via Twitter ([@MurcielagoCat](https://twitter.com/MurcielagoCat)) to get answer faster.

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

dotnet core linux quickstart
=======

- add package reference to the project

```sh
dotnet add package Reinforced.Typings
```

- create an helper `rtcli.sh` as follow ( change version as needed ) then `chmod +x rtcli.sh`

```sh
#!/bin/bash
dotnet ~/.nuget/packages/reinforced.typings/1.5.2/tools/netcoreapp2.1/rtcli.dll $@
```

- apply `[TsInterface]` attributes to csharp classes to export and run

- build copying related dlls within `dotnet build /p:CopyLocalLockFileAssemblies=true`

- run the tool

```sh
./rtcli.sh \
	SourceAssemblies="fullpath-project/bin/Debug/netcoreapp2.1/mylib.dll" \
	TargetDirectory="fullpath-target-directory" \
	Hierarchy="true"
```
