FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ZenithBeep.DisCatSharp/ZenithBeep.DisCatSharp.csproj ZenithBeep.DisCatSharp/
COPY ZenithBeep.Data/ZenithBeep.Data.csproj ZenithBeep.Data/
RUN dotnet restore ZenithBeep.DisCatSharp/ZenithBeep.DisCatSharp.csproj --disable-parallel
COPY . .
RUN dotnet publish ZenithBeep.DisCatSharp/ZenithBeep.DisCatSharp.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ZenithBeep.DisCatSharp.dll"]
