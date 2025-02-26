# foreign-exchange-rates
ASP.NET Core Web API to perform CRUD operations of foreign exchange rates

#Install dotnet-ef cli
dotnet tool install --global dotnet-ef

#Database Setup

Create the migration files with the command:
dotnet ef migrations add InitialCreate --project ForeignExchangeRates.Infrastructure --startup-project ForeignExchangeRates.WebAPI

Run the migration files with the command:
dotnet ef database update --project ForeignExchangeRates.Infrastructure --startup-project ForeignExchangeRates.WebAPI
