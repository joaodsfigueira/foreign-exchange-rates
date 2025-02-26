This is an ASP.NET Core Web API for performing CRUD operations to manage foreign exchange rates. The endpoint retrieves rates for a specific currency pair and first checks the system's database for the data. If the data is unavailable, it fetches it from a third party and saves it to the database. Whenever a new rate is added, an event is raised and sent to a queue.

This project was designed with a domain-centric architecture in mind.

# ⚙️ Setup

This project supports storing the data in-memory, but also in a real SQL Server database.

Using the "Database:UseInMemory" flag in the appsettings.json file (located in the WebAPI project), you can select the desired mode of operation. If set to false, the application will attempt to connect to the database engine, requiring you to specify the "Database:ConnectionString".

![](https://files.readme.io/4964ba182cbeed0fbb97f7cf2279796aa7cbd4ec0ea1518d83e2d722aa92f8c3-image.png)

The same feature flag logic applies to event sourcing. If you don't want to use event sourcing, simply set "EventSourcing:Use" to false. In this case, the service will inject a "dummy" event sourcing implementation using dependency injection (DI). If you do want to use event sourcing, set the flag to true and also specify "EventSourcing:QueueName" and "EventSourcing:AzureServiceBus:ConnectionString".

![](https://files.readme.io/d5c346506c6513f7c8f82f4c04c4c4df3f9b172c4bf9abaca1c9a2a340eab4e4-image.png)

The third-party API key is also configurable in appsettings.json and must be set before use. You can obtain one here: [https://www.alphavantage.co/support/#api-key](https://www.alphavantage.co/support/#api-key).

This project uses Entity Framework Core as object-relation mapper. With migrations we can easily setup the needed tables and indexes. To run the migrations use the following command:

dotnet ef database update --project ForeignExchangeRates.Infrastructure --startup-project ForeignExchangeRates.WebAPI

After running this command, your database should have a (empty) table looking something like this:

![](https://files.readme.io/dc09be9ecf4f0236e0ce163eee4fe7d5ba3713333e04a6159ec22e49b5f1ad85-image.png)

At this point you should be able to set the WebAPI project as startup project and launch it. This API exposes 44 different endpoints:

![](https://files.readme.io/a68205c2d899c4e68db496c72de6561204dc12dd4398993f9c56e64f2a622209-image.png)

Each request should include a required header named "ApiKey". The value is configurable is the appsettings.json as well.

![](https://files.readme.io/55b49069fc652a36cc100d0da496cc6541c6a44cec8efacd4ec9df49f058f005-image.png)

At the root directory there is a paste called "postman" that contains an importable collection that can be used to test each endpoint.

![](https://files.readme.io/b7d2597504d7ddf101955dabddffa1e2cf594827cf99dfd20b1a03cde473b6ea-image.png)



To improve in the future:

Add health checks\
Add API Key validation by user
Add a cache mechanism
