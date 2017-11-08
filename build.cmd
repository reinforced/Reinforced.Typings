call cleanup.cmd

cd package

del /q "build\*"
FOR /D %%p IN ("build\*.*") DO rmdir "%%p" /s /q

del /q "buildMultiTargeting\*"
FOR /D %%p IN ("buildMultiTargeting\*.*") DO rmdir "%%p" /s /q

del /q "content\*"
del /q "contentFiles\*"
FOR /D %%p IN ("contentFiles\*.*") DO rmdir "%%p" /s /q

del /q "lib\*"
FOR /D %%p IN ("lib\*.*") DO rmdir "%%p" /s /q

del /q "tools\*"
FOR /D %%p IN ("tools\*.*") DO rmdir "%%p" /s /q

cd ..

cd Reinforced.Typings.Integrate
dotnet restore Reinforced.Typings.Integrate.NETCore.csproj --no-cache
dotnet build Reinforced.Typings.Integrate.NETCore.csproj /p:Configuration=Release /verbosity:m /p:WarningLevel=0
cd ..

cd Reinforced.Typings.Cli

dotnet publish Reinforced.Typings.Cli.NETCore.csproj -o ../package/tools/net45 -f net45 -c Release
dotnet publish Reinforced.Typings.Cli.NETCore.csproj -o ../package/tools/netcoreapp1.0 -f netcoreapp1.0 -c Release
dotnet publish Reinforced.Typings.Cli.NETCore.csproj -o ../package/tools/netcoreapp2.0 -f netcoreapp2.0 -c Release

cd ..

xcopy xmls\Reinforced.Typings.settings.xml package\content\ /I /Y

xcopy xmls\Reinforced.Typings.targets package\build\ /I /Y
xcopy xmls\Reinforced.Typings.Multi.targets package\buildMultiTargeting\ /I /Y

xcopy xmls\Reinforced.Typings.props package\build\ /I /Y
xcopy xmls\Reinforced.Typings.props package\buildMultiTargeting\ /I /Y

rem lib
xcopy Reinforced.Typings\bin\Release\*.* package\lib\ /E /I /Y

rem build
xcopy Reinforced.Typings.Integrate\bin\Release\net45\Reinforced.Typings.Integrate.dll package\build\net45\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\netstandard1.6\*.* package\build\netstandard1.6\ /I /Y

rem package
nuget pack package\Reinforced.Typings.nuspec -BasePath package




