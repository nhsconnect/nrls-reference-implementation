REM General
SET MONGO_SVC_NAME=MongoDB36
SET MONGO_DATA_PATH=C:\Data\Mongo\db\
SET MONGO_LOG_PATH=C:\Data\Mongo\log\mongod.log
SET PASSWORD_LOCK_FILE=%MONGO_DATA_PATH%.mongodb_password_set

REM Admin User
SET MONGODB_ADMIN_USER=admin
SET MONGODB_ADMIN_PASS=adpass10!
 
REM Application Database User
SET MONGODB_APPLICATION_DATABASE=Demonstrator
SET MONGODB_APPLICATION_USER=riAdmin
SET MONGODB_APPLICATION_PASS=riapass10!

REM NRLS Database User
SET MONGODB_NRLS_DATABASE=Nrls
SET MONGODB_NRLS_USER=nsAdmin
SET MONGODB_NRLS_PASS=nsapass10!