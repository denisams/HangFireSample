FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HangFireLearn.sln ./
COPY HangFireLearn.api/HangFireLearn.api.csproj HangFireLearn.api/
RUN dotnet restore HangFireLearn.sln

COPY . .
RUN dotnet publish HangFireLearn.api/HangFireLearn.api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HangFireLearn.api.dll"]
