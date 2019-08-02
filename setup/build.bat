@ECHO OFF

REM Build Mongo Data Image, the NRL API, then NRL Demo APP
cd ../data && build.bat && cd ../NRLS-API && build.bat && cd ../Demonstrator && build.bat

