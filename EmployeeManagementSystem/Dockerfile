
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EmployeeManagementSystem.csproj ./
RUN dotnet restore EmployeeManagementSystem.csproj

COPY . ./
RUN dotnet publish EmployeeManagementSystem.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "EmployeeManagementSystem.dll"]
