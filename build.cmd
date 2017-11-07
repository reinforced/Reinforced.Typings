call cleanup.cmd

cd Reinforced.Typings.Integrate
dotnet restore Reinforced.Typings.Integrate.NETCore.csproj --no-cache
dotnet msbuild Reinforced.Typings.Integrate.NETCore.csproj /p:Configuration=Release /verbosity:m /p:WarningLevel=0
cd ../package

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

xcopy xmls\Reinforced.Typings.settings.xml package\content\ /I /Y

xcopy xmls\Reinforced.Typings.targets package\build\ /I /Y
xcopy xmls\Reinforced.Typings.Multi.targets package\buildMultiTargeting\ /I /Y

xcopy xmls\Reinforced.Typings.props package\build\ /I /Y
xcopy xmls\Reinforced.Typings.props package\buildMultiTargeting\ /I /Y

rem lib
xcopy Reinforced.Typings\bin\Release\*.* package\lib\ /E /I /Y

rem build
xcopy Reinforced.Typings.Integrate\bin\Release\net45\*.dll package\build\net45\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\net45\*.exe package\build\net45\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\netcoreapp1.0\*.dll package\build\netcoreapp1.0\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\netcoreapp2.0\*.dll package\build\netcoreapp2.0\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\netstandard1.5\*.dll package\build\netstandard1.5\ /I /Y
xcopy Reinforced.Typings.Integrate\bin\Release\netstandard2.0\*.dll package\build\netstandard2.0\ /I /Y

rem tools
xcopy Reinforced.Typings.Cli\bin\Release\net45\*.dll package\tools\net45\ /I /Y
xcopy Reinforced.Typings.Cli\bin\Release\net45\*.exe package\tools\net45\ /I /Y
xcopy Reinforced.Typings.Cli\bin\Release\netcoreapp1.0\*.dll package\tools\netcoreapp1.0\ /I /Y
xcopy Reinforced.Typings.Cli\bin\Release\netcoreapp2.0\*.dll package\tools\netcoreapp2.0\ /I /Y
xcopy Reinforced.Typings.Cli\bin\Release\netstandard1.5\*.dll package\tools\netstandard1.5\ /I /Y
xcopy Reinforced.Typings.Cli\bin\Release\netstandard2.0\*.dll package\tools\netstandard2.0\ /I /Y

rem package
nuget pack package\Reinforced.Typings.nuspec -BasePath package




