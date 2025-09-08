FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5079

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LMS.UserService.Presentation/LMS.UserService.Presentation.csproj", "LMS.UserService.Presentation/"]
COPY ["LMS.UserService.Application/LMS.UserService.Application.csproj", "LMS.UserService.Application/"]
COPY ["LMS.UserService.Infrastructure/LMS.UserService.Infrastructure.csproj", "LMS.UserService.Infrastructure/"]
COPY ["LMS.UserService.Domain/LMS.UserService.Domain.csproj", "LMS.UserService.Domain/"]
COPY ["LMS.Shared/LMS.Shared.csproj", "LMS.Shared/"]
RUN dotnet restore "LMS.UserService.Presentation/LMS.UserService.Presentation.csproj"
COPY . .
WORKDIR "/src/LMS.UserService.Presentation"
RUN dotnet build "./LMS.UserService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LMS.UserService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LMS.UserService.Presentation.dll"]
