@ECHO OFF

REM Get Variables
call config.cmd
 
REM Wait for MongoDB to boot
:statusretry

echo "=> Users: Waiting for confirmation of MongoDB service startup..."

timeout /t 5 /nobreak > NUL

for /F "tokens=3 delims=: " %%H in ('sc query %MONGO_SVC_NAME% ^| findstr "        STATE"') do (
    if "%%H" EQU "RUNNING" goto statusrunning

    REM TODO - start service if it's off
    REM if "%%H" EQU "STOPPED" goto :statusstopped
)
goto statusretry

:statusrunning

echo "=> Users: MongoDB service running."

REM START User Setup

REM Skip if lock file exists
if exist %PASSWORD_LOCK_FILE% goto userscomplete

echo "=> Starting user setup"

REM Create the admin user
echo "=> Creating admin user with a password in MongoDB"

mongo admin --eval "db.createUser({user: '%MONGODB_ADMIN_USER%', pwd: '%MONGODB_ADMIN_PASS%', roles:[{role:'root',db:'admin'}]});quit()"

timeout /t 3 /nobreak > NUL

REM Create the Demonstrator user

echo "=> Creating a %MONGODB_APPLICATION_DATABASE% user with a password in MongoDB"

mongo admin --eval "db=db.getSiblingDB('%MONGODB_APPLICATION_DATABASE%');db.createUser({user: '%MONGODB_APPLICATION_USER%', pwd: '%MONGODB_APPLICATION_PASS%', roles:[{role:'readWrite', db:'%MONGODB_APPLICATION_DATABASE%'}]});quit()"

timeout /t 1 /nobreak > NUL

REM Create the NRLS user

echo "=> Creating a %MONGODB_NRLS_DATABASE% user with a password in MongoDB"

mongo admin --eval "db=db.getSiblingDB('%MONGODB_NRLS_DATABASE%');db.createUser({user: '%MONGODB_NRLS_USER%', pwd: '%MONGODB_NRLS_PASS%', roles:[{role:'readWrite', db:'%MONGODB_NRLS_DATABASE%'}]});quit()"


REM If everything went well, add a file as a flag so we know in the future to not re-create the
REM users if we're recreating the container (provided we're using some persistent storage)
echo "=> Done creating users!"

echo. 2>%PASSWORD_LOCK_FILE%

echo "=> Restarting Mongod in auth mode"

net stop %MONGO_SVC_NAME%
mongod --auth --dbpath %MONGO_DATA_PATH% --logpath %MONGO_LOG_PATH% --reinstall --serviceName %MONGO_SVC_NAME% --serviceDisplayName %MONGO_SVC_NAME%
net start %MONGO_SVC_NAME%

REM END User Setup
:userscomplete
echo "=> Users: End user creation file."
