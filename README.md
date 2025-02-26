This is an ASP.NET Core Web API to perform CRUD operations that allows the management of foreign exchange rates. The endpoint retrieves the rates for a specific currency pair checks for the data in the system's database. If the data is not available, it will fetch from a third-party and save it on the database. Everytime that a new rate is added, an event is raised to a queue.

This project was designed considering a domain-centric architecture.

# ⚙️ Setup

This project supports storing the data in-memory, but also in a real SQL Server database.

Using the "Database:UseInMemory" flag in the appsettings.json file (located in the WebAPI project), you can select the desired mode of operation. If set to false, the application will attempt to connect to the database engine, requiring you to specify the "Database:ConnectionString".

![](https://files.readme.io/45e945f12677841d73266d2c5661138137a554a24fc4d591a38495ca75d928be-image.png)

The same feature flag logic applies to event sourcing. If you don't want to use event sourcing, simply set "EventSourcing:Use" to false. In this case, the service will inject a "dummy" event sourcing implementation using dependency injection (DI). If you do want to use event sourcing, set the flag to true and also specify "EventSourcing:QueueName" and "EventSourcing:AzureServiceBus:ConnectionString".

![](https://files.readme.io/34d8d10ebfbbe451efe884abfb9edabe5b840a19c0238e22463fc4016d76f3b9-image.png)

The third-party API key is also configurable in appsettings.json and must be set before use. You can obtain one here: [https://www.alphavantage.co/support/#api-key](https://www.alphavantage.co/support/#api-key).

This project uses Entity Framework Core as object-relation mapper. With migrations we can easily setup the needed tables and indexes. To run the migrations use the following command:

dotnet ef database update --project ForeignExchangeRates.Infrastructure --startup-project ForeignExchangeRates.WebAPI

After running this command, your database should have a (empty) table looking something like this:

![](https://files.readme.io/dc09be9ecf4f0236e0ce163eee4fe7d5ba3713333e04a6159ec22e49b5f1ad85-image.png)

At this point you should be able to set the WebAPI project as startup project and launch it. This API exposes 44 different endpoints:

![](https://files.readme.io/a68205c2d899c4e68db496c72de6561204dc12dd4398993f9c56e64f2a622209-image.png)

At the root directory there is a paste called "postman" that contains an importable collection that can be used to test each endpoint.

![](https://files.readme.io/b7d2597504d7ddf101955dabddffa1e2cf594827cf99dfd20b1a03cde473b6ea-image.png)
