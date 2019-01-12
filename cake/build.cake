#addin "Cake.FileHelpers"
var target = Argument("target", "Build");
const string version = "1.5.1";

Task("Clean")
  .Does(() =>
{
  CleanDirectories("../Reinforced.Typings*/**/bin");
  CleanDirectories("../Reinforced.Typings*/**/obj");

  Information("Clean completed");
});

const string packageRoot = "../package";
const string licenseRoot = "../package/license";
const string toolsPath = "../package/tools";
const string contentPath = "../package/content";
const string buildPath = "../package/build";
const string multiTargetPath = "../package/buildMultiTargeting";
const string libPath = "../package/lib";
const string RELEASE = "Release";
const string NETCORE22 = "netcoreapp2.2";
const string NETCORE21 = "netcoreapp2.1";
const string NETSTANDARD16 = "netstandard1.6";
const string NETSTANDARD15 = "netstandard1.5";
const string NETSTANDARD20 = "netstandard2.0";
const string NETCORE20 = "netcoreapp2.0";
const string NETCORE10 = "netcoreapp1.0";
const string NETCORE11 = "netcoreapp1.1";
const string NET461 = "net461";
const string NET46 = "net46";
const string NET45 = "net45";

var cliFrameworks = new[] { NETCORE10, NETCORE11, NET45, NET461,NETCORE20,NETCORE21,NETCORE22}; 
var rtFrameworks = new[]  { NETCORE10, NETCORE11, NETSTANDARD15,NETSTANDARD20,NETCORE20,NETCORE21,NETCORE22,NET45, NET461};
var taskFrameworks = new[] { NET46, NETSTANDARD20};

var netCore = new HashSet<string>(new[]{NETSTANDARD15,NETSTANDARD20,NETCORE10,NETCORE11,NETCORE20,NETCORE21,NETCORE22});

const string CliNetCoreProject = "../Reinforced.Typings.Cli/Reinforced.Typings.Cli.NETCore.csproj";
const string RtNetCoreProject = "../Reinforced.Typings/Reinforced.Typings.NETCore.csproj";
const string IntegrateProject = "../Reinforced.Typings.Integrate/Reinforced.Typings.Integrate.NETCore.csproj";
const string tfParameter = "TargetFrameworks";
string tfRgx = $"<{tfParameter}>[a-zA-Z0-9;.]*</{tfParameter}>"; 
const string tfSingleParameter = "TargetFramework";
string tfsRgx = $"<{tfSingleParameter}>[a-zA-Z0-9;.]*</{tfSingleParameter}>"; 

Task("PackageClean")
  .Description("Cleaning temporary package folder")
  .Does(() =>
{
  CleanDirectory(packageRoot); EnsureDirectoryExists(packageRoot);
  CleanDirectory(licenseRoot); EnsureDirectoryExists(licenseRoot);
  CleanDirectory(toolsPath); EnsureDirectoryExists(toolsPath);
  CleanDirectory(contentPath); EnsureDirectoryExists(contentPath);
  CleanDirectory(buildPath); EnsureDirectoryExists(buildPath);
  CleanDirectory(multiTargetPath); EnsureDirectoryExists(multiTargetPath);
  CleanDirectory(libPath); EnsureDirectoryExists(libPath);
});

Task("UpdateVersions")
.Description("Updating assembly/file versions")
.Does(()=>{
 // Update versions
  foreach(var p in new[]{CliNetCoreProject,RtNetCoreProject,IntegrateProject}){
    foreach(var par in new[]{"AssemblyVersion","FileVersion","InformationalVersion"}){
      var rgx = $"<{par}>[0-9.]*</{par}>";
      ReplaceRegexInFiles(p,rgx,$"<{par}>{version}</{par}>");
    }    
  }
});

Task("BuildIntegrate")
.Description("Building RT's integration MSBuild task")
.Does(()=>{
  foreach(var fw in taskFrameworks){
	  DotNetCoreMSBuildSettings mbs = null;
          
      if (netCore.Contains(fw)){
        mbs = new DotNetCoreMSBuildSettings()
          .WithProperty("RtAdditionalConstants","NETCORE;" + fw.ToUpperInvariant().Replace(".","_"))
          .WithProperty("RtNetCore","True");
      }
    DotNetCorePublish(IntegrateProject, new DotNetCorePublishSettings
    {
      Verbosity = DotNetCoreVerbosity.Quiet,
      Configuration = RELEASE,	  
      MSBuildSettings = mbs,
      OutputDirectory = System.IO.Path.Combine(buildPath, fw),
      Framework = fw
    });    
    
  }  
  
});

Task("Build")
  .IsDependentOn("Clean")
  .IsDependentOn("PackageClean")
  .IsDependentOn("UpdateVersions")
  .IsDependentOn("BuildIntegrate")
  .Does(() =>
{
  // Build various versions of CLI tool
  foreach(var fw in cliFrameworks){
      Information("---------");
      Information("Building CLI for {0}",fw);
      Information("---------");

      ReplaceRegexInFiles(CliNetCoreProject,tfRgx,$"<{tfParameter}>{fw}</{tfParameter}>");       
      ReplaceRegexInFiles(RtNetCoreProject,tfRgx,$"<{tfParameter}>{fw}</{tfParameter}>"); 
      ReplaceRegexInFiles(CliNetCoreProject,tfsRgx,$"<{tfSingleParameter}>{fw}</{tfSingleParameter}>");       
      ReplaceRegexInFiles(RtNetCoreProject,tfsRgx,$"<{tfSingleParameter}>{fw}</{tfSingleParameter}>"); 

      DotNetCoreMSBuildSettings mbs = null;
          
      if (netCore.Contains(fw)){
        mbs = new DotNetCoreMSBuildSettings()
          .WithProperty("RtAdditionalConstants","NETCORE;" + fw.ToUpperInvariant().Replace(".","_"))
          .WithProperty("RtNetCore","True");
      }
      DotNetCorePublish(CliNetCoreProject, new DotNetCorePublishSettings {  
        Configuration = RELEASE, 
        MSBuildSettings = mbs,
        Framework = fw,
        Verbosity = DotNetCoreVerbosity.Quiet,
        OutputDirectory = System.IO.Path.Combine(toolsPath, fw)        
      });
  }


  // Build various versions of lib
  foreach(var fw in rtFrameworks){
      Information("---------");
      Information("Building lib for {0}",fw);
      Information("---------");

      ReplaceRegexInFiles(RtNetCoreProject,tfRgx,$"<{tfParameter}>{fw}</{tfParameter}>");  
      ReplaceRegexInFiles(RtNetCoreProject,tfsRgx,$"<{tfSingleParameter}>{fw}</{tfSingleParameter}>"); 
      
      var mbs = new DotNetCoreMSBuildSettings()
          .WithProperty("DocumentationFile",$@"bin\Release\{fw}\Reinforced.Typings.xml");

      if (netCore.Contains(fw)){
        mbs = mbs
          .WithProperty("RtAdditionalConstants","NETCORE;" + fw.ToUpperInvariant().Replace(".","_"))
          .WithProperty("RtNetCore","True");
      }
     DotNetCorePublish(RtNetCoreProject, new DotNetCorePublishSettings {  
        Configuration = RELEASE,
        MSBuildSettings = mbs,    
        Framework = fw,
        Verbosity = DotNetCoreVerbosity.Quiet,
        OutputDirectory = System.IO.Path.Combine(libPath, fw)
      });
  }

  
  
  Information("---------");
  Information("Copying build stuff");
  Information("---------");

  // Copy build stuff
  CopyFileToDirectory("../stuff/Reinforced.Typings.settings.xml", contentPath);
  CopyFileToDirectory("../stuff/Reinforced.Typings.targets", buildPath);
  CopyFileToDirectory("../stuff/Reinforced.Typings.Multi.targets", multiTargetPath);

  Information("---------");
  Information("Writing readme");
  Information("---------");
  
  CopyFileToDirectory("../stuff/license.txt", licenseRoot);

  // Copy readme with actual version of Reinforced.Typings.settings.xml
  CopyFileToDirectory("../stuff/readme.txt", packageRoot);
  using(var tr = System.IO.File.OpenRead("../stuff/Reinforced.Typings.settings.xml"))
  using(var tw = new System.IO.FileStream(System.IO.Path.Combine(packageRoot,"readme.txt"),FileMode.Append))
  {
    tr.CopyTo(tw);
  }

  Information("---------");
  Information("Updating nuspec");
  Information("---------");
  // Copy nuspec
  CopyFileToDirectory("../stuff/Reinforced.Typings.nuspec", packageRoot);
  ReplaceTextInFiles("../package/*.nuspec","$$VERSION$$",version);
  var rn = string.Empty;
  if (System.IO.File.Exists(System.IO.Path.Combine("../stuff/relnotes", version) + ".md")){
        rn = System.IO.File.ReadAllText(System.IO.Path.Combine("../stuff/relnotes", version) + ".md");
  }

  ReplaceTextInFiles("../package/*.nuspec","$$RELNOTES$$",rn);

  Information("---------");
  Information("Packaging");
  Information("---------");
  NuGetPack("../package/Reinforced.Typings.nuspec", new NuGetPackSettings {
    BasePath = "../package",
    OutputDirectory = "../"
  });

  Information("Build complete");
});

RunTarget(target);