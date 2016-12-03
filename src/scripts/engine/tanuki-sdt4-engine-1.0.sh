#!/bin/sh

MAKE_OPT="nosse"
if [ "x$1" != "x" ]; then
  MAKE_OPT=$1
fi
CORE_CNT=`grep '^processor\W' /proc/cpuinfo | tail -1  | awk -F':' '{print ($2 + 1);}'`

echo "engine install tanuki-sdt4."

BUILD_ROOT=/opt/build/src
DIST_ROOT=/opt/build/dist

rm -Rf ${DIST_ROOT}
mkdir -p ${BUILD_ROOT} ${DIST_ROOT}/opt/usi_engine/bin ${DIST_ROOT}/opt/usi_engine/share/script

cd ${BUILD_ROOT} 

rm -Rf tanuki-sdt4
git clone https://github.com/nodchip/hakubishin- -b tanuki-sdt4-2016-10-09 tanuki-sdt4

cd tanuki-sdt4/source && \
  sed -i -e "s#Option(\"eval\")#Option(\"/opt/usi_engine/share/eval_dir\")#g" usi.cpp && \
  sed -i -e "s#read_book(\"book/#read_book(\"/opt/usi_engine/share/eval_dir/#g" *.cpp */*.cpp */*/*.cpp && \
  sed -i -e "s#book_list\[1\]#book_list\[4\]#g" *.cpp */*.cpp */*/*.cpp && \
  sed -i -s -e '1s/^\xef\xbb\xbf//' position.cpp  && \
  sed -i -e '1s/^/#include <stddef.h>\n/' position.cpp  && \
  make -j ${CORE_CNT} ${MAKE_OPT} && \
  install -s tanuki-sdt4 ${DIST_ROOT}/opt/usi_engine/bin
if [ $? != 0 ]; then
  echo "build error."
  exit 1
fi

AFTER_INSTALL=${DIST_ROOT}/opt/usi_engine/share/script/after_install_tanuki-sdt4.sh
echo "#!/bin/sh" > ${AFTER_INSTALL}
echo "unlink /opt/usi_engine/bin/engine" >> ${AFTER_INSTALL}
echo "ln -s /opt/usi_engine/bin/tanuki-sdt4 /opt/usi_engine/bin/engine" >>${AFTER_INSTALL}
chmod 755 ${AFTER_INSTALL}

cd ${DIST_ROOT}

fpm --url "https://github.com/yaneurao/YaneuraOu"  \
	--deb-user usi_engine --deb-group usi_engine \
	--after-install ${AFTER_INSTALL} \
	-s dir -t deb -C ${DIST_ROOT} -v 1.0 -n tanuki-sdt4-engine-${MAKE_OPT} -p tanuki-sdt4-engine-1.0-${MAKE_OPT}.deb .

mv tanuki-sdt4-engine-1.0-${MAKE_OPT}.deb  ../

