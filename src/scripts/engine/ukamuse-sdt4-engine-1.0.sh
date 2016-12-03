#!/bin/sh

MAKE_OPT="nosse"
if [ "x$1" != "x" ]; then
  MAKE_OPT=$1
fi
CORE_CNT=`grep '^processor\W' /proc/cpuinfo | tail -1  | awk -F':' '{print ($2 + 1);}'`

echo "engine install ukamuse_sdt4."

BUILD_ROOT=/opt/build/src
DIST_ROOT=/opt/build/dist

rm -Rf ${DIST_ROOT}
mkdir -p ${BUILD_ROOT} ${DIST_ROOT}/opt/usi_engine/bin ${DIST_ROOT}/opt/usi_engine/share/script

cd ${BUILD_ROOT} 

rm -Rf apery_sdt4
git clone https://github.com/HiraokaTakuya/apery.git -b SDT4 apery_sdt4

cd apery_sdt4/src && \
  sed -i -e "s#20161007#/opt/usi_engine/share/eval_dir#g" evaluate.cpp && \
  sed -i -e "s#book/20150503#/opt/usi_engine/share/eval_dir#g" usi.cpp && \
  make -j ${CORE_CNT} ${MAKE_OPT} && \
  mv apery ukamuse && \
  install -s ukamuse ${DIST_ROOT}/opt/usi_engine/bin
if [ $? != 0 ]; then
  echo "build error."
  exit 1
fi

AFTER_INSTALL=${DIST_ROOT}/opt/usi_engine/share/script/after_install_ukamuse-sdt4.sh
echo "#!/bin/sh" > ${AFTER_INSTALL}
echo "unlink /opt/usi_engine/bin/engine" >> ${AFTER_INSTALL}
echo "ln -s /opt/usi_engine/bin/ukamuse /opt/usi_engine/bin/engine" >>${AFTER_INSTALL}
chmod 755 ${AFTER_INSTALL}

cd ${DIST_ROOT}

fpm --url "https://github.com/HiraokaTakuya/apery/tree/SDT4"  \
	--deb-user usi_engine --deb-group usi_engine \
	--after-install ${AFTER_INSTALL} \
	-s dir -t deb -C ${DIST_ROOT} -v 1.0 -n ukamuse-sdt4-engine-${MAKE_OPT} -p ukamuse-sdt4-engine-1.0-${MAKE_OPT}.deb .

mv ukamuse-sdt4-engine-1.0-${MAKE_OPT}.deb  ../

