@echo off

:build
echo Building solution...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=PackageDevFx DevFxSolution.sln
if not errorlevel 0 goto error

echo Building NuGet package...
if not exist .\ReleasePackages mkdir .\ReleasePackages
del .\DevFx\bin\Release\HTB.DevFx.BaseFx.*
NuGet.exe pack .\DevFx\DevFx.csproj -o .\ReleasePackages -sym -Prop Configuration=Release


goto end


:error
echo Build error detected, stopping.


:end
echo Done.
pause