#!/bin/sh

echo "eval install tanuki-sdt4."

mkdir -p /opt/usi_engine/src
cd /opt/usi_engine/src

if [ ! -e tanuki-sdt4-2016-10-09.7z ]; then
  echo "downloading tanuki-sdt4-2016-10-09.7z.."
  wget -q https://github.com/nodchip/hakubishin-/releases/download/tanuki-sdt4-2016-10-09/tanuki-sdt4-2016-10-09.7z -O tanuki-sdt4-2016-10-09.7z
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi

if [ ! -e tanuki-sdt4 ]; then
  mkdir tanuki-sdt4
  cd tanuki-sdt4
  7z x -y ../tanuki-sdt4-2016-10-09.7z
  cd ..
fi

mkdir -p /opt/usi_engine/share/tanuki-sdt4
install -C tanuki-sdt4/eval/*.bin /opt/usi_engine/share/tanuki-sdt4
install -C tanuki-sdt4/book/*.db /opt/usi_engine/share/tanuki-sdt4

unlink /opt/usi_engine/share/eval_dir
ln -s /opt/usi_engine/share/tanuki-sdt4 /opt/usi_engine/share/eval_dir

