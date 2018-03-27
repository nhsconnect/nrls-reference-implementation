#!/bin/bash
 
# Globals
MONGODB_DATA_PATH=${MONGODB_DATA_PATH:-"/data/"}
MONGODB_DEFAULT_DATA_PATH=${MONGODB_DEFAULT_DATA_PATH:-"/data/defaultdata/"}

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
    echo "=> Data: Waiting for confirmation of MongoDB service startup..."
    sleep 5
    mongo admin --eval "help" >/dev/null 2>&1
    RET=$?
done

# START Data Import
echo "=> Dropping Personnel, ActorOrganisation, GenericSystem"

mongo << EOF
use $MONGODB_APPLICATION_DATABASE
db.auth('$MONGODB_APPLICATION_USER','$MONGODB_APPLICATION_PASS')
db.Personnel.drop()
db.ActorOrganisation.drop()
db.GenericSystem.drop()
EOF

echo "=> Dropping Patient, Organization, DocumentReference"

mongo << EOF
use $MONGODB_NRLS_DATABASE
db.auth('$MONGODB_NRLS_USER','$MONGODB_NRLS_PASS')
db.Patient.drop()
db.Organization.drop()
db.DocumentReference.drop()
EOF

mongoimport -c ActorOrganisation -d $MONGODB_APPLICATION_DATABASE -u $MONGODB_APPLICATION_USER -p $MONGODB_APPLICATION_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}ActorOrganisation.json
echo "=> ActorOrganisation IMPORTED"
mongoimport -c GenericSystem -d $MONGODB_APPLICATION_DATABASE -u $MONGODB_APPLICATION_USER -p $MONGODB_APPLICATION_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}GenericSystem.json
echo "=> GenericSystem IMPORTED"
mongoimport -c Personnel -d $MONGODB_APPLICATION_DATABASE -u $MONGODB_APPLICATION_USER -p $MONGODB_APPLICATION_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}Personnel.json
echo "=> Personnel IMPORTED" 

mongoimport -c Organization -d $MONGODB_NRLS_DATABASE -u $MONGODB_NRLS_USER -p $MONGODB_NRLS_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}Organization.json
echo "=> Organization IMPORTED"
mongoimport -c Patient -d $MONGODB_NRLS_DATABASE -u $MONGODB_NRLS_USER -p $MONGODB_NRLS_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}Patient.json
echo "=> Patient IMPORTED"
mongoimport -c DocumentReference -d $MONGODB_NRLS_DATABASE -u $MONGODB_NRLS_USER -p $MONGODB_NRLS_PASS --mode insert --file ${MONGODB_DEFAULT_DATA_PATH}DocumentReference.json
echo "=> DocumentReference IMPORTED" 
# END Data Import