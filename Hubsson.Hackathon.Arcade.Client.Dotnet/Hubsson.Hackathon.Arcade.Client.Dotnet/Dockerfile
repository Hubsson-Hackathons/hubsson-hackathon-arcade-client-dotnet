FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hubsson.Hackathon.Arcade.Client.Dotnet/Hubsson.Hackathon.Arcade.Client.Dotnet.csproj", "Hubsson.Hackathon.Arcade.Client.Dotnet/"]
RUN dotnet restore "Hubsson.Hackathon.Arcade.Client.Dotnet/Hubsson.Hackathon.Arcade.Client.Dotnet.csproj"
COPY . .
WORKDIR "/src/Hubsson.Hackathon.Arcade.Client.Dotnet"
RUN dotnet build "Hubsson.Hackathon.Arcade.Client.Dotnet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hubsson.Hackathon.Arcade.Client.Dotnet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hubsson.Hackathon.Arcade.Client.Dotnet.dll"]
