FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SmartOfferBooking.sln ./
COPY src/SmartOfferBooking.Domain/SmartOfferBooking.Domain.csproj src/SmartOfferBooking.Domain/
COPY src/SmartOfferBooking.Application/SmartOfferBooking.Application.csproj src/SmartOfferBooking.Application/
COPY src/SmartOfferBooking.Infrastructure/SmartOfferBooking.Infrastructure.csproj src/SmartOfferBooking.Infrastructure/
COPY src/SmartOfferBooking.API/SmartOfferBooking.API.csproj src/SmartOfferBooking.API/
RUN dotnet restore
COPY src/ src/
RUN dotnet publish src/SmartOfferBooking.API/SmartOfferBooking.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartOfferBooking.API.dll"]
