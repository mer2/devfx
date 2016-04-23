@echo off
setlocal enabledelayedexpansion

set workdir=%1
set target=%2

if "%workdir%"=="" goto error
if "%target%"=="" goto error
pushd %workdir%

::search git path
for /f "tokens=1* delims=_" %%1 in ('reg query "HKLM\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Git_is1" /v "InstallLocation"^| findstr /i "InstallLocation"') do (
  for /f "tokens=1*" %%3  in ("%%~2") do (
    set git_path=%%4
  )
)

if "%git_path%"=="" goto error

set git_path="%git_path%\bin\sh.exe" --login -i
call %git_path% %workdir%gitver.sh

for /f "delims= " %%i in (%workdir%\gitver.txt) do (set rev=%%i)
for /f "tokens=2 delims= " %%i in (%workdir%\gitver.txt) do (set version=%%i)

del /q "%workdir%\gitver.txt">nul

(for /f "tokens=*" %%i in (%target%\AssemblyInfo.Template.cs) do (
	set s=%%i
    set s=!s:$GITREV$=%rev%!
    set s=!s:$GITVERSION$=%version%!
	echo !s!
)) > "%target%\AssemblyInfo.cs"

echo done

goto end

:error
echo error

:end
popd