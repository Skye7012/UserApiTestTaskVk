#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/UserApiTestTaskVk.Api/UserApiTestTaskVk.Api.csproj", "src/UserApiTestTaskVk.Api/"]
COPY ["src/UserApiTestTaskVk.Contracts/UserApiTestTaskVk.Contracts.csproj", "src/UserApiTestTaskVk.Contracts/"]
COPY ["src/UserApiTestTaskVk.Infrastructure/UserApiTestTaskVk.Infrastructure.csproj", "src/UserApiTestTaskVk.Infrastructure/"]
COPY ["src/UserApiTestTaskVk.Domain/UserApiTestTaskVk.Domain.csproj", "src/UserApiTestTaskVk.Domain/"]
COPY ["src/UserApiTestTaskVk.Application/UserApiTestTaskVk.Application.csproj", "src/UserApiTestTaskVk.Application/"]
RUN dotnet restore "src/UserApiTestTaskVk.Api/UserApiTestTaskVk.Api.csproj"
COPY . .
WORKDIR "/src/src/UserApiTestTaskVk.Api"
RUN dotnet build "UserApiTestTaskVk.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserApiTestTaskVk.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserApiTestTaskVk.Api.dll"]