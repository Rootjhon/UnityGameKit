@ECHO OFF

SET CUR_PATH=%~dp0/CI/premake/

ECHO Support: vs2005 vs2008 vs2010 vs2012 vs2013 vs2015 vs2017 gmake gmake2 codelite xcode4
SET /p vsVersion="TargetVersion: "

%CUR_PATH%premake5_Window.exe --os=windows --file=%CUR_PATH%premake5.lua --dotnet=mono %vsVersion%