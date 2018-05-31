#!/bin/bash
set -m
 
mongodb_cmd="mongod --bind_ip 0.0.0.0 --dbpath /data/db --logpath /data/log"
cmd="$mongodb_cmd"
cmd_message="=> Starting MongoDB:Open"

if [ -f /data/db/.mongodb_password_set ]; then
    cmd="$cmd --auth"
    cmd_message="=> Starting MongoDB:Secure"
fi
 
echo "${cmd_message}";

$cmd & /data/cmds/set_mongodb_users.sh ; /data/cmds/set_mongodb_data.sh
 
fg