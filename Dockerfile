FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SaludPortal.Web/SaludPortal.Web.csproj", "SaludPortal.Web/"]
COPY ["AndesServices/AndesServices.csproj", "AndesServices/"]
COPY ["SaludPortal.Application/SaludPortal.Application.csproj", "SaludPortal.Application/"]
COPY ["RecetarServices/RecetarServices.csproj", "RecetarServices/"]
COPY ["LachybsServices/LachybsServices.csproj", "LachybsServices/"]
COPY ["XroadssAndesServices/XroadssAndesServices.csproj", "XroadssAndesServices/"]
COPY ["AdminLogsServices/AdminLogsServices.csproj", "AdminLogsServices/"]

RUN dotnet restore "SaludPortal.Web/SaludPortal.Web.csproj"

COPY . .

RUN dotnet publish "SaludPortal.Web/SaludPortal.Web.csproj" -c Release -o /app/publish --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SaludPortal.Web.dll"]
