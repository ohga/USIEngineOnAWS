#!/bin/sh

MAKE_OPT="nosse"
if [ "x$1" != "x" ]; then
  MAKE_OPT=$1
fi
CORE_CNT=`grep '^processor\W' /proc/cpuinfo | tail -1  | awk -F':' '{print ($2 + 1);}'`

echo "engine install shinyane-sdt4."

BUILD_ROOT=/opt/build/src
DIST_ROOT=/opt/build/dist

rm -Rf ${DIST_ROOT}
mkdir -p ${BUILD_ROOT} ${DIST_ROOT}/opt/usi_engine/bin ${DIST_ROOT}/opt/usi_engine/share/script

cd ${BUILD_ROOT} 

rm -Rf shinyane_sdt4
git clone https://github.com/yaneurao/YaneuraOu shinyane_sdt4
cd shinyane_sdt4 && git checkout 5f8e5f3 && cd ..

cd shinyane_sdt4/source && \
  sed -i -e "s/YANEURAOU_EDITION = YANEURAOU_2016_MID_ENGINE/#YANEURAOU_EDITION = YANEURAOU_2016_MID_ENGINE/g" Makefile && \
  sed -i -e "s/#YANEURAOU_EDITION = YANEURAOU_2016_LATE_ENGINE/YANEURAOU_EDITION = YANEURAOU_2016_LATE_ENGINE/g" Makefile && \
  sed -i -e "s#Option(\"eval\")#Option(\"/opt/usi_engine/share/eval_dir\")#g" usi.cpp && \
  sed -i -e "s#read_book(\"book/#read_book(\"/opt/usi_engine/share/eval_dir/#g" *.cpp */*.cpp */*/*.cpp && \
  sed -i -e "s#book_list\[1\]#book_list\[4\]#g" *.cpp */*.cpp */*/*.cpp && \
  make -j ${CORE_CNT} ${MAKE_OPT} && \
  mv YaneuraOu-by-gcc shinyane && \
  install -s shinyane ${DIST_ROOT}/opt/usi_engine/bin
if [ $? != 0 ]; then
  echo "build error."
  exit 1
fi

AFTER_INSTALL=${DIST_ROOT}/opt/usi_engine/share/script/after_install_shinyane-sdt4.sh
echo "#!/bin/sh" > ${AFTER_INSTALL}
echo "unlink /opt/usi_engine/bin/engine" >> ${AFTER_INSTALL}
echo "ln -s /opt/usi_engine/bin/shinyane /opt/usi_engine/bin/engine" >>${AFTER_INSTALL}
chmod 755 ${AFTER_INSTALL}

cd ${DIST_ROOT}

fpm --url "https://github.com/yaneurao/YaneuraOu"  \
	--deb-user usi_engine --deb-group usi_engine \
	--after-install ${AFTER_INSTALL} \
	-s dir -t deb -C ${DIST_ROOT} -v 1.0 -n shinyane-sdt4-engine-${MAKE_OPT} -p shinyane-sdt4-engine-1.0-${MAKE_OPT}.deb .

mv shinyane-sdt4-engine-1.0-${MAKE_OPT}.deb  ../

