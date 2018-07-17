cd %2

REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
REM ::: Get assembly version from the built .exe						:::
REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

for /F "tokens=* delims=" %%F in ('assver.exe %3') do ( 
  set FileVer=%%F
) 

echo ------------------------------------------------------ >> Build.log
echo The version is %FileVer%>>Build.log

REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
REM ::: Create 7zip file with .EXEs, .DLLs, etc. and the build.log      :::
REM :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

set ZipFile=%4.%FileVer%.7z

del /Q %ZipFile%

IF %5==Release (

	"C:\Program Files (x86)\7-zip\"7z.exe a %ZipFile% %3 %3.config *.dll > tmp.txt
	type tmp.txt >> Build.log
	"C:\Program Files (x86)\7-zip\"7z.exe a %ZipFile% Build.log
	del /Q tmp.txt
	notepad.exe Build.log
)