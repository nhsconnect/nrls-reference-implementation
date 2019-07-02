@ECHO OFF

REM Build NRLS-API Image
docker build -t nrlsapi . --no-cache
