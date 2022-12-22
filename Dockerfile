FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Savana.Product.API.csproj", "Savana.Product.API/"]
RUN dotnet restore "Savana.Product.API/Savana.Product.API.csproj"
WORKDIR "/src/Savana.Product.API"
COPY . .
RUN dotnet build "Savana.Product.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Savana.Product.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savana.Product.API.dll"]
