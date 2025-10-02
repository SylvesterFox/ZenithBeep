FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY *.sln .
COPY ReworkZenithBeep/*.csproj ReworkZenithBeep/
COPY ReworkZenithBeep.Data/*.csproj ReworkZenithBeep.Data/
COPY ReworkZenithBeep.Data.Migrations/*.csproj ReworkZenithBeep.Data.Migrations/
COPY DataConfig/*.json DataConfig/

RUN dotnet restore
COPY . ./

RUN dotnet publish ReworkZenithBeep/ReworkZenithBeep.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ReworkZenithBeep.dll"]