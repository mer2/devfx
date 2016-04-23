@echo off
set ProjectName=BaseFx
set PackagesDir=..\ReleasePackages
set NuGetPath=..\.nuget

:nuget
echo Building NuGet package...
if not exist %PackagesDir% mkdir %PackagesDir%
%NuGetPath%\NuGet.exe pack %ProjectName%.csproj -o %PackagesDir% -sym -Prop Configuration=Release -Build

goto end

:error
echo Build error detected, stopping.

:end
echo Done.
pause