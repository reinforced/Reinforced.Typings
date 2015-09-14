Reinforce.Typings is available on [NuGet](https://www.nuget.org/packages/Reinforced.Typings/)
-------------

```sh
PM> Install-Package Reinforced.Typings
```

What is that?
-------------

It is automated TypeScript sources generator from your MVC project's assembly. It is a way to describe your server types (typically, View models being passed to client via JSON) in TypeScript typings. Also it allows to generate glue code e.g. for controller methods.

Could you explain it?
-------------

Okay, well. Let's consider typical ASP.NET MVC web application. If it is AJAX-powered then you have to make requests to server and send/receive JSON data. Typically you receive/send from/to server kind of JSON-ed ViewModel classes that expose data from database/client input. Since TypeScript has been invented, there is "strongly-typed" way to write your javascript that perforь such kind of operations. So basically you better to have typings (or interfaces) for server View-models on client-side. Of course you can write them manually. 

And with Reinforced.Typings you only have to mark classes you want to have in TypeScript code with corresponding attribute - and them will be exported to specified TypeScript file after next build. Each build. Automatically.

Another example comes from MVC controlles invocation. For example, MVC WebAPI brings you a way to call server methods via accessing corresponding URLs and passing parameters via GET/POST. If you wish to perform controller method call from javascript/TypeScript then you have to use XMLHttpRequest or $.ajax or $http.get or whatever you use. Anyway, you have to write something like:

###### This
```javascript
	// Server URL may vary
	$http.get('Orders/List')
		.success(function(data){ /* handling */ })
		.error(function(){ /* handling */	});
```

###### When more comfortable is this
```javascript
	// Orders instance exposes all strongly-typed controller methods
	// List() return boilerplate typed promise
	// Controller TypeScript specification gets automatically updated when server-side code changes
	Orders.List()
		.success(function(data:IOrder[]){ /* handling */ })
		.error(function(){ /* handling */ });
```

With Reinforced.Typings it is easy to write small boilerplate code generator, mark necessary controllers with corresponding attribute and you will get boilerplate TypeScript glue code for controller methods invocation after next build. And it is kept updated. Each build. Awesome.

How to use it?
-------------

Start with installing Reinforced.Typings package from [NuGet](https://www.nuget.org/packages/Reinforced.Typings/)

```sh
PM> Install-Package Reinforced.Typings
```

It will add Reinforced.Typings references to you project, some msbuild files and Reinforced.Typings.settings.xml at the root of your project.

1. Equip your server types you want in TypeScript with TsClass, TsInterface or TsEnum attribute.
1. Adjust generation settings in Reinforced.Typings.settings.xml (if necessary)
1. Rebuild your project.
1. Add generated files to your solution.
1. Enjoy.

Documentation and samples
-------------

Detailed documentation and usage samples is coming. For now Reinforced.Typings has pretty XMLDOC comments so feel free to use it. 
Also you can star this project to make documentation appear faster :)