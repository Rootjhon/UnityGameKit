#!/bin/bash
CUR_PATH="$(cd $(dirname $0); pwd)/CI/premake/"
chmod 777 ${CUR_PATH}/premake5_MacOS


${CUR_PATH}/premake5_MacOS --os=macosx --dotnet=mono --file=${CUR_PATH}/premake5.lua gmake
