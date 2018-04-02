@ECHO OFF

REM Get Variables
call config.cmd

REM Wait for MongoDB to boot
:statusretry

echo "=> Data: Waiting for confirmation of MongoDB service startup..."

timeout /t 5 /nobreak > NUL

for /F "tokens=3 delims=: " %%H in ('sc query %MONGO_SVC_NAME% ^| findstr "        STATE"') do (
    if "%%H" EQU "RUNNING" goto statusrunning

    REM TODO - start service if it's off
    REM if "%%H" EQU "STOPPED" goto :statusstopped
)
goto statusretry

:statusrunning

echo "=> Data: MongoDB service running."

REM START Data Import

echo "=> Dropping Personnel, ActorOrganisation, GenericSystem, Benefits from %MONGODB_APPLICATION_DATABASE%"

mongo admin --eval "db=db.getSiblingDB('%MONGODB_APPLICATION_DATABASE%');db.auth('%MONGODB_APPLICATION_USER%','%MONGODB_APPLICATION_PASS%');db.Personnel.drop();db.ActorOrganisation.drop();db.GenericSystem.drop();db.Benefits.drop();quit()"

echo "=> Dropping Patient, Organization, DocumentReference from %MONGODB_NRLS_DATABASE%"

mongo admin --eval "db=db.getSiblingDB('%MONGODB_NRLS_DATABASE%');db.auth('%MONGODB_NRLS_USER%','%MONGODB_NRLS_PASS%');db.Patient.drop();db.Organization.drop();db.DocumentReference.drop();quit()"

echo "=> Importing Default Data from %~dp0..\defaultdata\"


mongoimport -c ActorOrganisation -d %MONGODB_APPLICATION_DATABASE% -u %MONGODB_APPLICATION_USER% -p %MONGODB_APPLICATION_PASS% --mode insert --file %~dp0..\defaultdata\ActorOrganisation.json
echo "=> ActorOrganisation IMPORTED"

mongoimport -c GenericSystem -d %MONGODB_APPLICATION_DATABASE% -u %MONGODB_APPLICATION_USER% -p %MONGODB_APPLICATION_PASS% --mode insert --file %~dp0..\defaultdata\GenericSystem.json
echo "=> GenericSystem IMPORTED"

mongoimport -c Personnel -d %MONGODB_APPLICATION_DATABASE% -u %MONGODB_APPLICATION_USER% -p %MONGODB_APPLICATION_PASS% --mode insert --file %~dp0..\defaultdata\Personnel.json
echo "=> Personnel IMPORTED" 

mongoimport -c Benefits -d %MONGODB_APPLICATION_DATABASE% -u %MONGODB_APPLICATION_USER% -p %MONGODB_APPLICATION_PASS% --mode insert --file %~dp0..\defaultdata\Benefits.json
echo "=> Benefits IMPORTED" 

mongoimport -c Patient -d %MONGODB_NRLS_DATABASE% -u %MONGODB_NRLS_USER% -p %MONGODB_NRLS_PASS% --mode insert --file %~dp0..\defaultdata\Patient.json
echo "=> Patient IMPORTED" 

mongoimport -c Organization -d %MONGODB_NRLS_DATABASE% -u %MONGODB_NRLS_USER% -p %MONGODB_NRLS_PASS% --mode insert --file %~dp0..\defaultdata\Organization.json
echo "=> Organization IMPORTED" 

mongoimport -c DocumentReference -d %MONGODB_NRLS_DATABASE% -u %MONGODB_NRLS_USER% -p %MONGODB_NRLS_PASS% --mode insert --file %~dp0..\defaultdata\DocumentReference.json
echo "=> DocumentReference IMPORTED" 

REM END Data Import