#!/usr/bin/bash

echo "Starting services ..."

sudo docker compose -f infra.yml up -d
sleep 5

sudo docker compose -f app.yml up -d
sleep 5

echo "Services were started: visit http://localhost:5432"
