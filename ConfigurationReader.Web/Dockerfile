FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ConfigurationReader.Web/ConfigurationReader.Web.csproj", "ConfigurationReader.Web/"]
COPY ["ConfigurationReader.Core/ConfigurationReader.Core.csproj", "ConfigurationReader.Core/"]
COPY ["ConfigurationReader.Data/ConfigurationReader.Data.csproj", "ConfigurationReader.Data/"]
RUN dotnet restore "ConfigurationReader.Web/ConfigurationReader.Web.csproj"
COPY . .
WORKDIR "/src/ConfigurationReader.Web"
RUN dotnet build "ConfigurationReader.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigurationReader.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationReader.Web.dll"] 