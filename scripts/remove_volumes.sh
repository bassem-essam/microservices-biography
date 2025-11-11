#!/usr/bin/bash

sudo docker compose -f infra.yml down
sudo docker compose -f app.yml down
sudo docker volume ls -q | xargs sudo docker volume rm
