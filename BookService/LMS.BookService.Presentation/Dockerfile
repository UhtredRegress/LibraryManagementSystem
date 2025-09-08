FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LMS.BookService.Presentation/LMS.BookService.Presentation.csproj", "LMS.BookService.Presentation/"]
COPY ["LMS.BookService.Domain/LMS.BookService.Domain.csproj", "LMS.BookService.Domain/"]
COPY ["LMS.BookService.Infrastructure/LMS.BookService.Infrastructure.csproj", "LMS.BookService.Infrastructure/"]
RUN dotnet restore "LMS.BookService.Presentation/LMS.BookService.Presentation.csproj"
COPY . .
WORKDIR "/src/LMS.BookService.Presentation"
RUN dotnet build "./LMS.BookService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LMS.BookService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LMS.BookService.Presentation.dll"]
