#!/bin/bash
 
# Globals
MONGODB_DATA_PATH=${MONGODB_DATA_PATH:-"/data/db"}
MONGODB_LOG_PATH=${MONGODB_LOG_PATH:-"/data/log"}
MONGODB_DEFAULT_DATA_PATH=${MONGODB_DEFAULT_DATA_PATH:-"/data/defaultdata/"}
MONGODB_PASSWORD_LOCK_FILE=${MONGODB_PASSWORD_LOCK_FILE:-"/data/db/.mongodb_password_set"}

# Admin User
MONGODB_ADMIN_USER=${MONGODB_ADMIN_USER:-"admin"}
MONGODB_ADMIN_PASS=${MONGODB_ADMIN_PASS:-"adpass10!"}
 
# Application Database User
MONGODB_APPLICATION_DATABASE=${MONGODB_APPLICATION_DATABASE:-"Demonstrator"}
MONGODB_APPLICATION_USER=${MONGODB_APPLICATION_USER:-"riAdmin"}
MONGODB_APPLICATION_PASS=${MONGODB_APPLICATION_PASS:-"riapass10!"}

# NRLS Database User
MONGODB_NRLS_DATABASE=${MONGODB_NRLS_DATABASE:-"Nrls"}
MONGODB_NRLS_USER=${MONGODB_NRLS_USER:-"nsAdmin"}
MONGODB_NRLS_PASS=${MONGODB_NRLS_PASS:-"nsapass10!"}
 
 # Wait for MongoDB to boot
RET=1
while [[ RET -ne 0 ]]; do
    echo "=> Users: Waiting for confirmation of MongoDB service startup..."
    sleep 5
    mongo admin --eval "help" >/dev/null 2>&1
    RET=$?
done

echo "=> Users: MongoDB service running."

# START User Setup
if [ ! -f $MONGODB_PASSWORD_LOCK_FILE ]; then

echo "=> Starting user setup"


# Create the admin user
echo "=> Creating admin user with a password in MongoDB"
mongo admin --eval "db.createUser({user: '$MONGODB_ADMIN_USER', pwd: '$MONGODB_ADMIN_PASS', roles:[{role:'root',db:'admin'}]});"
 
sleep 3
 
# Create the demonstrator  user

    echo "=> Creating an ${MONGODB_APPLICATION_DATABASE} user with a password in MongoDB"
    mongo admin -u $MONGODB_ADMIN_USER -p $MONGODB_ADMIN_PASS << EOF
use $MONGODB_APPLICATION_DATABASE
db.createUser({user: '$MONGODB_APPLICATION_USER', pwd: '$MONGODB_APPLICATION_PASS', roles:[{role:'readWrite', db:'$MONGODB_APPLICATION_DATABASE'}]})
EOF


# Now create the NRLS DB User
    echo "=> Creating an ${MONGODB_NRLS_DATABASE} user with a password in MongoDB"
    mongo admin -u $MONGODB_ADMIN_USER -p $MONGODB_ADMIN_PASS << EOF
use $MONGODB_NRLS_DATABASE
db.createUser({user: '$MONGODB_NRLS_USER', pwd: '$MONGODB_NRLS_PASS', roles:[{role:'readWrite', db:'$MONGODB_NRLS_DATABASE'}]})
EOF
 
sleep 1
 
# If everything went well, add a file as a flag so we know in the future to not re-create the
# users if we're recreating the container (provided we're using some persistent storage)
touch $MONGODB_PASSWORD_LOCK_FILE
echo "=> User data done!"

# mongod --shutdown --dbpath ${MONGODB_DATA_PATH}
mongod --auth --bind_ip 0.0.0.0 --dbpath ${MONGODB_DATA_PATH} --logpath ${MONGODB_LOG_PATH}
echo "=> User: MongoDb restarting in Auth mode"

fi
# END User Setup

echo "=> User setup complete"