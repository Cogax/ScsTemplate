# PoC für NSB Outbox

## Aufgabenstellung

Dieser Proof of Concept beinhaltet drei Projekte. `Web1` und `Web2` sind Web API's und `Worker` in ein "Background Worker" 
welcher allein für asynchrone Operationen gedacht ist (ist jedoch auch ein auch ein Wep API Projekt).

* `Web1` - *Publish & Subscribe*
  * Use Cases
    * Publiziert NSB Messages (bspw. bei erfolgreichem API call)
    * Handelt NSB Messages (bspw. damit er SignalR updates an den Client senden kann)
    * Modifiziert die Datenbank
  * Requirements
    * **Konsistenz zwischen DB und NSB im HTTP Request Kontext (API -> DB -> Event)**
    * Konsistenz zwischen DB und NSB im Message Handler Kontext (Event -> DB -> Event)
  * Out of Scope
    * Konsistenz zwischen DB und SignalR
    * Konsistenz zwischen NSB und SignalR 
* `Web2` - *Nur Publish (SendOnly)*
  * Use Cases
    * Publiziert NSB Messages (bspw. bei erfolgreichem API call)
    * Modifiziert die Datenbank
  * Requirements
    * Konsistenz zwischen DB und NSB im HTTP Request Kontext (API -> DB -> Event)
  * Out of Scope
    * Konsistenz zwischen DB und SignalR
    * Konsistenz zwischen NSB und SignalR 
* `Worker` - *Publish & Subscribe*
  * Use Cases
    * Publiziert NSB Messages
    * Handelt NSB Messages
    * Modifiziert die Datenbank
  * Requirements
    * Konsistenz zwischen DB und NSB im Message Handler Kontext (Event -> DB -> Event)


### Problem

NServiceBus bietet mit ihrer Outbox bereits eine Lösung für solche Konsiszentprobleme an. Das Problem ist jedoch, 
dass diese ausschliesslich im Zusammenhand mit einer Inbox funktioniert. Mit aktivieren der Outbox ist also nur die 
Konsistenz zwischen DB und NSB im Message Handler Kontext (Event -> DB -> Event) gelöst.

Hier einige links dazu:
* https://docs.particular.net/samples/router/update-and-publish/
* https://discuss.particular.net/t/outbox-in-an-asp-net-core-scenario/966/6

### Lösungsansatz

Der Lösungsansatz besteht darin, dass im `Web1` und `Web2` sowohl die NSB- Persistence sowie auch der Transport via MSSQL definiert
wird. Dabei werden die NSB Messages nicht auf den Bus, sondern in die Datenbank "publiziert". Dadurch kann die selbe
Transaktion verwendet werden, welche auch für die Business Operationen verwendet wird.

Via NSB können dann mehrere Transport Implementationen gekoppelt werden. Dabei wird der MSSQL Transport des `Web`
und der RabbitMQ Transport des `Worker` aneinandergekoppelt. Der `Worker` kann dann Messages direkt von der Datenbank
statt RabbitMQ entgegennehmen und verarbeiten.

> Im Moment gibt es mehrere Lösungsansätze:
> * [`NServiceBus.Router`](https://docs.particular.net/nservicebus/router/) ist ein Community Projekt und hat keinen offiziellen NSB support.
> * [`NServiceBus.Connector.SqlServer`](https://www.nuget.org/packages/NServiceBus.Connector.SqlServer) basiert auf `NServiceBus.Router` und vereinfacht dessen Handhabung
> * [`https://github.com/peto268/NServiceBus.WebOutbox`](https://github.com/peto268/NServiceBus.WebOutbox) 
> * [`NServiceBus.Transport.Bridge`](https://docs.particular.net/nservicebus/bridge/) ist eine offiziell supportete NSB Komponente, momentan existiert jedoch nur eine preview Version.

Der `Worker` selbst hat das Outbox Feature aktiviert, wordurch beim behandeln von Messages die Konsistenz zwischen DB 
und NSB gewährleistet werden kann. Auch `Web` benötigt ein aktives Outbox Feature, damit er ebenfalls die Konsistenz
beim verarbeiten von Messages mit anschliesendem Publish gewährleisten kann.


## Implementation

### Starten

* `docker-compose up`
* `Poc.Nsb.Outbox.Web` und `Poc.Nsb.Outbox.Worker` im Visual Studio laufen lassen

### Migrationen

1. `git commit` damit der revert einfacher geht
1. Migration hinzufügen: `Add-Migration <migration-name> -Context WriteModelDbContext -OutputDir Adapters/Persistence/Migrations -Project Poc.Nsb.Outbox.Infrastructure -StartupProject Poc.Nsb.Outbox.Web`
1. Applikation starten, damit DB migriert wird 
1. Read Model Context aus der DB generieren: `Scaffold-DbContext 'server=localhost;database=PocDb;user=sa;password=Top-Secret;' Microsoft.EntityFrameworkCore.SqlServer -DataAnnotations -Context ReadModelDbContext -ContextDir Adapters/Persistence/Contexts -OutputDir Adapters/Persistence/Generated -Project Poc.Nsb.Outbox.Infrastructure -StartupProject Poc.Nsb.Outbox.Web -NoPluralize -Force`


# TODO
* SendLocal prüfen
* Sagas prüfen
