#!/bin/bash

# Make cache dir
if [ ! -d "$HOME/cache" ]; then
    mkdir -m 777 "$HOME/cache"
fi

BASE_URL=http://netstorage.unity3d.com/unity
VERSION="2017.3.0f3"

# Find hash on the html of the unity archives
tmp=$(curl -s -i -X GET https://unity3d.com/fr/get-unity/download/archive)
re="https:\/\/netstorage\.unity3d\.com\/unity\/(.+?)\/MacEditorInstaller\/Unity-$VERSION\.pkg"
echo $re

HASH=a9f86dcd79df

echo "version:$VERSION - hash:$HASH"

function download()
{
	url="$1"
	FILENAME=$(basename "$url")

	if [ ! -f "$HOME/cache/$FILENAME" ]; then
		echo "Downloading Unity from $url: "
		curl --retry 5 -o "$HOME/cache/$FILENAME" "$url"
	fi
}

function install() 
{
	package="$1"
	url="$BASE_URL/$HASH/$package"

	download "$url"

	echo "Installing "$(basename "$package")
	sudo installer -dumplog -package $(basename "$package") -target /
}

# See $BASE_URL/$HASH/unity-$VERSION-$PLATFORM.ini for complete list
install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Mac-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Android-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-$VERSION.pkg"