@echo off

:build
echo Building solution...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=PackageBaseFx DevFxSolution.sln
if not errorlevel 0 goto error

echo Building NuGet package...
if not exist .\ReleasePackages mkdir .\ReleasePackages
NuGet.exe pack .\BaseFx\BaseFx.csproj -o .\ReleasePackages -sym -Prop Configuration=Release


goto end


:error
echo Build error detected, stopping.


:end
echo Done.
pause