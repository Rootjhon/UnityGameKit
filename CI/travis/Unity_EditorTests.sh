#!/bin/bash

echo "Unity-Editor Testing..."

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-runTests \
	-projectPath "$(pwd)" \
	-testResults "$(pwd)"/results.xml \
	-testPlatform editmode

echo 'Unity-Editor Test Result:'
cat "$(pwd)"/results.xml