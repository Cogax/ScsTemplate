# Proof of Concept für NSB Outbox mit Web & Worker Projekt

## Run
* `docker-compose up`

## Migrations
* New: `Add-Migration <migration-name> -Project Poc.Nsb.Outbox.Infrastructure -StartupProject Poc.Nsb.Outbox.Web`


## Design Entscheidungen
* ID's werden Clientseitig erstellt damit diese in den Events referenziert werden können.
