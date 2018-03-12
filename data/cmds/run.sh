#!/bin/bash
set -m
 
mongodb_cmd="mongod --bind_ip 0.0.0.0 --dbpath /data/db"
cmd="$mongodb_cmd"

if [ -f /data/db/.mongodb_password_set ]; then
    cmd="$cmd --auth"
fi
 
$cmd & /data/cmds/set_mongodb_users_and_Data.sh
 
fg