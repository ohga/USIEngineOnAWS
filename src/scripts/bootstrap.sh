#!/bin/sh

INSTANCE_NAME=unknown
ENGINE=ukamuse-sdt4-engine-1.0
EVAL=ukamuse-sdt4-eval-1.0

# unused..
if [ "x$1" != "x" ]; then
  INSTANCE_NAME=$1
fi

if [ "x$2" != "x" ]; then
  ENGINE=$2
fi
if [ ! -e engine/${ENGINE}.sh ]; then
  echo "ERROR: illegal engine name"
  exit 1
fi
echo ${ENGINE}

if [ "x$3" != "x" ]; then
  EVAL=$3
fi
if [ ! -e eval/${EVAL}.sh ]; then
  echo "ERROR: illegal eval name"
  exit 1
fi

echo 'ClientAliveInterval 15' >> /etc/ssh/sshd_config
echo 'ClientAliveCountMax 1200' >> /etc/ssh/sshd_config
systemctl restart sshd
if [ $? != 0 ]; then
  echo "ERROR: restart sshd"
  exit 1
fi

###### (trancate)create usi_engine environment
rm -Rf /opt/usi_engine /var/mail/usi_engine
userdel usi_engine > /dev/null 2>&1

mkdir -p /opt/usi_engine/src /opt/usi_engine/bin /opt/usi_engine/share /opt/usi_engine/.ssh

ln -s /tmp /opt/usi_engine/share/eval_dir
ln -s /bin/false /opt/usi_engine/bin/engine

echo -n 'no-pty ' > /opt/usi_engine/.ssh/authorized_keys
cat /home/ubuntu/.ssh/authorized_keys >> /opt/usi_engine/.ssh/authorized_keys
chmod 700 /opt/usi_engine/.ssh
chmod 600 /opt/usi_engine/.ssh/authorized_keys 

useradd -d /opt/usi_engine -M -s /opt/usi_engine/bin/engine usi_engine
touch /opt/usi_engine/.hushlogin

apt-get update && sleep 5
apt-get install -y libgomp1 unzip p7zip-full

#############################################
## TODO: ...

MAKE_OPT="nosse"
STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse2`
if [ "x${STR}" != "x" ]; then
        MAKE_OPT="sse2"
fi
STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse4_1`
if [ "x${STR}" != "x" ]; then
        MAKE_OPT="sse41"
fi


# apery genealogy
if [ "${ENGINE}" = "ukamuse-sdt4-engine-1.0" ]; then
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse4_2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="sse"
  fi
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep bmi2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="bmi2"
  fi
fi
if [ "${ENGINE}" = "silent_majority-engine-1.2" ]; then
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse4_2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="sse"
  fi
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep bmi2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="bmi2"
  fi
fi

# yaneura-o genealogy
if [ "${ENGINE}" = "shinyane-sdt4-engine-1.0" ]; then
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse4_2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="sse42"
  fi
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep avx2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="avx2"
  fi
fi
if [ "${ENGINE}" = "tanuki-sdt4-engine-1.0" ]; then
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep sse4_2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="sse42"
  fi
  STR=`cat /proc/cpuinfo | grep flags | head -1 | grep avx2`
  if [ "x${STR}" != "x" ]; then
          MAKE_OPT="avx2"
  fi
fi

# gikou.
if [ "${ENGINE}" = "gikou-engine-1.0.1" ]; then
  MAKE_OPT="release"
fi
if [ "${ENGINE}" = "gikou-engine-BookMaxPly_70_0_120-1.0.1" ]; then
  MAKE_OPT="release"
fi
    
#############################################

if [ ! -f /tmp/${ENGINE}-${MAKE_OPT}.deb ]; then
  apt-get install -y ruby ruby-ffi gem make g++ && sleep 5
  if [ $? != 0 ]; then
    echo "ERROR: apt-get install"
    exit 1
  fi
  gem install fpm 
  if [ $? != 0 ]; then
    echo "ERROR: gem install fpm"
    exit 1
  fi

  /bin/sh engine/${ENGINE}.sh ${MAKE_OPT}
  if [ $? != 0 ]; then
    echo "ERROR: engine script"
    exit 1
  fi
  
  mv /opt/build/${ENGINE}-${MAKE_OPT}.deb /tmp/
fi

dpkg -i /tmp/${ENGINE}-${MAKE_OPT}.deb
if [ $? != 0 ]; then
  echo "ERROR: dpkg install"
  exit 1
fi

/bin/sh eval/${EVAL}.sh
if [ $? != 0 ]; then
  echo "ERROR: eval script"
  exit 1
fi
chown -R usi_engine.usi_engine /opt/usi_engine

# USIClient.exe - https://sites.google.com/site/shogixyz/home/shogiutil
apt-get install -y ucspi-tcp 
tcpserver -HR -l -c1 -u 1000 0 53556 nice -n 5 /opt/usi_engine/bin/engine &

echo "install script OK"

