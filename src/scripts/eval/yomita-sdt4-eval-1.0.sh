#!/bin/sh

echo "eval install yomita."

mkdir -p /opt/usi_engine/src
cd /opt/usi_engine/src

if [ ! -e yomita_sdt4.zip ]; then
  echo "downloading yomita_sdt4.zip.."
  wget -q https://github.com/TukamotoRyuzo/Yomita/releases/download/0/yomita_sdt4.zip -O yomita_sdt4.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi

if [ ! -e yomita_sdt4 ]; then
  unzip -o yomita_sdt4.zip
fi

mkdir -p /opt/usi_engine/share/yomita_sdt4
install -C yomita_sdt4/eval/kppt_file/SDT4/*.bin /opt/usi_engine/share/yomita_sdt4

unlink /opt/usi_engine/share/eval_dir
ln -s /opt/usi_engine/share/yomita_sdt4 /opt/usi_engine/share/eval_dir

