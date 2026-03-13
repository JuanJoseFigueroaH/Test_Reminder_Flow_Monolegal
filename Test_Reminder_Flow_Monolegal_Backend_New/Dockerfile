FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ReminderFlow.Api/ReminderFlow.Api.csproj", "src/ReminderFlow.Api/"]
COPY ["src/ReminderFlow.Application/ReminderFlow.Application.csproj", "src/ReminderFlow.Application/"]
COPY ["src/ReminderFlow.Domain/ReminderFlow.Domain.csproj", "src/ReminderFlow.Domain/"]
COPY ["src/ReminderFlow.Infrastructure/ReminderFlow.Infrastructure.csproj", "src/ReminderFlow.Infrastructure/"]
RUN dotnet restore "src/ReminderFlow.Api/ReminderFlow.Api.csproj"
COPY . .
WORKDIR "/src/src/ReminderFlow.Api"
RUN dotnet build "ReminderFlow.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReminderFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8000
ENTRYPOINT ["dotnet", "ReminderFlow.Api.dll"]
