FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Cathode.Gateway/Cathode.Gateway.csproj", "Cathode.Gateway/"]
RUN dotnet restore "Cathode.Gateway/Cathode.Gateway.csproj"
COPY . .
WORKDIR "/src/Cathode.Gateway"
RUN dotnet build "Cathode.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cathode.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cathode.Gateway.dll"]
