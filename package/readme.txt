-----------------------
IMPORTANT NOTICE ABOUT Reinforced.Typings.settings.xml FILE
-----------------------

Hi!
Congradulations. You have just installed Reinforced.Typings.

If you see that Reinforced.Typings.settings.xml file is included 
into your project - please close this file and continue working. Everything if fine.

Text below is for .NET Core users who dont get Reinforced.Typings.settings.xml 
automatically copied into root poject folder. Yes, you must see 
Reinforced.Typings.settings.xml in the root of your project along 
with other files. if you don't - please, continue reading.

According to new NuGet's restrictions regarding content files in packages, 
unfortunately I am not able to automatically add Reinfroced.Typings.settings.xml 
to your project. So please do that by yourself. Trust me - it is pretty simple. 
And please forgive NuGet for such a strange design. You can read discussions 
about why they followed that way and lots more stuff about transient content 
restore, or adding specific NuGet command for content, etc. here: 
https://github.com/NuGet/Home/wiki/Bringing-back-content-support,-September-24th,-2015

So now please create empty Reinforced.Typings.settings.xml in the root 
folder of your project and put the following text there:

<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">	
	<PropertyGroup>
	
		<!-- 
			This property points target file where to put generated sources. 
			It is not used if RtDivideTypesAmongFiles='true' specified.
			Important! Do not forget to include generated file to solution.
		-->
		<RtTargetFile>$(ProjectDir)Scripts\project.ts</RtTargetFile>
		
		<!--
			This property sets RT configuration method for fluent configuration.
			If you dont want to use attributes for some reason then here you can
			specify full-qualified configuration method name (including namespace, 
			type and method name, e.g. My.Assembly.Configuration.ConfigureTypings) 
			of method that consumes Reinforce.Typings.Fluent.ConfigurationBuilder type
			and is void (nothing returns). This method will be executed to build 
			types exporting configuration in fluent manner.
			Surely you can continue using attributes. Also fluent configuration methods
			could not provide a way to configure some specific things (generics parameters
			override and classes' TsBaseParam). However in this case fluent configuration
			would be preferred. It means that if member configuration is supplied both 
			in attributes and in fluent methods then configuration from fluent methods will
			be used.
		-->
		<RtConfigurationMethod></RtConfigurationMethod>
		
		<!--
			By default all of your typings will be located in single file specified by RtTargetFile.
			It may be heavy for large projects because you will get single large file containing signinficant
			part of TS sources. It could lead to various problems (e.g. monstrous SCM merges). 
			So here we have RtDivideTypesAmongFiles parameter that will make Reinforced.Typings 
			generate TS sources in C#/Java/somewhat-OO-language-manner (class per file) when set to true.
			
			Important! In case of using this setting, do not forget to add generated files to solution manually.
		-->
		<RtDivideTypesAmongFiles>false</RtDivideTypesAmongFiles>
		
		<!--
			... and if you use RtDivideTypesAmongFiles then please specify target directory
			where to put all generated stuff. Reinforced.Typings will automatically create
			directories structure according to used namespaces.
			Note that in case of using RtDivideTypesAmongFiles, RtTargetFile parameter will 
			NOT be used anymore.
		-->
		<RtTargetDirectory>$(ProjectDir)Scripts\MyApplication</RtTargetDirectory>
		
		
		<!--
			Disables typescript compilation in solution. 
			Use it when your TypeScripts are broken and you need to rebuild project and then regenerate typings 
			to fix it, but TypeScript compilation fails and failing project build. This option will temporary 
			disable typescripts build.
		-->
		<RtBypassTypeScriptCompilation>false</RtBypassTypeScriptCompilation>				
		
		<!--
			Disables Reinforced.Typings generation on build. Use it when it is necessary to temporary disable 
			typings generation.
		-->
		<RtDisable>false</RtDisable>		
		
	</PropertyGroup>
	
	<!--
		If you want Reinforced.Typings to lookup for attributed classes not only current
		project's assembly but also another assembly then you can specify it in RtAdditionalAssembly
		item group. 
		
		Reinforced.Typings receives reference assemblies list from CoreCompile task so you
		can specify here assemblies from your project's references (with or without .dll extension).
		If desired assembly is not in references list of current project then you will have
		to specify full path to it.
	
	<ItemGroup>
		<RtAdditionalAssembly Include="My.Project.Assembly"/>
		<RtAdditionalAssembly Include="My.Project.Assembly.dll"/>
		<RtAdditionalAssembly Include="C:\Full\Path\To\Assembly\My.Project.Assembly.dll"/>
		<RtAdditionalAssembly Include="$(SolutionDir)\AnotherProject\bin\Debug\AnotherProject.dll"/>
	</ItemGroup>
	-->
</Project>