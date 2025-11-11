#!/usr/bin/bash

echo "Stopping services ..."

sudo docker compose -f infra.yml down
sudo docker compose -f app.yml down

echo "Services were stopped"
