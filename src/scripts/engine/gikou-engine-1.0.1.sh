#!/bin/sh

MAKE_OPT="release"
if [ "x$1" != "x" ]; then
  MAKE_OPT=$1
fi
CORE_CNT=`grep '^processor\W' /proc/cpuinfo | tail -1  | awk -F':' '{print ($2 + 1);}'`

echo "engine install gikou v1.0.1."

BUILD_ROOT=/opt/build/src
DIST_ROOT=/opt/build/dist
SCRIPT_ROOT=`pwd`

rm -Rf ${DIST_ROOT}
mkdir -p ${BUILD_ROOT} ${DIST_ROOT}/opt/usi_engine/bin ${DIST_ROOT}/opt/usi_engine/share/script

cd ${BUILD_ROOT} 

if [ ! -e Gikou-v1.0.1.zip ]; then
  echo "downloading Gikou-v1.0.1.zip.."
  wget -q https://github.com/gikou-official/Gikou/archive/v1.0.1.zip -O Gikou-v1.0.1.zip
  if [ $? != 0 ]; then
    echo "download error."
    exit 1
  fi
fi

rm -Rf Gikou-1.0.1
unzip -o Gikou-v1.0.1.zip

cd Gikou-1.0.1 && \
  sed -i -e "s#\(\(params\|book\|probability\|progress\)\.bin\)#/opt/usi_engine/share/eval_dir/\1#g" src/*.cc && \
  patch -p1 < ${SCRIPT_ROOT}/engine/gikou-engine-quit_command.patch && \
  make -j ${CORE_CNT} ${MAKE_OPT} && \
  mv bin/release bin/gikou-v1.0.1 && \
  install -s bin/gikou-v1.0.1 ${DIST_ROOT}/opt/usi_engine/bin
if [ $? != 0 ]; then
  echo "build error."
  exit 1
fi

AFTER_INSTALL=${DIST_ROOT}/opt/usi_engine/share/script/after_install_gikou-1.0.1.sh
echo "#!/bin/sh" > ${AFTER_INSTALL}
echo "unlink /opt/usi_engine/bin/engine" >> ${AFTER_INSTALL}
echo "ln -s /opt/usi_engine/bin/gikou-v1.0.1 /opt/usi_engine/bin/engine" >>${AFTER_INSTALL}
chmod 755 ${AFTER_INSTALL}

cd ${DIST_ROOT}

fpm --url "https://github.com/gikou-official/Gikou"  \
	--deb-user usi_engine --deb-group usi_engine \
	--after-install ${AFTER_INSTALL} \
	-s dir -t deb -C ${DIST_ROOT} -v 1.0.1 -n gikou-engine-${MAKE_OPT} -p gikou-engine-1.0.1-${MAKE_OPT}.deb .

mv gikou-engine-1.0.1-${MAKE_OPT}.deb ../

