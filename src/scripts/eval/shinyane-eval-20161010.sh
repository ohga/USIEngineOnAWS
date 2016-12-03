#!/bin/sh

echo "eval install ShinYane20161010."

mkdir -p /opt/usi_engine/src
cd /opt/usi_engine/src

if [ ! -e ShinYane20161010.zip ]; then
  echo "downloading ShinYane20161010.zip.."
  URL=`wget --keep-session-cookies --save-cookies=/tmp/cookies.txt \
    'https://docs.google.com/uc?export=download&id=0ByIGrGAuSfHHVVh0bEhxRHNpcGc' -q -O - \
    | perl -nle 'if($_=~/download-link.*?href="(.*?)"/i){$str=$1;$str=~s/&amp;/&/g;print "https://docs.google.com$str";}'`
  wget -q --load-cookies /tmp/cookies.txt $URL -O ShinYane20161010.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
  wget -q "https://docs.google.com/uc?export=download&id=0ByIGrGAuSfHHcXRrc2FmdHVmRzA" -O yaneura_book3.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi
if [ ! -e ShinYane20161010 ]; then
  mkdir ShinYane20161010
  cd ShinYane20161010
  unzip -o ../ShinYane20161010.zip
  unzip -o ../yaneura_book3.zip
  cd ..
fi

mkdir -p /opt/usi_engine/share/ShinYane/20161010
install -C ShinYane20161010/*.bin /opt/usi_engine/share/ShinYane/20161010
install -C ShinYane20161010/yaneura_book3.db /opt/usi_engine/share/ShinYane/20161010
 
unlink /opt/usi_engine/share/eval_dir
ln -s /opt/usi_engine/share/ShinYane/20161010 /opt/usi_engine/share/eval_dir

