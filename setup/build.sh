#!/bin/bash

cd /nrlsrefimp/data
docker build -t nrlsmongodb . --no-cache

cd /nrlsrefimp/Demonstrator
docker build -t nrlsdemo . --no-cache

cd /nrlsrefimp/NRLS-API
docker build -t nrlsapi . --no-cache