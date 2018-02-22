@ECHO OFF

SET UnityProject=..\Doc

SET ProjectPath=%1
SET TargetPath=%2
SET TargetName=%3

%ProjectPath%\..\Dependencies\Tools\pdb2mdb.exe %TargetPath%

rem xcopy  %TargetPath%\..\%TargetName%.dll       %UnityProject%\    /v  /D  /s  /y 
rem xcopy  %TargetPath%\..\%TargetName%.dll.mdb   %UnityProject%\    /v  /D  /s  /y 
rem xcopy  %TargetPath%\..\%TargetName%.XML       %UnityProject%\    /v  /D  /s  /y 