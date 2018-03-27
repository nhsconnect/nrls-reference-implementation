#!/bin/bash

cd /nrlsrefimp/data
docker build -t nrlsmongodb .

cd /nrlsrefimp/Demonstrator
docker build -t nrlsdemo .

cd /nrlsrefimp/NRLS-API
docker build -t nrlsapi .