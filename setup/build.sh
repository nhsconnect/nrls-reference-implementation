#!/bin/bash

cd /home/ec2-user/nrls-reference-implementation-onelondon-27062019/data
docker build -t nrlsmongodb . --no-cache

cd /home/ec2-user/nrls-reference-implementation-onelondon-27062019/Demonstrator
docker build -t nrlsdemo . --no-cache

cd /home/ec2-user/nrls-reference-implementation-onelondon-27062019/NRLS-API
docker build -t nrlsapi . --no-cache