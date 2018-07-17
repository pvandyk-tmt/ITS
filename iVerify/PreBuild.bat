
setlocal enabledelayedexpansion
set assemblypath=Src\Properties

REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
REM ::: Create Build log with date & time & build computer information  :::
REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

echo Built %DATE% at %TIME% on computer %COMPUTERNAME% > %2Build.log
echo ------------------------------------------------------ >> %2Build.log

REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
REM ::: Substitute the "$WCREV$" and "$WCDATE$" tags in AssemblyInfo.cs :::
REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

SubWCRev.exe . AssemblyInfo.cs %assemblypath%\AssemblyInfo.cs.temp >> %2Build.log

REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
REM ::: If built on release folder strip the '-beta' off the build number::
REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

IF EXIST ..\..\%4. (

    del /Q %assemblypath%\AssemblyInfo.cs

	for /f "tokens=* delims=" %%a in (%assemblypath%\AssemblyInfo.cs.temp) do (

	   set write=%%a
	   set write=!write:-beta=!

	   echo !write! 
	   echo !write! >> %assemblypath%\AssemblyInfo.cs
	)

) ELSE (

    copy /Y %assemblypath%\AssemblyInfo.cs.temp %assemblypath%\AssemblyInfo.cs
)

del /Q %assemblypath%\AssemblyInfo.cs.temp

if %5 == Release notepad.exe %assemblypath%\AssemblyInfo.cs
