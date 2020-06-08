FROM mcr.microsoft.com/dotnet/core/sdk:3.1.300 as build

COPY . /src

RUN dotnet publish -c Release -o /app /src/ProducerConsumer.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.4
COPY --from=build /app /app

EXPOSE 80

WORKDIR /app

ENTRYPOINT ["dotnet", "ProducerConsumer.dll"]
