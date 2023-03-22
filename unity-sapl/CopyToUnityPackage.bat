@ECHO OFF
SETLOCAL
SET "targetdir=../unity-sapl-package/Runtime"
SET "keepfile1=FTK.sapl_unity.asmdef"
SET "keepfile2=FTK.sapl_unity.asmdef.meta"
SET "keepdir=keep me"

FOR /d %%a IN ("%targetdir%\*") DO IF /i NOT "%%~nxa"=="%keepdir%" RD /S /Q "%%a"
FOR %%a IN ("%targetdir%\*") DO IF /i NOT "%%~nxa"=="%keepfile1%" IF /i NOT "%%~nxa"=="%keepfile2%" DEL "%%a"

robocopy "./Assets/Sapl" "../unity-sapl-package/Runtime" /s
if %errorlevel% leq 1 exit 0 else exit %errorlevel%