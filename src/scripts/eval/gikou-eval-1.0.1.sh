#!/bin/sh

echo "eval install gikou."

mkdir -p /opt/usi_engine/src
cd /opt/usi_engine/src

if [ ! -e gikou_win_20160606.zip ]; then
  echo "downloading gikou_win_20160606.zip.."
  wget -q https://github.com/gikou-official/Gikou/releases/download/v1.0.1/gikou_win_20160606.zip -O gikou_win_20160606.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi

if [ ! -e gikou_win_20160606 ]; then
  unzip -o gikou_win_20160606.zip
  rm -Rf __MACOSX
fi

mkdir -p /opt/usi_engine/share/gikou/20160606 
install -C gikou_win_20160606/*.bin /opt/usi_engine/share/gikou/20160606 
 
unlink /opt/usi_engine/share/eval_dir
ln -s /opt/usi_engine/share/gikou/20160606 /opt/usi_engine/share/eval_dir

