@ECHO OFF

REM Build Mongo Data Image
docker build -t nrlsmongodb . --no-cache
