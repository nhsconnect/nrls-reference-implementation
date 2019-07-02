@ECHO OFF

REM Build Mongo Data Image
cd ../data && build.bat

REM Build Demonstrator Image
cd ../Demonstrator && build.bat

REM Build NRLS-API Image
cd ../NRLS-API && build.bat
