@echo off

:build
echo Building solution...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=PackageData DevFxSolution.sln
if not errorlevel 0 goto error

echo Building NuGet package...
if not exist .\ReleasePackages mkdir .\ReleasePackages
del .\Data\bin\Release\HTB.DevFx.BaseFx.*
del .\Data\bin\Release\HTB.DevFx.???
NuGet.exe pack .\Data\Data.csproj -o .\ReleasePackages -sym -Prop Configuration=Release


goto end


:error
echo Build error detected, stopping.


:end
echo Done.
pause