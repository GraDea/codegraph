FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["codegraph.ui/codegraph.ui.csproj", "codegraph.ui/"]
RUN dotnet restore "codegraph.ui/codegraph.ui.csproj"
COPY . .
WORKDIR "/src/codegraph.ui"
RUN dotnet build "codegraph.ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "codegraph.ui.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "codegraph.ui.dll"]
