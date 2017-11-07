del /q "Reinforced.Typings\bin\*"
FOR /D %%p IN ("Reinforced.Typings\bin\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings\obj\*"
FOR /D %%p IN ("Reinforced.Typings\obj\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Cli\bin\*"
FOR /D %%p IN ("Reinforced.Typings.Cli\bin\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Cli\obj\*"
FOR /D %%p IN ("Reinforced.Typings.Cli\obj\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Integrate\bin\*"
FOR /D %%p IN ("Reinforced.Typings.Integrate\bin\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Integrate\obj\*"
FOR /D %%p IN ("Reinforced.Typings.Integrate\obj\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Tests\bin\*"
FOR /D %%p IN ("Reinforced.Typings.Tests\bin\*.*") DO rmdir "%%p" /s /q

del /q "Reinforced.Typings.Tests\obj\*"
FOR /D %%p IN ("Reinforced.Typings.Tests\obj\*.*") DO rmdir "%%p" /s /q