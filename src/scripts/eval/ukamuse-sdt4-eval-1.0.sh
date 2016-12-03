#!/bin/sh

echo "eval install ukamuse."

mkdir -p /opt/usi_engine/src
cd /opt/usi_engine/src

if [ ! -e ukamuse_sdt4.zip ]; then
  echo "downloading ukamuse_sdt4.zip.."
  wget -q https://github.com/HiraokaTakuya/apery/releases/download/SDT4/ukamuse_sdt4.zip -O ukamuse_sdt4.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi

if [ ! -e ukamuse_sdt4 ]; then
  unzip -o ukamuse_sdt4.zip
fi

mkdir -p /opt/usi_engine/share/ukamuse_sdt4
install -C ukamuse_sdt4/bin/20161007/*.bin /opt/usi_engine/share/ukamuse_sdt4

unlink /opt/usi_engine/share/eval_dir
ln -s /opt/usi_engine/share/ukamuse_sdt4 /opt/usi_engine/share/eval_dir

