@set CUR_PATH=%~dp0
@%CUR_PATH%premake5_Window.exe --file=%CUR_PATH%premake5.lua --dotnet=mono vs2017
PAUSE