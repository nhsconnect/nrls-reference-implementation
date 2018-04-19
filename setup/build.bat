@ECHO OFF

REM Build Mongo Data Image
cd ../data & docker build -t nrlsmongodb . --no-cache

REM Build Demonstrator Image
cd ../Demonstrator & docker build -t nrlsdemo . --no-cache

REM Build NRLS-API Image
cd ../NRLS-API & docker build -t nrlsapi . --no-cache
