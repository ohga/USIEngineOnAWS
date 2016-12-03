#!/bin/zsh

DIST_DIR=/tmp/dist
SOURCE_DIR=~/USIEngineOnAWS
FILE_DATE="201612060000"

rm -Rf ${DIST_DIR}
mkdir -p ${DIST_DIR}/USIEngineOnAWS

cd ${DIST_DIR}/USIEngineOnAWS

cp ${SOURCE_DIR}/src/USIEngineOnAWS/bin/Release/USIEngineOnAWS.exe USIEngineOnAWS.exe \
  && cp ${SOURCE_DIR}/src/USIEngineOnAWS/bin/Release/AWSSDK.Core.dll AWSSDK.Core.dll  \
  && cp ${SOURCE_DIR}/src/USIEngineOnAWS/bin/Release/AWSSDK.EC2.dll AWSSDK.EC2.dll  \
  && cp ${SOURCE_DIR}/src/USIEngineOnAWS/bin/Release/Renci.SshNet.dll Renci.SshNet.dll  \
  && cp -a ${SOURCE_DIR}/src/resource . \
  && cp ${SOURCE_DIR}/src/USIEngineViaSSH/bin/Release/USIEngineViaSSH.exe resource/USIEngineViaSSH.ex_ \
  && cp ${SOURCE_DIR}/src/AWSSDK.LICENSE ${SOURCE_DIR}/src/Renci.SshNet.LICENSE ${SOURCE_DIR}/LICENSE . \
  && touch -t ${FILE_DATE} ${SOURCE_DIR}/src/scripts/**/* \
  && tar zcf resource/setup.tar.gz -C ${SOURCE_DIR}/src/scripts bootstrap.sh engine eval 
if [ $? != 0 ]; then
  echo "ERROR: package."
  exit 1
fi

cd ${DIST_DIR} 
git clone https://github.com/ohga/USIEngineOnAWS.git tmp
cp -a tmp/src USIEngineOnAWS
rm -Rf tmp

touch -t ${FILE_DATE} **/* && zip -r -q USIEngineOnAWS.zip USIEngineOnAWS 
if [ $? != 0 ]; then
  echo "ERROR: zip."
  exit 1
fi

rm -Rf ${DIST_DIR}/USIEngineOnAWS
echo "---> ${DIST_DIR}/USIEngineOnAWS.zip created"


