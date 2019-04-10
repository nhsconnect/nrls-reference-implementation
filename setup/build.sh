#!/bin/bash

cd ../data
docker build -t nrlsmongodb . --no-cache

cd ../Demonstrator
docker build -t nrlsdemo . --no-cache

cd ../NRLS-API
docker build -t nrlsapi . --no-cache
