#!/bin/sh

MAKE_OPT="nosse"
if [ "x$1" != "x" ]; then
  MAKE_OPT=$1
fi
CORE_CNT=`grep '^processor\W' /proc/cpuinfo | tail -1  | awk -F':' '{print ($2 + 1);}'`

echo "engine install silent_majority v1.2."

BUILD_ROOT=/opt/build/src
DIST_ROOT=/opt/build/dist

rm -Rf ${DIST_ROOT}
mkdir -p ${BUILD_ROOT} ${DIST_ROOT}/opt/usi_engine/bin ${DIST_ROOT}/opt/usi_engine/share/script

cd ${BUILD_ROOT} 

rm -Rf silent_majority-v1.2
git clone https://github.com/Jangja/silent_majority.git -b 1.2 silent_majority-v1.2

cd silent_majority-v1.2/src && \
  sed -i -e "s#20160307#/opt/usi_engine/share/eval_dir#g" usi.cpp && \
  sed -i -e "s#book/20150503#/opt/usi_engine/share/eval_dir#g" usi.cpp && \
  make -j ${CORE_CNT} ${MAKE_OPT} && \
  mv apery silent_majority-v1.2 && \
  install -s silent_majority-v1.2 ${DIST_ROOT}/opt/usi_engine/bin
if [ $? != 0 ]; then
  echo "build error."
  exit 1
fi

AFTER_INSTALL=${DIST_ROOT}/opt/usi_engine/share/script/after_install_silent_majority-v1.2.sh
echo "#!/bin/sh" > ${AFTER_INSTALL}
echo "unlink /opt/usi_engine/bin/engine" >> ${AFTER_INSTALL}
echo "ln -s /opt/usi_engine/bin/silent_majority-v1.2 /opt/usi_engine/bin/engine" >>${AFTER_INSTALL}
chmod 755 ${AFTER_INSTALL}

cd ${DIST_ROOT}

fpm --url "https://github.com/Jangja/silent_majority/tree/1.2"  \
	--deb-user usi_engine --deb-group usi_engine \
	--after-install ${AFTER_INSTALL} \
	-s dir -t deb -C ${DIST_ROOT} -v 1.2 -n silent_majority-engine-${MAKE_OPT} -p silent_majority-engine-1.2-${MAKE_OPT}.deb .

mv silent_majority-engine-1.2-${MAKE_OPT}.deb ../

