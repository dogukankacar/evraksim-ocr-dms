FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

# Install Tesseract OCR and English/Turkish language packs
RUN apt-get update \
    && apt-get install -y tesseract-ocr tesseract-ocr-tur tesseract-ocr-eng \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy projects
COPY ["DMS.API/DMS.API.csproj", "DMS.API/"]
COPY ["DMS.Application/DMS.Application.csproj", "DMS.Application/"]
COPY ["DMS.Infrastructure/DMS.Infrastructure.csproj", "DMS.Infrastructure/"]
COPY ["DMS.Core/DMS.Core.csproj", "DMS.Core/"]

# Restore dependencies
RUN dotnet restore "DMS.API/DMS.API.csproj"

# Copy the rest
COPY . .
WORKDIR "/src/DMS.API"
RUN dotnet build "DMS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DMS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DMS.API.dll"]
