# Self Contained System - Proof of Concept

* Tactical Domain Driven Design Patterns (Aggregate Root, Entities, Value Objects, Domain Events, Repository)
* Unit Of Work Pattern & Optimistic Concurrency
* Messaging via NServiceBus
* Outbox Pattern (`NServiceBusOutbox` & `HangfireOutbox`)
* Integration Tests

*Use Cases*

## TODO

* Hangfire Concurrency bei Outbox und mehreren Server-Instanzen vermeiden
* SendLocal prüfen
* Sagas prüfen
* Dapper sql command + transaction

## Tests ausführen

1. `docker-compose up`
2. `Web` Projekt kurz starten (damit DB migriert wird)
3. Tests mit Visual Studio ausführen


### Outbox Problematik

NServiceBus bietet mit ihrer Outbox bereits eine Lösung für solche Konsiszentprobleme an. Das Problem ist jedoch, 
dass diese ausschliesslich im Zusammenhand mit einer Inbox funktioniert. Mit aktivieren der Outbox ist also nur die 
Konsistenz zwischen DB und NSB im Message Handler Kontext (Event -> DB -> Event) gelöst.

Hier einige links dazu:
* https://docs.particular.net/samples/router/update-and-publish/
* https://discuss.particular.net/t/outbox-in-an-asp-net-core-scenario/966/6

> Im Moment gibt es mehrere Lösungsansätze:
> * [`NServiceBus.Router`](https://docs.particular.net/nservicebus/router/) ist ein Community Projekt und hat keinen offiziellen NSB support.
> * [`NServiceBus.Connector.SqlServer`](https://www.nuget.org/packages/NServiceBus.Connector.SqlServer) basiert auf `NServiceBus.Router` und vereinfacht dessen Handhabung
> * [`https://github.com/peto268/NServiceBus.WebOutbox`](https://github.com/peto268/NServiceBus.WebOutbox) 
> * [`NServiceBus.Transport.Bridge`](https://docs.particular.net/nservicebus/bridge/) ist eine offiziell supportete NSB Komponente, momentan existiert jedoch nur eine preview Version.
> * Outbox via Hangfire selbst implementieren
