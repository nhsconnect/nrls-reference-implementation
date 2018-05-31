@ECHO OFF

REM Get Variables
call config.cmd

SET cmd=mongod --dbpath %MONGO_DATA_PATH% --logpath %MONGO_LOG_PATH% --install --serviceName %MONGO_SVC_NAME% --serviceDisplayName %MONGO_SVC_NAME%
SET cmd_message=Starting MongoDB:Open

IF EXIST %PASSWORD_LOCK_FILE% (
SET cmd=%cmd% --auth
SET cmd_message=Starting MongoDB:Secure
)

echo "=> %cmd_message%"

%cmd% && net start %MONGO_SVC_NAME% & set_mongodb_users.bat && set_mongodb_data.bat
