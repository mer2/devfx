#!/bin/bash
VER_FILE=gitver.txt
LOCALREV=`git rev-list HEAD | wc -l | awk '{print $1}'`
VER="$(git rev-list HEAD -n 1 | cut -c 1-8)"
VER=_$VER
GIT_VERSION="$LOCALREV r$LOCALREV$VER"
echo $GIT_VERSION>$VER_FILE
echo 'Done'